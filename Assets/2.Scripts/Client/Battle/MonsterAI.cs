using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.AI;
using TMPro;

public class MonsterAI : MonoBehaviourPunCallbacks
{
    public enum State { IDLE, MOVE, ESCAPE, STUN, DIE };
    public State state = State.IDLE;

    public PhotonView PV;
    public NavMeshAgent agent;
    public Transform[] SpawnPoint;
    public GameObject StunEffect;
    public GameObject ExploseEffect;

    public Animator anim;
    public int _hashEscape;
    public int _hashStun;

    private Vector3 tartget;
    private GameObject playerPos = null;
    private Vector3 escapePos;

    private int monsterHP=100;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();   
        _hashEscape = Animator.StringToHash("Escape");
        _hashStun = Animator.StringToHash("Stun");
        SpawnPoint = GameObject.Find("SpawnPointsGroup").GetComponentsInChildren<Transform>();
    }
    new void OnEnable()
    {
        tartget = SpawnPoint[Random.Range(1, SpawnPoint.Length)].position;
        agent.destination = tartget;
        state = State.MOVE;
        StartCoroutine("CheckState");
        monsterHP = 100;
        agent.isStopped = false;
        StunEffect.SetActive(false);
        agent.baseOffset = 0.7f;
    }
    IEnumerator CheckState()
    {
        var wfs = new WaitForSeconds(1f);
        if(PhotonNetwork.IsMasterClient)
        {
            while (state != State.DIE)
            {
                yield return wfs;
                float distance = Vector3.Distance(transform.position, tartget);

                if (distance < 5f && state == State.MOVE)
                {
                    tartget = SpawnPoint[Random.Range(1, SpawnPoint.Length)].position;
                    agent.destination = tartget;
                    anim.SetBool(_hashEscape, false);
                }
            }
            yield return null;
        }
        yield break;
    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, 7f, 1 << 7 | 1 << 8);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (state != State.ESCAPE && state != State.STUN)
                {
                    escapePos = (transform.position - colliders[i].transform.position).normalized;
                    playerPos = colliders[i].gameObject;
                    agent.speed = 8;
                    agent.destination = transform.position + escapePos * 7.0f;
                    state = State.ESCAPE;
                    anim.SetBool(_hashEscape, true);
                }
            }
            if (state == State.ESCAPE)
            {
                agent.isStopped = false;
                agent.speed = 8;
                escapePos = (transform.position - playerPos.transform.position).normalized;
                agent.destination = transform.position + escapePos * 7.0f;
                anim.SetBool(_hashEscape, true);
            }
            if (colliders.Length == 0 && state == State.ESCAPE)
            {
                state = State.MOVE;
                agent.speed = 5;
                tartget = SpawnPoint[Random.Range(1, SpawnPoint.Length)].position;
                agent.destination = tartget;
                anim.SetBool(_hashEscape, false);
            }
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.CompareTag("fireball") && PV.IsMine)
        {
            SoundManager.Instance.ExplosionSound();
            other.gameObject.SetActive(false);
            Vector3 pos = other.GetContact(0).point;
            Quaternion rot = Quaternion.LookRotation(-other.GetContact(0).normal);
            PV.RPC("StunMode", RpcTarget.AllBuffered, 34, pos);
        }
    }

    [PunRPC]
    void Recovery()
    {
        if(state == State.STUN)
        {
            agent.isStopped = false;
            agent.baseOffset = 0.7f;
            state = State.MOVE;
            monsterHP = 100;
            anim.SetBool(_hashStun, false);
            StunEffect.SetActive(false);
        }
    }

    [PunRPC]
    void StunMode(int a, Vector3 pos)
    {
        monsterHP -= a;
        GameObject exp = Instantiate(ExploseEffect, pos, Quaternion.identity);
        Destroy(exp, 1);
        if (monsterHP < 0)
        {
            state = State.STUN;
            agent.isStopped = true;
            StunEffect.SetActive(true);
            anim.SetBool(_hashEscape, false);
            anim.SetBool(_hashStun,true);
            agent.baseOffset = 0;
            Invoke("Recovery", 5.0f);
        }
    }
}
