using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class LoginPhotonManager : MonoBehaviourPunCallbacks
{
    private string LobbyRoom = "Campus#2.Campus";
    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    #region 서버 연결 => 로비 입장
    public void Connect() => PhotonNetwork.ConnectUsingSettings();

    public override void OnConnectedToMaster() => PhotonNetwork.JoinLobby();

    public override void OnJoinedLobby()
    {
        PhotonNetwork.JoinRoom(LobbyRoom);
    }

    public void Disconnect() => PhotonNetwork.Disconnect();

    public override void OnDisconnected(DisconnectCause cause)
    {

    }

    public override void OnJoinedRoom()
    {
        // 방 입장
        PhotonNetwork.LoadLevel("2.Campus");
        PhotonNetwork.IsMessageQueueRunning = false;
    }


    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        // 방 생성
        PhotonNetwork.CreateRoom(LobbyRoom, new RoomOptions { MaxPlayers = 20 });
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("방 생성에 실패");
    }
    #endregion
}
