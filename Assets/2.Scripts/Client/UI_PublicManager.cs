using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class UI_PublicManager : MonoBehaviour
{
    private EventSystem system;
    private string _sceneName;

    [Header("로그인")]
    private Animator _idAnim;
    private Animator _pwAnim;
    private Animator _loginPanelAnim;
    private Animator _signupPanelAnim;
    private GameObject _characterPanel;
    private Button _loginButton;
    private Button _signupButton;
    private Button _crpButton;
    private TMP_InputField _sPwText;
    private TMP_InputField _srPwText;
    private TextMeshProUGUI _sErrText;

    [Header("페티씬")]
    private ShareCam _agManager;
    private IngamePhotonManager _ptManager;
    private GameObject _createRoomPanel;
    private GameObject _createQuizPanel;

    private bool _camState = false;
    private bool _voiceState = false;
    private Animator _sideBarAnim;
    private Toggle _camToggle;
    private Toggle _quizToggle;

    public Sprite[] chatButtonSprite;
    private Image _chatOnOffImg;
    private GameObject _chatPanel;
    private GameObject _chatInput;
    private TMP_InputField _sendChat;
    private bool _chatState = false;

    [Header("문제만들기")]
    public GameObject[] ansType;
    private Transform _answerZone = null;
    private TextMeshProUGUI _maxAnswer = null;
    private TMP_InputField _AnsKeyword = null;

    [Header("설정창")]
    private GameObject _setting;
    private GameObject _rebinding;
    private TextMeshProUGUI _basic;
    private TextMeshProUGUI _control;
    private TextMeshProUGUI _display;
    private TextMeshProUGUI _sound;

    void Start()
    {
        _sceneName = SceneManager.GetActiveScene().name;
        system = EventSystem.current;
        if (_sceneName == "1.Login")
        {
            _idAnim = GameObject.Find("LID_Input").GetComponent<Animator>();
            _pwAnim = GameObject.Find("LPW_Input").GetComponent<Animator>();
            _loginPanelAnim = GameObject.Find("LoginPanel").GetComponent<Animator>();
            _signupPanelAnim = GameObject.Find("SignupPanel").GetComponent<Animator>();
            _characterPanel = transform.GetChild(3).gameObject;
            _loginButton = GameObject.Find("Login_Button").GetComponent<Button>();
            _signupButton = GameObject.Find("SignUp_Button").GetComponent<Button>();
            _crpButton = transform.GetChild(3).GetChild(7).GetComponent<Button>();
            _sPwText = GameObject.Find("SPW_Input").GetComponent<TMP_InputField>();
            _srPwText = GameObject.Find("SRPW_Input").GetComponent<TMP_InputField>();
            _sErrText = GameObject.Find("SError_Text").GetComponent<TextMeshProUGUI>();
        }
        else
        {
            _agManager = GameObject.Find("AgoraManager").GetComponent<ShareCam>();
            _ptManager = GameObject.Find("PhotonManager")?.GetComponent<IngamePhotonManager>();
            _sideBarAnim = GameObject.Find("LSideBar").GetComponent<Animator>();
            _camToggle = _sideBarAnim.transform.GetChild(0).GetComponent<Toggle>();
            _quizToggle = _sideBarAnim.transform.GetChild(1).GetComponent<Toggle>();
            _chatOnOffImg = GameObject.Find("ChatOnOff").GetComponent<Image>();
            _chatPanel = _chatOnOffImg.transform.GetChild(0).gameObject;
            _chatInput = _chatPanel.transform.GetChild(1).gameObject;
            _sendChat = _chatInput.GetComponent<TMP_InputField>();
            _setting = gameObject.transform.GetChild(10).gameObject;
            _createRoomPanel = transform.GetChild(8).gameObject;
            _createQuizPanel = transform.GetChild(9).gameObject;
            _basic = _setting.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            _control = _setting.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            _display = _setting.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
            _sound = _setting.transform.GetChild(3).GetComponent<TextMeshProUGUI>();
            _rebinding = _setting.transform.GetChild(5).GetChild(2).gameObject;
        }
    }

    void Update()
    {
        if (_sceneName == "1.Login")
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                // Tab은 아래의 Selectable 객체를 선택
                Selectable next = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();
                if (next != null)
                {
                    next.Select();
                }
            }
            else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                if (_signupPanelAnim.GetBool("On"))
                {
                    _signupButton.onClick.Invoke();
                }
                else if (_characterPanel.activeSelf)
                {
                    _crpButton.onClick.Invoke();
                }
                else
                {
                    _loginButton.onClick.Invoke();
                }

            }
        }
        else
        {
            if (_chatState)
            {
                if (Input.GetKeyDown(KeyCode.Return) && !_chatInput.activeSelf)
                {
                    _chatInput.SetActive(true);
                    _sendChat.ActivateInputField();
                }
                else if (Input.GetKeyDown(KeyCode.Return) && _chatInput.activeSelf)
                {
                    if (_sendChat.text != "")
                        _ptManager.Send();
                    _chatInput.SetActive(false);
                }
            }

            if (Input.GetKeyDown(KeyCode.Escape) && !_rebinding.activeSelf)
            {
                Set_setting();
            }
        }
    }



    public void GameExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    #region 로그인 씬
    public void SelectID() => _idAnim.SetBool("ClickInput", true);

    public void DeSelectID()
    {
        if (_idAnim.GetComponent<TMP_InputField>().text == "") _idAnim.SetBool("ClickInput", false);
    }
    public void SelectPW() => _pwAnim.SetBool("ClickInput", true);

    public void DeSelectPW()
    {
        if (_pwAnim.GetComponent<TMP_InputField>().text == "") _pwAnim.SetBool("ClickInput", false);
    }


    public void SignupPannelOC(int i)
    {
        if (i == 0)
        {
            _loginPanelAnim.SetBool("On", true);
            _signupPanelAnim.SetBool("On", true);
        }
        else if (i == 1)
        {
            _loginPanelAnim.SetBool("On", false);
            _signupPanelAnim.SetBool("On", false);
        }
    }

    public void PW_Match()
    {
        if (_sPwText.text != "" && _srPwText.text != "")
        {
            if (_sPwText.text != _srPwText.text)
            {
                _sErrText.text = "비밀번호가 일치하지 않습니다.";
            }
        }
        else
            _sErrText.text = "";
    }

    public void CRPanel()
    {
        _loginPanelAnim.SetBool("On", true);
        _characterPanel.SetActive(true);
    }
    #endregion

    #region 페티 씬
    #region 사이드바
    public void LSideBar()
    {
        if (_camToggle.isOn)
        {
            _quizToggle.transform.GetChild(1).gameObject.SetActive(false);
            if (!_sideBarAnim.GetBool("sidebarState"))
                _sideBarAnim.SetBool("sidebarState", true);
        }
        else if (_quizToggle.isOn)
        {
            _quizToggle.transform.GetChild(1).gameObject.SetActive(true);
            if (!_sideBarAnim.GetBool("sidebarState"))
                _sideBarAnim.SetBool("sidebarState", true);
        }
        else
        {
            _sideBarAnim.SetBool("sidebarState", false);
        }
    }

    #region 문제 생성 UI관련
    public void QuizToggle(bool isOn)
    {
        if (isOn == true)
        {
            Singleton.Inst.currSelect = int.Parse(system.currentSelectedGameObject.name);
        }
    }

    public void AnsType(int O)
    {
        for (int i = 0; i < 3; i++)
        {
            if (i == O)
                ansType[O].SetActive(true);
            else
            {
                TMP_InputField[] at = ansType[i].GetComponentsInChildren<TMP_InputField>(true);
                for (int k = 0; k < at.Length; k++)
                {
                    at[k].text = "";
                }
                ansType[i].SetActive(false);
            }
        }
    }

    public void AnsItem(bool isOn)
    {
        if (isOn)
        {
            system.currentSelectedGameObject.transform.GetChild(1).gameObject.SetActive(true);
        }
        else
        {
            Transform ai = system.currentSelectedGameObject.transform.GetChild(1);
            ai.GetComponent<TMP_InputField>().text = "";
            ai.gameObject.SetActive(false);
        }
    }

    public void AddAnswer()
    {
        if (_answerZone == null)
        {
            _answerZone = GameObject.Find("AnswerZone").transform;
        }
        for (int i = 1; i < 9; i++)
        {
            if (!_answerZone.GetChild(i).gameObject.activeSelf)
            {
                _answerZone.GetChild(i).gameObject.SetActive(true);
                if (i == 8)
                {
                    _answerZone.GetChild(9).gameObject.SetActive(false);
                }
                break;
            }
        }
    }

    public void MaxKeyword(string text)
    {
        if (_maxAnswer == null)
        {
            _maxAnswer = GameObject.Find("MaxKeywordNum").GetComponent<TextMeshProUGUI>();
        }
        _maxAnswer.text = (text.Split("#").Length - 1) + "";
    }

    public void AnsKeyword(string text)
    {
        if (_AnsKeyword == null)
        {
            _AnsKeyword = GameObject.Find("AnsKeyword_input").GetComponent<TMP_InputField>();
        }
        if (int.Parse(text) < 0)
        {
            _AnsKeyword.text = "0";
        }
        else if (int.Parse(text) > int.Parse(_maxAnswer.text))
        {
            _AnsKeyword.text = _maxAnswer.text;
        }
    }
    #endregion

    #region 아고라UI관련
    public void CamOnOffButton()
    {
        if (_camState) // 끄기
        {
            _agManager.OffCam();
            _camState = false;
        }
        else // 실행
        {
            _agManager.OnCam();
            _camState = true;
        }
    }

    public void VoiceOnOffButton()
    {
        if (_voiceState) // 끄기
        {
            _agManager.OffAudio();
            _voiceState = false;
        }
        else
        {
            _agManager.OnAudio();
            _voiceState = true;
        }
    }

    public void ChatOnOffButton()
    {
        if (_chatState) // 끄기
        {
            _chatPanel.SetActive(false);
            _chatOnOffImg.sprite = chatButtonSprite[0];
            _chatState = false;
            _sendChat.text = "";
            _chatInput.SetActive(false);
        }
        else
        {
            _chatPanel.SetActive(true);
            _chatOnOffImg.sprite = chatButtonSprite[1];
            _chatState = true;
        }
    }
    #endregion
    #endregion

    #region 세팅
    public void Set_setting()
    {
        if (_setting.activeSelf)
        {
            if (!Singleton.Inst.controled)
            {
                _setting.SetActive(false);
                if (!_createRoomPanel.activeSelf && !_createQuizPanel.activeSelf) Cursor.lockState = CursorLockMode.Locked;
            }
            else
                Singleton.Inst.controled = false;
        }
        else
        {
            _setting.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void SetTitleColor()
    {
        _basic.color = new Color(0.76f, 0.76f, 0.76f);
        _control.color = new Color(0.76f, 0.76f, 0.76f);
        _display.color = new Color(0.76f, 0.76f, 0.76f);
        _sound.color = new Color(0.76f, 0.76f, 0.76f);
        EventSystem.current.currentSelectedGameObject.GetComponent<TextMeshProUGUI>().color = new Color(0.73f, 0.73f, 0.15f);
    }
    #endregion
    #endregion
}
