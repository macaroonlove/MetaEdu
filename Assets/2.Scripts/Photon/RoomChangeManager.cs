using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class RoomChangeManager : MonoBehaviour
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
        if (isJoin)
        {
            loadingBar.fillAmount = 0.1f;
            yield return new WaitForSeconds(0.5f);
        }
        PhotonNetwork.JoinOrCreateRoom(roomName, new RoomOptions { MaxPlayers = (byte)maxPlayer }, null);
        loadingBar.fillAmount = 0.2f;
        yield return new WaitForSeconds(1f);
        PhotonNetwork.LoadLevel(roomName.Split("#")[1]);
        PhotonNetwork.IsMessageQueueRunning = false;
        while (PhotonNetwork.LevelLoadingProgress < 1)
        {
            loadingBar.fillAmount = 0.2f + PhotonNetwork.LevelLoadingProgress * 0.4f;
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
            AsyncOperation op = SceneManager.LoadSceneAsync("Petty", LoadSceneMode.Additive);
            op.allowSceneActivation = false;

            float timer = 0f;
            while (true)
            {
                yield return null;
                if(op.progress < 0.85f)
                {
                    loadingBar.fillAmount = 0.6f + (op.progress * 0.3f);
                }
                else
                {
                    op.allowSceneActivation = true;
                    timer += Time.unscaledDeltaTime;
                    loadingBar.fillAmount = Mathf.Lerp(0.9f, 1f, timer);
                    if(loadingBar.fillAmount >= 1f)
                    {
                        yield return new WaitForSeconds(0.5f);
                        transform.GetChild(0).gameObject.SetActive(false);
                        PhotonNetwork.IsMessageQueueRunning = true;
                        yield break;
                    }
                }
            }
        }
    }


}
