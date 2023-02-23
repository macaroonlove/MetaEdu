using Cinemachine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestMonsterAI : MonoBehaviour
{
    private PhotonView PV;
    private CinemachineVirtualCamera battle_vcam;
    private QuizManager quizManager;
    private PlayerController _playerController;
    private void OnEnable()
    {
        AnswerInput.OnMonsterDie += this.OnMonsterDie;
        quizManager.CreateQuiz();
        quizManager.QuestionPanel.SetActive(true);
        if (quizManager.Question[quizManager.currQuiz].Split("¢È")[0] == "0")
        {
            quizManager.AnswerPanel.SetActive(true);
            quizManager.AnswerPanel.transform.GetChild(0).gameObject.SetActive(true);
        }
        else if (quizManager.Question[quizManager.currQuiz].Split("¢È")[0] == "1")
        {
            quizManager.AnswerPanel.SetActive(true);
            quizManager.AnswerPanel.transform.GetChild(1).gameObject.SetActive(true);
        }
        else if (quizManager.Question[quizManager.currQuiz].Split("¢È")[0] == "2")
        {
            quizManager.AnswerPanel.SetActive(true);
            quizManager.AnswerPanel.transform.GetChild(2).gameObject.SetActive(true);
        }
    }

    private void OnDisable()
    {
        AnswerInput.OnMonsterDie -= this.OnMonsterDie;
        battle_vcam.Priority = 5;
        _playerController.grammaticalPersonState = true;
        GetComponentInParent<PlayerBattle>().Renderer();
    }
    private void Awake()
    { 
        quizManager = GameObject.Find("QuizManager").GetComponent<QuizManager>();
        battle_vcam = GetComponentInParent<PlayerBattle>().Battle_Vcam;
        PV = GetComponentInParent<PhotonView>();
        _playerController = GetComponentInParent<PlayerController>();
    }
    void OnMonsterDie()
    {
        gameObject.GetComponentInParent<PlayerBattle>().AttackState = true;
        PV.RPC("SendMonsterDie", RpcTarget.OthersBuffered);
        this.gameObject.SetActive(false);
    }
}
