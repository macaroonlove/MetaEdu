using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class RoomChangeManager : MonoBehaviourPunCallbacks
{
    /* Singleton */
    private static RoomChangeManager _instance;

    public static RoomChangeManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new RoomChangeManager();
            }
            return _instance;
        }
    }

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this);
        }
    }

    public Image loadingBar;
    private int _type = 0;

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public void RoomOut(string roomName, int maxPlayer, int type)
    {
        PhotonNetwork.LeaveRoom();
        Singleton.Inst.isPatty = false;

        StartCoroutine(LoadRoom(roomName, true, maxPlayer, type));
    }

    public void RoomChange(string roomName) => StartCoroutine(LoadRoom(roomName, false, 20, 0));

    IEnumerator LoadRoom(string roomName, bool isJoin, int maxPlayer, int type)
    {
        transform.GetChild(0).gameObject.SetActive(true);
        loadingBar.fillAmount = 0.1f;
        while (true)
        {
            if (PhotonNetwork.InLobby)
            {                
                if (type.Equals(1)) // ClassRoom
                {
                    PhotonNetwork.JoinOrCreateRoom(roomName, new RoomOptions { MaxPlayers = (byte)maxPlayer }, null);
                }
                else // Others
                {
                    _type = type;
                    Invoke(nameof(RandomRoom), 2f * Time.deltaTime);
                }

                yield break;
            }
            yield return null;
        }
    }

    private void RandomRoom()
    {
        Hashtable expectedCustomRoomProperties = new Hashtable() { { "type", _type } };
        PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, (byte)(_type.Equals(3) ? 4 : 20));
    } 

    public override void OnJoinedRoom()
    {
        loadingBar.fillAmount = 0.2f;
        StartCoroutine(LevelRoom());
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log(message);
        CreateRoomWithConditions();
    }

    private void CreateRoomWithConditions()
    {
        string roomName = (_type.Equals(0) ? "#2.Campus" : _type.Equals(2) ? "#4.Battle" : "#5.Goldenball") + "#" + UtilClass.GenerateToken(10);
        Hashtable customRoomProperties = new Hashtable() { { "type", _type } };

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.CustomRoomProperties = customRoomProperties;
        roomOptions.MaxPlayers = (byte)(_type.Equals(3) ? 4 : 20);
        roomOptions.CustomRoomPropertiesForLobby = new string[] { "type" };
        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

    IEnumerator LevelRoom()
    {
        PhotonNetwork.LoadLevel(PhotonNetwork.CurrentRoom.Name.Split("#")[1]);
        while (PhotonNetwork.LevelLoadingProgress < 1)
        {
            loadingBar.fillAmount = 0.2f + PhotonNetwork.LevelLoadingProgress * 0.7f;
            if (PhotonNetwork.LevelLoadingProgress == 0.9f)
            {
                StartCoroutine(LoadPetty());
            }
            yield return null;
        }
    }

    IEnumerator LoadPetty()
    {
        if (!Singleton.Inst.isPatty)
        {
            Singleton.Inst.isPatty = true;
            SceneManager.LoadScene("Petty", LoadSceneMode.Additive);
            float timer = 0f;
            while (true)
            {
                yield return null;
                timer += Time.unscaledDeltaTime;
                loadingBar.fillAmount = Mathf.Lerp(0.9f, 1f, timer);
                if (loadingBar.fillAmount == 1f)
                {
                    float findRoom = 0;
                    while (findRoom < 2f)
                    {
                        yield return null;
                        findRoom += Time.deltaTime;
                    }
                    transform.GetChild(0).gameObject.SetActive(false);
                    yield break;
                }
            }
        }

        //if (!Singleton.Inst.isPatty)
        //{
        //    Singleton.Inst.isPatty = true;
        //    SceneManager.LoadScene("Petty", LoadSceneMode.Additive);
        //    transform.GetChild(0).gameObject.SetActive(false);
        //    listener.enabled = false;
        //    yield return null;
        //    AsyncOperation op = SceneManager.LoadSceneAsync("Petty", LoadSceneMode.Additive);
        //    op.allowSceneActivation = true;

        //    float timer = 0f;
        //    while (true)
        //    {
        //        yield return null;
        //        if (op.progress < 0.85f)
        //        {
        //            loadingBar.fillAmount = 0.6f + (op.progress * 0.3f);
        //        }
        //        else
        //        {
        //            //op.allowSceneActivation = true;
        //            listener.enabled = false;
        //            timer += Time.unscaledDeltaTime;
        //            loadingBar.fillAmount = Mathf.Lerp(0.9f, 1f, timer);
        //            if (loadingBar.fillAmount >= 1f)
        //            {
        //                yield return new WaitForSeconds(1f);
        //                transform.GetChild(0).gameObject.SetActive(false);
        //                yield break;
        //            }
        //        }
        //    }
        //}
    }
}
