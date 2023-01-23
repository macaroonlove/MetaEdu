using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun;
using TMPro;

public class RoomPuller : MonoBehaviourPunCallbacks
{
    Action<List<RoomInfo>> callback = null;
    LoadBalancingClient client = null;

    public TextMeshProUGUI subState;

    List<RoomInfo> MyList = new List<RoomInfo>();
    public Button[] JoinRoomButton;
    public Button PrevButton;
    public Button NextButton;
    public TextMeshProUGUI currentPageText;
    private int currentPage = 1;
    private int maxPage, multiple;

    public TMP_InputField RoomName;
    public TMP_Dropdown RoomMax;
    private String[] roomSets = { "3_1.ClassRoom", "3_2.ClassRoom" };
    private String roomSet = "3_1.ClassRoom";

    void OnEnable()
    {
        PullRoomList();
    }

    void OnDisable()
    {
        if (client != null)
            client.Disconnect();
    }

    void Update()
    {
        if (client != null)
        {
            client.Service();
        }
    }

    public void OnGetRoomsInfo(Action<List<RoomInfo>> callback)
    {
        this.callback = callback;
        client = new LoadBalancingClient();
        client.AddCallbackTarget(this);
        client.StateChanged += OnStateChanged;
        client.AppId = PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime;
        client.AppVersion = PhotonNetwork.NetworkingClient.AppVersion;
        client.EnableLobbyStatistics = true;

        client.ConnectToRegionMaster(PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion);
    }

    void OnStateChanged(ClientState previousState, ClientState state)
    {
        subState.text = "서브: " + state;

        if (state == ClientState.ConnectedToMasterServer)
        {
            client.OpJoinLobby(null);
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if (callback != null)
        {
            callback(roomList);
        }
    }


    public void MyListClick(int num)
    {
        if (num == -2) --currentPage;
        else if (num == -1) ++currentPage;
        else
        {
            client.RemoveCallbackTarget(this);
            client.Disconnect();
            client = null;
            callback = null;
            RoomChangeManager.Instance.RoomOut(MyList[multiple + num].Name, 20);
            //PhotonNetwork.LeaveRoom();
            //PhotonNetwork.AddCallbackTarget(this);

            //StartCoroutine("JR", num);
        }
        MyListRenewal();
    }

    //IEnumerator JR(int num)
    //{
    //    yield return new WaitForSeconds(0.5f);
        
    //    //PhotonNetwork.JoinRoom(MyList[multiple + num].Name);
    //}

    void MyListRenewal()
    {
        maxPage = (MyList.Count % JoinRoomButton.Length == 0) ? MyList.Count / JoinRoomButton.Length : MyList.Count / JoinRoomButton.Length + 1;

        PrevButton.interactable = (currentPage <= 1) ? false : true;
        NextButton.interactable = (currentPage >= maxPage) ? false : true;
        currentPageText.text = "" + currentPage;
        multiple = (currentPage - 1) * JoinRoomButton.Length;
        for (int i = 0; i < JoinRoomButton.Length; i++)
        {
            JoinRoomButton[i].interactable = (multiple + i < MyList.Count) ? true : false;
            JoinRoomButton[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = (multiple + i < MyList.Count) ? MyList[multiple + i].PlayerCount + "/" + MyList[multiple + i].MaxPlayers : "";
            JoinRoomButton[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = (multiple + i < MyList.Count) ? MyList[multiple + i].Name.Split("#")[0] : "";
            JoinRoomButton[i].transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = (multiple + i < MyList.Count) ? MyList[multiple + i].Name.Split("#")[1] : "";
        }
    }

    public void PullRoomList()
    {
        OnGetRoomsInfo
        (
            (roomList) =>
            {
                int roomCount = roomList.Count;
                for (int i = 0; i < roomCount; i++)
                {
                    if (!roomList[i].RemovedFromList) // 현재 존재하는 방이라면
                    {
                        if (!MyList.Contains(roomList[i])) // 만약 룸리스트에 i번째 항목이 MyList에 없다면
                        {
                            if (!roomList[i].Name.Contains("2.Campus") && !roomList[i].Name.Contains("4.Battle"))
                                MyList.Add(roomList[i]); // MyList에 룸리스트의 i번째 항목 추가
                        }
                        else
                            MyList[MyList.IndexOf(roomList[i])] = roomList[i];
                    }
                    else if (MyList.IndexOf(roomList[i]) != -1)
                        MyList.RemoveAt(MyList.IndexOf(roomList[i]));
                }
                MyListRenewal();
            }
        );
    }

    public void RoomSet(int i)
    {
        roomSet = roomSets[i];
    }

    public void CreateRoom()
    {
        client.RemoveCallbackTarget(this);
        client.Disconnect();
        client = null;
        callback = null;
        RoomChangeManager.Instance.RoomOut((RoomName.text == "" ? PhotonNetwork.LocalPlayer.NickName + "님의 방" : RoomName.text) + "#" + roomSet, RoomMax.value + 2);

        //Invoke("CR", 0.5f);
    }

    //void CR()
    //{
    //    PhotonNetwork.CreateRoom((RoomName.text == "" ? PhotonNetwork.LocalPlayer.NickName + "님의 방" : RoomName.text) + "#" + roomSet, new RoomOptions { MaxPlayers = (byte)(RoomMax.value + 2) });
    //}

    //public override void OnCreateRoomFailed(short returnCode, string message)
    //{
    //    RoomName.text = "";
    //    CreateRoom();
    //}

    //public void JoinRandomRoom() => PhotonNetwork.JoinRandomRoom();

    //public override void OnJoinRandomFailed(short returnCode, string message)
    //{
    //    RoomName.text = "";
    //    CreateRoom();
    //}

    //public override void OnJoinRoomFailed(short returnCode, string message)
    //{
    //    PhotonNetwork.JoinOrCreateRoom("Campus", new RoomOptions { MaxPlayers = 20 }, null);
    //    PhotonNetwork.LoadLevel(1);
    //}

    //public override void OnJoinedRoom()
    //{
    //    Singleton.Inst.isPatty = false;
    //    PhotonNetwork.LoadLevel(PhotonNetwork.CurrentRoom.Name.Split("#")[1]);
    //    PhotonNetwork.IsMessageQueueRunning = false;
    //}
}