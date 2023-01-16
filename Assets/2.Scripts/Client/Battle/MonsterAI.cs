using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.AI;
using Unity.VisualScripting;
using System.Linq;
using UnityEngine.TextCore.Text;
using TMPro;

public class MonsterAI : MonoBehaviourPunCallbacks
{


    public PhotonView PV;
    public NavMeshAgent agent;
    public Transform[] SpawnPoint;
    public GameObject StunEffect;
    public GameObject ExploseEffect;

    private Vector3 tartget;
    private GameObject playerPos = null;
    private Vector3 escapePos;
    public enum State { IDLE, MOVE, ESCAPE, STUN ,DIE };
    public State state = State.IDLE;

    private int monsterHP=100;

    private
    void OnEnable()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            tartget = SpawnPoint[Random.Range(0, SpawnPoint.Length)].position;
            agent.destination = tartget;
            state = State.MOVE;
            StartCoroutine("CheckState");
            monsterHP = 100;
        }
        //QuestUIInput.OnMonsterDie += this.OnMonsterDie;
    }

    void OnDisable()
    {
        //QuestUIInput.OnMonsterDie -= this.OnMonsterDie;
    }

    IEnumerator CheckState()
    {
        while (state != State.DIE)
        {
            yield return new WaitForSeconds(0.3f);
            float distance = Vector3.Distance(transform.position, tartget);

            if (distance < 5f && state == State.MOVE && state != State.STUN)
            {
                agent.isStopped = false;
                tartget = SpawnPoint[Random.Range(0, SpawnPoint.Length)].position;
                agent.destination = tartget;
            }
        }
        yield return null;
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
                    agent.isStopped = false;
                    escapePos = (transform.position - colliders[i].transform.position).normalized;
                    playerPos = colliders[i].gameObject;
                    agent.speed = 8;
                    agent.destination = transform.position + escapePos * 7.0f;
                    state = State.ESCAPE;
                }
            }
            if (state == State.ESCAPE && state != State.STUN)
            {
                agent.isStopped = false;
                agent.speed = 8;
                escapePos = (transform.position - playerPos.transform.position).normalized;
                agent.destination = transform.position + escapePos * 7.0f;
            }
            if (colliders.Length == 0 && state == State.ESCAPE && state != State.STUN)
            {
                state = State.MOVE;
                agent.speed = 5;
                tartget = SpawnPoint[Random.Range(0, SpawnPoint.Length)].position;
                agent.destination = tartget;
            }
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.CompareTag("fireball") && PV.IsMine)
        {
            if (PV.IsMine)
            {
                Destroy(other.gameObject);
                Vector3 pos = other.GetContact(0).point;
                Quaternion rot = Quaternion.LookRotation(-other.GetContact(0).normal);
                PV.RPC("StunMode", RpcTarget.AllBuffered, 34, pos, rot);
            }
        }
    }

    void Recovery()
    {
        state = State.MOVE;
        monsterHP = 100;
        StunEffect.SetActive(false);
    }

    [PunRPC]
    void StunMode(int a, Vector3 pos, Quaternion rot)
    {
        monsterHP -= a;
        //GameObject explose = Instantiate<GameObject>(ExploseEffect, pos, rot);
        //Destroy(explose,1.0f);
        if (monsterHP < 0)
        {
            state = State.STUN;
            agent.isStopped = true;
            StunEffect.SetActive(true);
            Invoke("Recovery", 5.0f);
        }
    }
}
