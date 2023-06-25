using ExitGames.Client.Photon;
using Photon.Pun;

public class LoginPhotonManager : MonoBehaviourPunCallbacks
{
    private string LobbyRoom = "Campus#2.Campus";
    void Start()
    {
        SoundManager.Instance.Login_BGM();
        SoundManager.Instance.EffectExample();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    #region 서버 연결 => 로비 입장
    public void Connect() => PhotonNetwork.ConnectUsingSettings();

    public override void OnJoinedLobby()
    {
        RoomChangeManager.Instance.RoomChange(LobbyRoom);
    }

    public void Disconnect() => PhotonNetwork.Disconnect();
    #endregion
}
