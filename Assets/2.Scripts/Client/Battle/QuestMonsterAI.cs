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
    private PlayerBattle _playerBattle;
    private Animator _anim;
    private int _hashEsc;
    private void OnEnable()
    {
        PlayerBattle.OnMonsterDieEvent += this.OnMonsterDie;
        quizManager.CreateQuiz();
        quizManager.QuestionPanel.SetActive(true);
        if (quizManager.questionList.answer[quizManager.currQuiz].Split("��")[0] == "0")
        {
            quizManager.AnswerPanel.SetActive(true);
            quizManager.AnswerPanel.transform.GetChild(0).gameObject.SetActive(true);
            switch(quizManager.choiceAnswerList.Count)
            {
                case 1:
                    quizManager.choiceAnswers[3].transform.parent.gameObject.SetActive(false);
                    quizManager.choiceAnswers[2].transform.parent.gameObject.SetActive(false);
                    quizManager.choiceAnswers[1].transform.parent.gameObject.SetActive(false);
                    break;
                case 2:
                    quizManager.choiceAnswers[3].transform.parent.gameObject.SetActive(false);
                    quizManager.choiceAnswers[2].transform.parent.gameObject.SetActive(false);
                    break;
                case 3:
                    quizManager.choiceAnswers[3].transform.parent.gameObject.SetActive(false);
                    break;
                default:
                    break;
            }
            quizManager.choiceAnswerList.Clear();
        }
        else if (quizManager.questionList.answer[quizManager.currQuiz].Split("��")[0] == "1")
        {
            quizManager.AnswerPanel.SetActive(true);
            quizManager.AnswerPanel.transform.GetChild(1).gameObject.SetActive(true);
        }
        else if (quizManager.questionList.answer[quizManager.currQuiz].Split("��")[0] == "2")
        {
            quizManager.AnswerPanel.SetActive(true);
            quizManager.AnswerPanel.transform.GetChild(2).gameObject.SetActive(true);
        }
    }

    private void OnDisable()
    {
        PlayerBattle.OnMonsterDieEvent -= this.OnMonsterDie;
        battle_vcam.Priority = 5;
        _playerController.grammaticalPersonState = true;
        GetComponentInParent<PlayerBattle>().Renderer();
        Camera.main.cullingMask = -1;
    }
    private void Awake()
    { 
        quizManager = GameObject.Find("QuizManager").GetComponent<QuizManager>();
        _playerBattle = GetComponentInParent<PlayerBattle>();
        battle_vcam = _playerBattle.Battle_Vcam;
        PV = GetComponentInParent<PhotonView>();
        _playerController = GetComponentInParent<PlayerController>();
        TryGetComponent(out _anim);
        _hashEsc = Animator.StringToHash("Escape");
    }
    void OnMonsterDie()
    {
        _playerBattle.AttackState = true;
        PV.RPC("SendMonsterDie", RpcTarget.OthersBuffered);
        this.gameObject.SetActive(false);
        _playerBattle.anim.SetBool(_playerBattle.animBattle, false);
    }

    public void RunMonster()
    {
        _anim.SetTrigger(_hashEsc);
    }
}
