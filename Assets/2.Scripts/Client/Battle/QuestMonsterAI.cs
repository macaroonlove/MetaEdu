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
    private int _hashStun;
    private Rigidbody rigid;
    private void OnEnable()
    {
        PlayerBattle.OnMonsterDieEvent += this.OnMonsterDie;
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
        _anim.SetBool(_hashStun, true);
    }

    private void OnDisable()
    {
        PlayerBattle.OnMonsterDieEvent -= this.OnMonsterDie;
        battle_vcam.Priority = 5;
        _playerController.grammaticalPersonState = true;
        GetComponentInParent<PlayerBattle>().Renderer();
        _anim.SetBool(_hashStun, false);

        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;
        rigid.ResetInertiaTensor();
        rigid.ResetCenterOfMass();
    }
    private void Awake()
    { 
        quizManager = GameObject.Find("QuizManager").GetComponent<QuizManager>();
        _playerBattle = GetComponentInParent<PlayerBattle>();
        battle_vcam = _playerBattle.Battle_Vcam;
        PV = GetComponentInParent<PhotonView>();
        _playerController = GetComponentInParent<PlayerController>();
        rigid = GetComponent<Rigidbody>();
        _anim = GetComponent<Animator>();
        _hashStun = Animator.StringToHash("Stun");
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
        _anim.SetBool(_hashStun, false);
        rigid.AddForce(Vector3.up * 400f + Vector3.back * 300f);
    }
}
