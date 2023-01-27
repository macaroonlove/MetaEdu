using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

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

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public void RoomOut(string roomName, int maxPlayer)
    {
        PhotonNetwork.LeaveRoom();
        Singleton.Inst.isPatty = false;

        StartCoroutine(LoadRoom(roomName, true, maxPlayer));
    }

    public void RoomChange(string roomName) => StartCoroutine(LoadRoom(roomName, false, 20));

    IEnumerator LoadRoom(string roomName, bool isJoin, int maxPlayer)
    {
        transform.GetChild(0).gameObject.SetActive(true);
        loadingBar.fillAmount = 0.1f;
        while (true)
        {
            if (PhotonNetwork.InLobby)
            {
                Debug.Log(roomName);
                PhotonNetwork.JoinOrCreateRoom(roomName.Contains("null") ? null : roomName, new RoomOptions { MaxPlayers = (byte)maxPlayer }, null);

                yield break;
            }
            yield return null;
        }
    }

    public override void OnJoinedRoom()
    {
        loadingBar.fillAmount = 0.2f;
        StartCoroutine(LevelRoom());
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
                    transform.GetChild(0).gameObject.SetActive(false);
                    break;
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
