using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class GBSetting : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI currPlayer;
    public bool a = true;

    IEnumerator Start()
    {
        var wfs = new WaitForSeconds(2f);
        while (!PhotonNetwork.CurrentRoom.PlayerCount.Equals(4))
        {
            currPlayer.text = PhotonNetwork.CurrentRoom.PlayerCount.ToString();
            yield return wfs;
        }
        yield return wfs;
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            if (PhotonNetwork.PlayerList[i].Equals(PhotonNetwork.LocalPlayer))
            {
                GameObject myCRT = GameObject.Find(PhotonNetwork.LocalPlayer.NickName);
                myCRT.transform.position = i.Equals(0) ? new Vector3(3f, 0f, 0.5f) : i.Equals(1) ? new Vector3(1.15f, 0f, -0.5f) : i.Equals(2) ? new Vector3(-1.15f, 0f, -0.5f) : new Vector3(-3f, 0f, 0.5f);
                myCRT.transform.rotation = i.Equals(0) ? Quaternion.Euler(0, -60f, 0f) : i.Equals(1) ? Quaternion.Euler(0, -20f, 0f) : i.Equals(2) ? Quaternion.Euler(0, 20f, 0f) : Quaternion.Euler(0, 60f, 0f);
            }
        }
        gameObject.SetActive(false);
        yield break;
    }
}