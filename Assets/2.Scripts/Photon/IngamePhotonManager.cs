using UnityEngine;
using UnityEngine.SceneManagement;
using PlayFab;
using PlayFab.ClientModels;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class IngamePhotonManager : MonoBehaviourPunCallbacks
{
    public PhotonView PV;

    public Transform quizContent;

    private Vector3 _startPosition;
    private Quaternion _startRotation;
    private bool _isCreate = false;
    private string _sex;

    public GameObject selectEL;

    public TMP_InputField sendChat;
    public TextMeshProUGUI[] chatText;

    public TextMeshProUGUI roomName;
    public TextMeshProUGUI roomType;
    public TextMeshProUGUI playerNum;
    public TextMeshProUGUI[] attendance;

    void Awake() // 2번 실행
    {
        _isCreate = false;
        string _sn = SceneManager.GetActiveScene().name;
        if (_sn.Equals("2.Campus"))
        {
            _startPosition = new Vector3(-25f, 0.3f, 25f);
            _startRotation = Quaternion.Euler(0, -45.0f, 0f);
            roomName.transform.parent.gameObject.SetActive(false);
        }
        else if (_sn.Equals("3_1.ClassRoom"))
        {
            _startPosition = new Vector3(5.5f, -2f, -1.25f);
            _startRotation = Quaternion.Euler(0, -90.0f, 0f);
            roomName.text = "이름: " + PhotonNetwork.CurrentRoom.Name.Split("#")[0];
            roomType.text = "종류: 집중형 강의실";
        }
        else if (_sn.Equals("3_2.ClassRoom"))
        {
            _startPosition = new Vector3(3.5f, -2.1f, -1.5f);
            _startRotation = Quaternion.Euler(0, -90.0f, 0f);
            roomName.text = "이름: " + PhotonNetwork.CurrentRoom.Name.Split("#")[0];
            roomType.text = "종류: 토론형 강의실";
        }
        else if (_sn.Equals("4.Battle"))
        {
            _startPosition = new Vector3(-25f, 0.3f, 25f);
            _startRotation = Quaternion.Euler(0, -45.0f, 0f);
            roomName.text = "이름: " + PhotonNetwork.CurrentRoom.Name.Split("#")[0];
            roomType.text = "종류: 턴제 퀴즈";
        }
        else if (_sn.Equals("5.Goldenball"))
        {
            _startPosition = new Vector3(-3f, 0f, 0.5f);
            _startRotation = Quaternion.Euler(0, 60f, 0f);
            roomName.text = "이름: " + PhotonNetwork.CurrentRoom.Name.Split("#")[0];
            roomType.text = "종류: 골든벨";
        }

        // 캐릭터 생성
        var request = new GetUserDataRequest() { PlayFabId = Singleton.Inst.Playfab_ID };
        PlayFabClientAPI.GetUserData(request, GetDataSuccess, (error) => print("실패"));
        PhotonNetwork.IsMessageQueueRunning = true;
    }

    void Start()
    {
        for (int i = 0; i < chatText.Length; i++) chatText[i].text = "";
    }

    public void BackLobby()
    {
        if (SceneManager.GetActiveScene().name.Equals("2.Campus"))
        {
            selectEL.SetActive(true);
        }
        else
        {
            RoomChangeManager.Instance.RoomOut("Campus#2.Campus", 20, 0);
        }
    }

    public void ExitOrLogin(bool exit)
    {
        if (exit)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }
        else
        {
            Singleton.Inst.isPatty = false;
            PhotonNetwork.Disconnect();
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        SceneManager.LoadScene("1.Login");
    }

    #region 캐릭터 생성 및 문제 초기화 작업
    void GetDataSuccess(GetUserDataResult result)
    {
        _sex = result.Data["Sex"].Value;
        
        PhotonNetwork.LocalPlayer.NickName = result.Data["NickName"].Value;
        if (!_isCreate) CreateCharacter();

        if (Singleton.Inst.questions.Equals(""))
        {
            Singleton.Inst.questions = result.Data["Question"].Value;
        }
        Singleton.Inst.QuestionInit();
    }

    void CreateCharacter()
    {
        GameObject myCharacter = PhotonNetwork.Instantiate(_sex, _startPosition, _startRotation);
        myCharacter.name = PhotonNetwork.LocalPlayer.NickName;
        myCharacter.tag = "Player";
        myCharacter.layer = 7;
        _isCreate = true;

        GameObject.Find("AgoraManager").GetComponent<ShareCam>().enabled = true;
        GameObject.Find("Stick").GetComponent<VirtualJoystick>().enabled = true;
        GameObject.Find("Look").GetComponent<VirtualJoystick>().enabled = true;
    }
    #endregion

    #region 채팅 구현
    public void Send()
    {
        PV.RPC(nameof(Chating), RpcTarget.All, "<color=\"green\"><size=\"20\">" + PhotonNetwork.LocalPlayer.NickName + "</size></color>\n" + sendChat.text);
        sendChat.text = "";
    }

    [PunRPC]
    void Chating(string msg)
    {
        bool isInput = false;
        
        for (int i = 0; i < chatText.Length; i++)
        {
            if (chatText[i].text == "")
            {
                isInput = true;
                chatText[i].text = msg;
                break;
            }
        }
        if (!isInput)
        {
            for (int i = 1; i < chatText.Length; i++)
                chatText[i - 1].text = chatText[i].text;
            chatText[chatText.Length - 1].text = msg;
        }
        sendChat.ActivateInputField();
    }
    #endregion

    #region 출석부
    public void Attendance(bool isOn)
    {
        playerNum.text = $"출석부 <size=25>({PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers})</size>";
        if (isOn)
        {
            for (int i = 0; i < attendance.Length; i++)
            {
                if (i < PhotonNetwork.PlayerList.Length)
                {
                    attendance[i].text = PhotonNetwork.PlayerList[i].NickName;
                }
                else
                {
                    attendance[i].text = "";
                }
            }
        }
    }

    #endregion
}
