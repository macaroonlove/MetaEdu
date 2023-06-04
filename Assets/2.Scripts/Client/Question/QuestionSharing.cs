using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class QuestionSharing : MonoBehaviour
{
    public PhotonView PV;
    public TextMeshProUGUI[] receivePlayer;
    public GameObject[] sharedQuiz;
    public TextMeshProUGUI[] sharedQuizText;
    private List<int> _playerNum = new List<int>();
    public List<QuestionData> _sharedQuiz = new();
    public int _currQuiz = 0;

    void Start()
    {
        QuestionManager.Inst.QuestionInit();
    }

    #region 문제 공유 하기
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
            PV.RPC(nameof(ReceiveQuiz), PhotonNetwork.PlayerListOthers[_playerNum[i]], QuestionManager.Inst.questionDatas[QuestionManager.Inst.selectQuestion]);
        }
        //_playerNum.Clear();
    }
    #endregion
    #region 문제 공유 받기
    [PunRPC]
    private void ReceiveQuiz(QuestionData quiz, PhotonMessageInfo info)
    {
        _sharedQuiz.Add(quiz);
        for(int i = 0; i < _sharedQuiz.Count; i++)
        {
            sharedQuizText[i].text = "<color=#00FFFF><size=\"17\">" + info.Sender.NickName + "</size></color>\n" + _sharedQuiz[i].title;
            sharedQuiz[i].SetActive(true);
        }
    }

    void RenewalQuiz()
    {
        for(int i = _currQuiz; i < sharedQuizText.Length - 1; i++)
        {
            sharedQuizText[i].text = sharedQuizText[i + 1].text;
        }
        sharedQuiz[_sharedQuiz.Count].SetActive(false);
    }

    public void CurrentQuiz(int i) => _currQuiz = i;

    public void RemoveQuiz()
    {
        _sharedQuiz.RemoveAt(_currQuiz);
        RenewalQuiz();
    }

    public void RemoveAllQuiz()
    {
        if (_sharedQuiz.Count == 0)
            return;

        for(int i = 0; i < sharedQuiz.Length; i++)
            sharedQuiz[i].SetActive(false);
        _sharedQuiz.Clear();
    }

    public async void SaveQuiz()
    {
        QuestionManager.Inst.questionDatas.Add(_sharedQuiz[_currQuiz]);

        if (await QuestionManager.Inst.SetUserData())
        {
            QuestionManager.Inst.QuestionInit();
            _sharedQuiz.RemoveAt(_currQuiz);
            RenewalQuiz();
        }
        else
        {
            Debug.Log("공유받기에 실패하셨습니다.");
        }
    }
    #endregion
}