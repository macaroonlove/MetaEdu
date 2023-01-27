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
        SoundManager.Instance.Login_BGM();
        SoundManager.Instance.EffectExample();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    #region ���� ���� => �κ� ����
    public void Connect() => PhotonNetwork.ConnectUsingSettings();

    public override void OnJoinedLobby()
    {
        RoomChangeManager.Instance.RoomChange(LobbyRoom);
    }

    public void Disconnect() => PhotonNetwork.Disconnect();

    //public override void OnJoinedRoom()
    //{
    //    RoomChangeManager.Instance.RoomChange("2.Campus");
    //}

    //public override void OnJoinRoomFailed(short returnCode, string message)
    //{
    //    // �� ����
    //    PhotonNetwork.CreateRoom(LobbyRoom, new RoomOptions { MaxPlayers = 20 });
    //}

    //public override void OnCreateRoomFailed(short returnCode, string message)
    //{
    //    Debug.Log("�� ������ ����");
    //}
    #endregion
}
