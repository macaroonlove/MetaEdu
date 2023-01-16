using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Net.NetworkInformation;
using UnityEngine.AI;
using Cinemachine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerBattle : MonoBehaviourPunCallbacks
{
    public PlayerController controller;
    public GameObject QuestMonster;
    private PhotonView PV;
    public CinemachineVirtualCamera Battle_Vcam;
    QuizManager quizManager;

    [Header("파이어볼 이펙트")]
    public GameObject[] Effect;
    public GameObject FirePos;
    public bool AttackState =true;
    // Start is called before the first frame update

    void Awake()
    {
        if (!SceneManager.GetActiveScene().name.Contains("Battle"))
        {
            this.enabled = false;
            //gameObject.GetComponent<PlayerBattle>().enabled = false;
        }
    }

    void Start()
    {
        PV = GetComponent<PhotonView>();
        quizManager = GameObject.Find("QuizManager").GetComponent<QuizManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (PV.IsMine)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, 2.5f, 1 << 9);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].gameObject.GetComponent<MonsterAI>().state == MonsterAI.State.STUN)
                {
                    quizManager.battleText.SetActive(true);
                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        OnBattle(colliders[i].GetComponent<PhotonView>().ViewID);
                    }
                }
            }
            if (colliders.Length == 0)
            {
                quizManager.battleText.SetActive(false);
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (AttackState)
                {
                    OnAttack();
                }
            }
        }
    }

    void OnBattle(int MonsterID)
    {
        //other.GetComponent<NavMeshAgent>().isStopped = true;
        //카메라 변경
        Battle_Vcam.Priority = 20;
        QuestMonster.SetActive(true);
        AttackState = false;
        quizManager.battleText.SetActive(false);
        
        Transform[] allChildren = GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            child.gameObject.layer = 0;
        }

        Camera.main.cullingMask = ~(1 << 10) & ~(1 << 8);

        Cursor.lockState = CursorLockMode.None;

        PV.RPC("Battle", RpcTarget.AllBuffered, MonsterID);
    }

    public void OnAttack()
    {
        if (PV.IsMine)
        {
            PV.RPC("Attack", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    public void Attack()
    {
        GameObject FireBall = Instantiate(Effect[Random.Range(0, 5)], FirePos.transform.position, Quaternion.Euler(-90.0f, 0, 0));

        FireBall.GetComponent<Rigidbody>().AddForce(Camera.main.transform.localRotation * Vector3.forward * 1200);
        Destroy(FireBall, 1);
    }

    [PunRPC]
    void SendMonsterDie()
    {
        if (PV.IsMine)
        {
            gameObject.layer = 7;
        }
        else if (PV.IsMine == false)
        {
            gameObject.layer = 8;
        }
        gameObject.tag = "Player";

        AttackState = true;
    }

    [PunRPC]
    void Battle(int MonsterID)
    {
        PhotonNetwork.GetPhotonView(MonsterID).gameObject.SetActive(false);
        gameObject.tag = "Untagged";
        gameObject.layer = 0;
    }

}