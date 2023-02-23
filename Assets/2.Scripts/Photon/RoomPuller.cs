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

    void Awake()
    {
        PullRoomList();
        gameObject.SetActive(false);
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
            RoomChangeManager.Instance.RoomOut(MyList[multiple + num].Name, 20, 1);
        }
        MyListRenewal();
    }

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
                    if (!roomList[i].RemovedFromList) // ���� �����ϴ� ���̶��
                    {
                        if (!MyList.Contains(roomList[i])) // ���� �븮��Ʈ�� i��° �׸��� MyList�� ���ٸ�
                        {
                            if (!roomList[i].Name.Split("#")[0].Equals("")) //!roomList[i].Name.Contains("#2.Campus") && 
                                MyList.Add(roomList[i]); // MyList�� �븮��Ʈ�� i��° �׸� �߰�
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
        RoomChangeManager.Instance.RoomOut((RoomName.text == "" ? PhotonNetwork.LocalPlayer.NickName + "���� ��" : RoomName.text) + "#" + roomSet + "#" + UtilClass.GenerateToken(10), RoomMax.value + 2, 1);
    }
}