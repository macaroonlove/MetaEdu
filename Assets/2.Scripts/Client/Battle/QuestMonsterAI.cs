using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestMonsterAI : MonoBehaviour
{
    QuizManager quizManager;
    private void OnEnable()
    {
        QuestUIInput.OnMonsterDie += this.OnMonsterDie;
        quizManager.FadeEffect.SetActive(true);
        quizManager.QuestionPanel.SetActive(true);
        quizManager.ChoiceAnswerPanel.SetActive(true);
        Invoke("Fade", 3.0f);
    }

    private void OnDisable()
    {
        QuestUIInput.OnMonsterDie -= this.OnMonsterDie;
        this.gameObject.GetComponentInParent<PlayerBattle>().Battle_Vcam.Priority = 5;
    }
    private void Awake()
    { 
        quizManager = GameObject.Find("QuizManager").GetComponent<QuizManager>();
    }
    void OnMonsterDie()
    {
        this.gameObject.GetComponentInParent<PhotonView>().RPC("SendMonsterDie", RpcTarget.AllBuffered);
        this.gameObject.SetActive(false);
    }

    void Fade()
    {
        quizManager.FadeEffect.SetActive(false);
    }
}
