using System;
using System.Collections.Generic;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;
using TMPro;

public class RoomPuller : MonoBehaviourPunCallbacks
{
    Action<List<RoomInfo>> callback = null;
    LoadBalancingClient client = null;

    List<RoomInfo> _myList = new List<RoomInfo>();

    public Button[] joinRoomButton;
    public Button prevButton;
    public Button nextButton;
    public TextMeshProUGUI currentPageText;
    private int _currentPage = 1;
    private int _maxPage, _multiple;

    public TMP_InputField roomName;
    public TMP_Dropdown roomMax;
    private string[] _roomSets = { "3_1.ClassRoom", "3_2.ClassRoom" };
    private string _roomSet = "3_1.ClassRoom";

    void Awake()
    {
        PullRoomList();
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (!ReferenceEquals(client, null))
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
        if (state.Equals(ClientState.ConnectedToMasterServer))
        {
            client.OpJoinLobby(null);
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if (!ReferenceEquals(callback, null))
        {
            callback(roomList);
        }
    }

    public void MyListClick(int num)
    {
        if (num.Equals(-2)) --_currentPage;
        else if (num.Equals(-1)) ++_currentPage;
        else
        {
            client.RemoveCallbackTarget(this);
            client.Disconnect();
            client = null;
            callback = null;
            RoomChangeManager.Instance.RoomOut(_myList[_multiple + num].Name, 20, 1);
        }
        MyListRenewal();
    }

    void MyListRenewal()
    {
        _maxPage = (_myList.Count % joinRoomButton.Length).Equals(0) ? _myList.Count / joinRoomButton.Length : _myList.Count / joinRoomButton.Length + 1;

        prevButton.interactable = (_currentPage <= 1) ? false : true;
        nextButton.interactable = (_currentPage >= _maxPage) ? false : true;
        currentPageText.text = "" + _currentPage;
        _multiple = (_currentPage - 1) * joinRoomButton.Length;
        for (int i = 0; i < joinRoomButton.Length; i++)
        {
            joinRoomButton[i].interactable = (_multiple + i < _myList.Count) ? true : false;
            joinRoomButton[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = (_multiple + i < _myList.Count) ? _myList[_multiple + i].PlayerCount + "/" + _myList[_multiple + i].MaxPlayers : "";
            joinRoomButton[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = (_multiple + i < _myList.Count) ? _myList[_multiple + i].Name.Split("#")[0] : "";
            joinRoomButton[i].transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = (_multiple + i < _myList.Count) ? _myList[_multiple + i].Name.Split("#")[1].Equals("3_1.ClassRoom") ? "집중형 강의실" : "토론형 강의실" : "";
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
                        if (!_myList.Contains(roomList[i])) // 만약 룸리스트에 i번째 항목이 _myList에 없다면
                        {
                            if (!roomList[i].Name.Split("#")[0].Equals(""))
                                _myList.Add(roomList[i]); // _myList에 룸리스트의 i번째 항목 추가
                        }
                        else
                            _myList[_myList.IndexOf(roomList[i])] = roomList[i];
                    }
                    else if (!_myList.IndexOf(roomList[i]).Equals(-1))
                        _myList.RemoveAt(_myList.IndexOf(roomList[i]));
                }
                MyListRenewal();
            }
        );
    }

    public void RoomSet(int i)
    {
        _roomSet = _roomSets[i];
    }

    public void CreateRoom()
    {
        client.RemoveCallbackTarget(this);
        client.Disconnect();
        client = null;
        callback = null;
        RoomChangeManager.Instance.RoomOut((roomName.text.Equals("") ? PhotonNetwork.LocalPlayer.NickName + "님의 방" : roomName.text) + "#" + _roomSet + "#" + UtilClass.GenerateToken(10), roomMax.value + 2, 1);
    }
}