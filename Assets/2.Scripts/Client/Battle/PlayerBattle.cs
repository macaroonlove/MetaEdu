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
    public List<GameObject> FireballPool = new List<GameObject>();
    public PlayerController controller;
    public GameObject QuestMonster;
    public CinemachineVirtualCamera Battle_Vcam;
    public PhotonView PV;

    [Header("파이어볼 이펙트")]
    public GameObject FirePos;
    public bool AttackState = true;
    public GameObject Effect;

    private QuizManager quizManager;
    void Awake()
    {
        PV = gameObject.GetComponent<PhotonView>();
        CreateFireballs(5);
        if (!SceneManager.GetActiveScene().name.Contains("Battle"))
        {
            this.enabled = false;
        }
    }

    new void OnEnable()
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
        Battle_Vcam.Priority = 20;
        QuestMonster.SetActive(true);
        AttackState = false;
        quizManager.battleText.SetActive(false);

        Transform[] allChildren = GetComponentsInChildren<Transform>();
        for (int i = 0; i < allChildren.Length; i++)
        {
            allChildren[i].gameObject.layer = 0;
        }

        Camera.main.cullingMask = ~(1 << 9) & ~(1 << 8);

        Cursor.lockState = CursorLockMode.None;

        PV.RPC("Battle", RpcTarget.AllBuffered, MonsterID);
    }

    public void OnAttack()
    {
        PV.RPC("Attack", RpcTarget.AllViaServer);
    }

    [PunRPC]
    public void Attack()
    {
        GameObject FireBall = GetFireball();
        FireBall.transform.SetPositionAndRotation(FirePos.transform.position, FirePos.transform.rotation);
        FireBall.GetComponent<Rigidbody>().AddForce(transform.forward * 1000);
        StartCoroutine("OnRelease", FireBall);
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

    void CreateFireballs(int FireballCount)
    {
        for (int i = 0; i < FireballCount; i++)
        {
            GameObject fireball = Instantiate(Effect) as GameObject;
            fireball.SetActive(false);
            FireballPool.Add(fireball);
        }
    }

    public GameObject GetFireball()
    {
        GameObject reqFireball = null;

        for (int i = 0; i < FireballPool.Count; i++)
        {
            if (FireballPool[i].activeSelf == false)
            {
                reqFireball = FireballPool[i];
                break;
            }
        }

        if (reqFireball == null)
        {
            GameObject newFireball = Instantiate<GameObject>(Effect) as GameObject;
            reqFireball = newFireball;
        }

        reqFireball.SetActive(true);

        return reqFireball;
    }

    private IEnumerator OnRelease(GameObject fireball)
    {
        var wfs = new WaitForSeconds(1f);
        yield return wfs;
        fireball.SetActive(false);
        yield break;
    }
}