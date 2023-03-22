using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;

public class QuestionSharing : MonoBehaviour
{
    public PhotonView PV;
    public TextMeshProUGUI[] receivePlayer;
    private List<int> _playerNum = new List<int>();

    #region 문제 공유
    public void SharePanel()
    {
        for(int i = 0; i < receivePlayer.Length; i++)
        {
            if(i < PhotonNetwork.PlayerListOthers.Length)
            {
                receivePlayer[i].transform.parent.gameObject.SetActive(true);
                receivePlayer[i].text = PhotonNetwork.PlayerListOthers[i].NickName;
            }
            else
            {
                receivePlayer[i].transform.parent.gameObject.SetActive(false);
            }
        }
    }

    public void PlayerToggle(int i)
    {
        if (_playerNum.Contains(i))
        {
            _playerNum.Remove(i);
        }
        else
        {
            _playerNum.Add(i);
        }
    }

    public void ShareQuiz()
    {
        for(int i = 0; i < _playerNum.Count; i++)
        {
            PV.RPC(nameof(ReceiveQuiz), PhotonNetwork.PlayerListOthers[_playerNum[i]], Singleton.Inst.question[Singleton.Inst.currSelect]);
        }
        _playerNum.Clear();
    }

    [PunRPC]
    private void ReceiveQuiz(string quiz)
    {
        Singleton.Inst.questions += "▦" + quiz;
        var request = new UpdateUserDataRequest() { Data = new Dictionary<string, string>() { { "Question", Singleton.Inst.questions } } };
        PlayFabClientAPI.UpdateUserData(request, (result) => { Singleton.Inst.QuestionInit(); }, (Error) => { Debug.Log("공유받기에 실패하셨습니다."); });
    }
    #endregion
}