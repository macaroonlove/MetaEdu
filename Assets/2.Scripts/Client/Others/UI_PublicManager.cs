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
    private GameObject _characterModel;
    private Button _loginButton;
    private Button _signupButton;
    private Button _crpButton;
    private TMP_InputField _sPwText;
    private TMP_InputField _srPwText;
    private TextMeshProUGUI _sErrText;

    [Header("페티씬")]
    private ShareCam _agManager;
    private IngamePhotonManager _ptManager;
    private Animator _phoneAnim;
    private Animator _camPanel;
    private GameObject _createRoomPanel;
    private GameObject _createQuizPanel;

    private GameObject _chatPanel;
    private TMP_InputField _sendChat;

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

    private QuizManager _quizManager;
    private bool _battleScene;
    void Start()
    {
        _sceneName = SceneManager.GetActiveScene().name;
        system = EventSystem.current;
        if (_sceneName.Equals("1.Login"))
        {
            GameObject.Find("LID_Input").TryGetComponent(out _idAnim);
            GameObject.Find("LPW_Input").TryGetComponent(out _pwAnim);
            GameObject.Find("LoginPanel").TryGetComponent(out _loginPanelAnim);
            GameObject.Find("SignupPanel").TryGetComponent(out _signupPanelAnim);
            _characterPanel = transform.GetChild(3).gameObject;
            _characterModel = GameObject.Find("CreateCharacter");
            _characterModel.SetActive(false);
            GameObject.Find("Login_Button").TryGetComponent(out _loginButton);
            GameObject.Find("SignUp_Button").TryGetComponent(out _signupButton);
            transform.GetChild(3).GetChild(7).TryGetComponent(out _crpButton);
            GameObject.Find("SPW_Input").TryGetComponent(out _sPwText);
            GameObject.Find("SRPW_Input").TryGetComponent(out _srPwText);
            GameObject.Find("SError_Text").TryGetComponent(out _sErrText);
            _battleScene = false;
        }
        else
        {
            GameObject.Find("AgoraManager").TryGetComponent(out _agManager);
            GameObject.Find("PhotonManager").TryGetComponent(out _ptManager);
            _chatPanel = GameObject.Find("ChatPanel");
            _chatPanel.transform.GetChild(1).TryGetComponent(out _sendChat);
            _setting = gameObject.transform.GetChild(6).gameObject;
            transform.GetChild(4).TryGetComponent(out _phoneAnim);
            transform.GetChild(1).TryGetComponent(out _camPanel);
            _createRoomPanel = transform.GetChild(3).gameObject;
            _createQuizPanel = transform.GetChild(5).gameObject;
            _setting.transform.GetChild(0).TryGetComponent(out _basic);
            _setting.transform.GetChild(1).TryGetComponent(out _control);
            _setting.transform.GetChild(2).TryGetComponent(out _display);
            _setting.transform.GetChild(3).TryGetComponent(out _sound);
            _rebinding = _setting.transform.GetChild(5).GetChild(2).gameObject;
            if (_sceneName.Equals("4.Battle"))
            {
                GameObject.Find("QuizManager").TryGetComponent(out _quizManager);
                _battleScene = true;
            }
            else
            {
                _battleScene = false;
            }
        }
    }

    void Update()
    {
        if (_sceneName.Equals("1.Login"))
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
            if (_chatPanel.activeSelf)
            {
                if (Input.GetKeyDown(KeyCode.Return) && Input.GetKey(KeyCode.RightShift))
                {
                }
                else if (Input.GetKeyDown(KeyCode.Return))
                {
                    if (!_sendChat.text.Trim().Equals(""))
                    {
                        _ptManager.Send();
                    }
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
        if (_idAnim.GetComponent<TMP_InputField>().text.Equals("")) _idAnim.SetBool("ClickInput", false);
    }
    public void SelectPW() => _pwAnim.SetBool("ClickInput", true);

    public void DeSelectPW()
    {
        if (_pwAnim.GetComponent<TMP_InputField>().text.Equals("")) _pwAnim.SetBool("ClickInput", false);
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
        if (!_sPwText.text.Equals("") && !_srPwText.text.Equals(""))
        {
            if (!_sPwText.text.Equals(_srPwText.text))
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
        _characterModel.SetActive(true);
    }
    #endregion

    #region 페티 씬
    #region 폰
    public void Topbar(bool isOn)
    {
        _phoneAnim.SetBool("Open", isOn);
    }

    #region 문제 생성 UI관련
    public void QuizToggle(bool isOn)
    {
        if (isOn.Equals(true))
        {
            QuestionManager.Inst.selectQuestion = int.Parse(system.currentSelectedGameObject.name);
            if(_battleScene)
            {
                _quizManager.Question.Clear();
                _quizManager.InitQuiz();
            }
        }
    }

    public void AnsType(int O)
    {
        for (int i = 0; i < 3; i++)
        {
            if (i.Equals(O))
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
        if (ReferenceEquals(_answerZone, null))
        {
            _answerZone = GameObject.Find("AnswerZone").transform;
        }
        for (int i = 1; i < 9; i++)
        {
            if (!_answerZone.GetChild(i).gameObject.activeSelf)
            {
                _answerZone.GetChild(i).gameObject.SetActive(true);
                if (i.Equals(8))
                {
                    _answerZone.GetChild(9).gameObject.SetActive(false);
                }
                break;
            }
        }
    }

    public void MaxKeyword(string text)
    {
        if (ReferenceEquals(_maxAnswer, null))
        {
            GameObject.Find("MaxKeywordNum").TryGetComponent(out _maxAnswer);
        }
        _maxAnswer.text = (text.Split("#").Length - 1) + "";
    }

    public void AnsKeyword(string text)
    {
        if (ReferenceEquals(_AnsKeyword, null))
        {
            GameObject.Find("AnsKeyword_input").TryGetComponent(out _AnsKeyword);
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

    #region 아고라
    public void CamPanel(bool isOn)
    {
        _camPanel.SetBool("Open", isOn);
    }

    public void CamOnOffButton(bool isOn)
    {
        if (!isOn) // 끄기
        {
            _agManager.OffCam();
        }
        else // 실행
        {
            _agManager.OnCam();
        }
    }

    public void VoiceOnOffButton(bool isOn)
    {
        if (!isOn) // 끄기
        {
            _agManager.OffAudio();
        }
        else
        {
            _agManager.OnAudio();
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
