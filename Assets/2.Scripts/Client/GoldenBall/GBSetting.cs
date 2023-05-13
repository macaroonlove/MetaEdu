using System.Collections;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class GBSetting : MonoBehaviourPunCallbacks
{
    private PlayerInputPress _inputPress;
    private Animator _phoneAnim;
    private bool _phoneState = false;

    public TextMeshProUGUI currPlayer;
    public bool a = true;
    public GameObject Time;
    public GameObject goldenBallManager;

    IEnumerator Start()
    {
        var wfs = new WaitForSeconds(2f);
        while (!PhotonNetwork.CurrentRoom.PlayerCount.Equals(1))
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
        Time.SetActive(true);
        goldenBallManager.SetActive(true);
        GameObject player = GameObject.FindWithTag("Player");
        player.GetComponent<Score>().enabled = false;
        player.GetComponent<Score>().enabled = true;
        player.TryGetComponent(out _inputPress);
        GameObject.Find("Phone").TryGetComponent(out _phoneAnim);
        transform.GetChild(0).gameObject.SetActive(false);
        wfs = new WaitForSeconds(0.1f);
        while (true)
        {
            if (_inputPress.phone)
            {
                _phoneState = !_phoneState;
                _phoneAnim.SetBool("Phone", _phoneState);
                _phoneAnim.SetBool("Open", false);
                _inputPress.phone = false;
            }
            yield return wfs;
        }
    }

    public void BackLobby()
    {
        RoomChangeManager.Instance.RoomOut("Campus#2.Campus", 20, 0);
    }
}