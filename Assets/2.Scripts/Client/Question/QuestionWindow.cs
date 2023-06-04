using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Cinemachine;
using TMPro;
using System.Linq;

public class QuestionWindow : MonoBehaviour
{
    public TextMeshProUGUI[] title;
    public TextMeshProUGUI[] desc;
    public TextMeshProUGUI[] download;
    public TextMeshProUGUI[] who;
    public TextMeshProUGUI currPage;
    public Button prevBtn, nextBtn;
    public Button closePhone;
    public Scrollbar scroll;
    public GameObject downloadPanel, deletePanel, sharePanel, screenSaver;

    private Camera _mainCamera;
    private CinemachineVirtualCamera _questCam;
    private PlayerInputPress _playerInputPress;
    private PlayerInput _playerInput;
    private PlayerController _playerController;
    private GameObject _aggroUI;
    private Animator _phoneAnim;
    private TMP_InputField _title, _desc;
    private Button _sharingBtn;
    private bool _isWatch = false;

    private QuestionData[] _tempArray = new QuestionData[25];
    private int _page = 1;
    private int _maxPage, _multiple = 0;
    private int _orderCategory = 0;
    private int _downloadNum;

    void Start()
    {
        _mainCamera = Camera.main;
        transform.GetChild(0).GetComponent<Canvas>().worldCamera = _mainCamera;
        transform.GetChild(1).TryGetComponent(out _questCam);
        GameObject.Find("Phone").TryGetComponent(out _phoneAnim);
        _phoneAnim.transform.GetChild(3).GetChild(0).GetChild(4).TryGetComponent(out _sharingBtn);
        _aggroUI = transform.GetChild(0).GetChild(1).gameObject;
        sharePanel.transform.GetChild(1).GetChild(0).TryGetComponent(out _title);
        sharePanel.transform.GetChild(1).GetChild(1).TryGetComponent(out _desc);

        closePhone.onClick.AddListener(() => QuestionControl(false));
        _sharingBtn.onClick.AddListener(() => SharingPanel());
    }

    public async void PanelInit()
    {
        if(await QuestionManager.Inst.GetTitleData())
        {
            _tempArray = Enumerable.Reverse(QuestionManager.Inst.titleDatas).ToArray(); // 최신순
            if (_orderCategory.Equals(1))
                _tempArray = _tempArray.OrderBy(x => x.download).Reverse().ToArray(); // 인기순
            else if (_orderCategory.Equals(2))
                _tempArray = _tempArray.Where(x => x.owner == Singleton.Inst.displayId).Reverse().ToArray(); // 등록한 문제
            
            _maxPage = (_tempArray.Length % 25).Equals(0) ? _tempArray.Length / 25 : _tempArray.Length / 25 + 1;

            prevBtn.interactable = (_page <= 1) ? false : true;
            nextBtn.interactable = (_page >= _maxPage) ? false : true;
            currPage.text = "" + _page;

            for (int i = 0; i < 25; i++)
            {
                _multiple = (_page - 1) * 25 + i;
                if (_tempArray.Length > _multiple)
                {
                    title[i].transform.parent.gameObject.SetActive(true);

                    title[i].text = _tempArray[_multiple].title;
                    desc[i].text = _tempArray[_multiple].desc;
                    download[i].text = _tempArray[_multiple].download.ToString();
                    who[i].text = _tempArray[_multiple].owner;
                }
                else
                {
                    title[i].transform.parent.gameObject.SetActive(false);
                }
            }
        }
        scroll.value = 1;
    }

    public void CategoryControl(int i)
    {
        _orderCategory = i;
        PanelInit();
    }

    public void IsDownload(int i)
    {
        _downloadNum = i;
        if (_orderCategory.Equals(2))
            deletePanel.SetActive(true);
        else
            downloadPanel.SetActive(true);
    }

    public async void DownloadQuestion()
    {
        QuestionManager.Inst.questionDatas.Add(_tempArray[_downloadNum]);
        int idx = QuestionManager.Inst.titleDatas.FindIndex(x => x.Equals(_tempArray[_downloadNum]));

        if (await QuestionManager.Inst.SetUserData())
        {
            QuestionManager.Inst.QuestionInit();
            if (!QuestionManager.Inst.titleDatas[idx].downloader.Contains(Singleton.Inst.displayId))
            {
                if (await QuestionManager.Inst.DownloadPlus(idx))
                {
                    PanelInit();
                }
            }
        }
    }

    public void QuestionControl(bool isOn)
    {
        _phoneAnim.SetBool("Sharing", isOn);
        closePhone.gameObject.SetActive(isOn);
        _sharingBtn.gameObject.SetActive(isOn);
    }

    private void SharingPanel()
    {
        _title.text = "";
        _desc.text = "";
        SharingLimit(true);
        QuestionControl(false);
    }

    public async void SharingQuestion()
    {
        if (QuestionManager.Inst.questionDatas[QuestionManager.Inst.selectQuestion].owner.Equals(Singleton.Inst.displayId))
        {
            if (await QuestionManager.Inst.AddQuestion(_title.text, _desc.text))
            {
                PanelInit();
                SharingLimit(false);
            }
        }
    }

    public void SharingLimit(bool tf)
    {
        sharePanel.SetActive(tf);
        _playerInput.enabled = !tf;
    }

    public async void DeleteQuestion()
    {
        int idx = QuestionManager.Inst.titleDatas.FindIndex(x => x.Equals(_tempArray[_downloadNum]));

        if (await QuestionManager.Inst.DeleteQuestion(idx))
        {
            PanelInit();
            SharingLimit(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (ReferenceEquals(_playerInputPress, null))
            {
                PanelInit();
                other.TryGetComponent(out _playerInput);
                other.TryGetComponent(out _playerInputPress);
                other.TryGetComponent(out _playerController);
            }
            _aggroUI.SetActive(true);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (_playerInputPress.interact)
            {
                if (!_isWatch)
                {
                    screenSaver.SetActive(false);
                    _aggroUI.SetActive(false);
                    _playerController.enabled = false;
                    _mainCamera.cullingMask = _mainCamera.cullingMask & ~(1 << 7) & ~(1 << 8) & ~(1 << 12);
                    _questCam.Priority = 30;
                    _isWatch = true;
                    _phoneAnim.SetBool("Phone", false);
                    _phoneAnim.SetBool("Open", false);
                }
                else
                {
                    screenSaver.SetActive(true);
                    _aggroUI.SetActive(true);
                    _playerController.enabled = true;
                    _mainCamera.cullingMask = -1;
                    _questCam.Priority = 1;
                    _isWatch = false;
                    _phoneAnim.SetBool("Phone", false);
                    _phoneAnim.SetBool("Open", false);
                    _phoneAnim.SetBool("Sharing", false);
                    closePhone.gameObject.SetActive(false);
                }
                _playerInputPress.interact = false;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _aggroUI.SetActive(false);
        }
    }
}