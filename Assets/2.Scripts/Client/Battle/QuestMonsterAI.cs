using Cinemachine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class QuestMonsterAI : MonoBehaviour
{
    private PhotonView PV;
    private CinemachineVirtualCamera battle_vcam;
    private QuizManager quizManager;
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
    }
    private void Awake()
    { 
        quizManager = GameObject.Find("QuizManager").GetComponent<QuizManager>();
        battle_vcam = gameObject.GetComponentInParent<PlayerBattle>().Battle_Vcam;
        PV = gameObject.GetComponentInParent<PhotonView>();
    }
    void OnMonsterDie()
    {
        PV.RPC("SendMonsterDie", RpcTarget.AllBuffered);
        this.gameObject.SetActive(false);
    }
}
