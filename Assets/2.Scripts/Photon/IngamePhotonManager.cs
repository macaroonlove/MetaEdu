using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using PlayFab;
using PlayFab.ClientModels;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class IngamePhotonManager : MonoBehaviourPunCallbacks
{
    public PhotonView PV;

    public GameObject QuizManager;
    public GameObject BattleUI;

    public Transform quizContent;

    Vector3 StartPosition;
    Quaternion StartRotation;
    bool _isCreate = false;
    string _sex;

    public TMP_InputField SendChat;
    public TextMeshProUGUI[] ChatText;

    void Awake() // 2번 실행
    {
        _isCreate = false;
        string _sn = SceneManager.GetActiveScene().name;
        if (_sn.Equals("2.Campus"))
        {
            StartPosition = new Vector3(-25f, 0.3f, 25f);
            StartRotation = Quaternion.Euler(0, -45.0f, 0f);
        }
        else if (_sn.Equals("3_1.ClassRoom"))
        {
            StartPosition = new Vector3(5.5f, -2f, -1.25f);
            StartRotation = Quaternion.Euler(0, -90.0f, 0f);
        }
        else if (_sn.Equals("4.Battle"))
        {
            StartPosition = new Vector3(-25f, 0.3f, 25f);
            StartRotation = Quaternion.Euler(0, -45.0f, 0f);
            QuizManager.SetActive(true);
            BattleUI.SetActive(true);
        }

        // 캐릭터 생성
        var request = new GetUserDataRequest() { PlayFabId = Singleton.Inst.Playfab_ID };
        PlayFabClientAPI.GetUserData(request, GetDataSuccess, (error) => print("실패"));
        PhotonNetwork.IsMessageQueueRunning = true;
    }

    void Start()
    {
        for (int i = 0; i < ChatText.Length; i++) ChatText[i].text = "";
    }

    void Update()
    {
    }

    public void BackLobby()
    {
        if (SceneManager.GetActiveScene().name.Equals("2.Campus"))
        {
            Singleton.Inst.isPatty = false;
            PhotonNetwork.Disconnect();
        }
        else
        {
            RoomChangeManager.Instance.RoomOut("Campus#2.Campus", 20);
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        SceneManager.LoadScene("1.Login");
    }

    #region 캐릭터 생성 및 문제 초기화 작업
    void GetDataSuccess(GetUserDataResult result)
    {
        string s = result.Data["Sex"].Value;
        if (s == "M")
        {
            _sex = "M";
        }
        else if (s == "Q")
        {
            _sex = "Q";
        }
        else
        {
            _sex = "W";

        }
        PhotonNetwork.LocalPlayer.NickName = result.Data["NickName"].Value;
        if (!_isCreate) CreateCharacter();

        if (Singleton.Inst.questions == "")
        {
            Singleton.Inst.questions = result.Data["Question"].Value;
        }
        Singleton.Inst.QuestionInit();
    }

    void CreateCharacter()
    {
        GameObject myCharacter = PhotonNetwork.Instantiate(_sex, StartPosition, StartRotation);
        myCharacter.name = PhotonNetwork.LocalPlayer.NickName;
        myCharacter.tag = "Player";
        myCharacter.layer = 7;
        _isCreate = true;

        if (_sex.Equals("Q"))
        {
            for(int i = 3; i < 9; i++)
            {
                myCharacter.transform.GetChild(i).gameObject.layer = 12;
            }
        }

        GameObject.Find("AgoraManager").GetComponent<ShareCam>().enabled = true;
    }
    #endregion

    #region 채팅 구현
    public void Send()
    {
        PV.RPC(nameof(Chating), RpcTarget.All, "<color=\"green\">" + PhotonNetwork.LocalPlayer.NickName + "</color>\n" + SendChat.text);
        SendChat.text = "";
    }

    [PunRPC]
    void Chating(string msg)
    {
        bool isInput = false;
        for (int i = 0; i < ChatText.Length; i++)
        {
            if (ChatText[i].text == "")
            {
                isInput = true;
                ChatText[i].text = msg;
                break;
            }
        }
        if (!isInput)
        {
            for (int i = 1; i < ChatText.Length; i++)
                ChatText[i - 1].text = ChatText[i].text;
            ChatText[ChatText.Length - 1].text = msg;
        }
    }
    #endregion
}
