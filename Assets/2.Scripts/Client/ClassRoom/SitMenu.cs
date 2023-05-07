using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class SitMenu : MonoBehaviour, IPointerDownHandler, IDragHandler ,IEndDragHandler
{
    public static int tableGroup = 0;
    public static bool groupChannel;
    private EventSystem _system;
    private PlayerController _playerController;
    private ShareCam _agoraManager;
    private Animator _anim;
    private GameObject _mCanvas;
    private Color _skyBlue = new Color(0, 1, 1);
    [SerializeField]
    private Transform _targetTr; // 이동될 UI

    private Vector2 _startingPoint;
    private Vector2 _moveBegin;
    private Vector2 _moveOffset;

    public static bool isGetUp = true;
    public Canvas[] dashBoard;
    public RectTransform[] dashBoardRect;
    private Vector2 _vt = new Vector2(3.84f, 2.16f);
    private Vector2 _st = new Vector2(1f, 1f);

    [SerializeField]
    private TextMeshProUGUI _minimalizeText;

    private void Awake()
    {
        TryGetComponent(out _anim);

        _system = EventSystem.current;
        groupChannel = false;
        // 이동 대상 UI를 지정하지 않은 경우, 자동으로 부모로 초기화
        if (ReferenceEquals(_targetTr, null))
            _targetTr = transform.parent;
    }

    private void OnEnable()
    {
        if(ReferenceEquals(_playerController, null))
        {
            GameObject.Find(PhotonNetwork.LocalPlayer.NickName).TryGetComponent(out _playerController);
            GameObject.Find("AgoraManager").TryGetComponent(out _agoraManager);
            _mCanvas = GameObject.Find("Mobile_Canvas");
        }
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        _startingPoint = _targetTr.position;
        _moveBegin = eventData.position;
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        _moveOffset = eventData.position - _moveBegin;
        _targetTr.position = _startingPoint + _moveOffset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        float x = Mathf.Clamp(transform.position.x, 50, 1990);
        float y = Mathf.Clamp(transform.position.y, -70, 1050);
        _targetTr.position = new Vector2(x, y);
    }

    public void AnimTrigger(int num)
    {
        _playerController.AnimTrigger(num);
    }

    public void FullScreen(bool isOn)
    {
        if (isOn)
        {
            _system.currentSelectedGameObject.transform.GetChild(0).GetComponent<Image>().color = _skyBlue;
            _system.currentSelectedGameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = _skyBlue;
            if (groupChannel)
            {
                dashBoard[tableGroup].transform.parent.rotation = Quaternion.Euler(90, 0, 0);
                dashBoard[tableGroup].renderMode = RenderMode.ScreenSpaceOverlay;
                dashBoard[tableGroup].sortingOrder = -1;
            }
            else
            {
                dashBoard[0].renderMode = RenderMode.ScreenSpaceOverlay;
                dashBoard[0].sortingOrder = -1;
            }
            _mCanvas.SetActive(false);
            isGetUp = false;
        }
        else
        {
            _system.currentSelectedGameObject.transform.GetChild(0).GetComponent<Image>().color = Color.white;
            _system.currentSelectedGameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = Color.white;
            if (groupChannel)
            {
                dashBoard[tableGroup].transform.parent.localRotation = Quaternion.Euler(80, 0, 0);
                dashBoard[tableGroup].renderMode = RenderMode.WorldSpace;
                dashBoardRect[tableGroup].sizeDelta = _st;
                dashBoardRect[tableGroup].anchoredPosition3D = Vector3.zero;
                dashBoardRect[tableGroup].localScale = Vector3.one;
                dashBoardRect[tableGroup].localRotation = Quaternion.Euler(0, 180, 0);
            }
            else
            {
                dashBoard[0].renderMode = RenderMode.WorldSpace;
                dashBoardRect[0].sizeDelta = _vt;
                dashBoardRect[0].anchoredPosition = Vector2.zero;
            }
            _mCanvas.SetActive(true);
            isGetUp = true;
        }
    }

    public void Minimalize(bool isOn)
    {
        _anim.SetBool("isMin", isOn);
        _minimalizeText.text = isOn ? "최대화" : "최소화";
        _system.currentSelectedGameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = isOn ? _skyBlue : Color.white;
        _system.currentSelectedGameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = isOn ? _skyBlue : Color.white;
    }

    public void SmallRoom(bool isOn)
    {
        groupChannel = isOn;
        _system.currentSelectedGameObject.transform.GetChild(0).GetComponent<Image>().color = isOn ? _skyBlue : Color.white;
        _system.currentSelectedGameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = isOn ? _skyBlue : Color.white;
        _agoraManager.SmallRoom(isOn);
    }

    void OnDestroy()
    {
        isGetUp = true;
    }
}