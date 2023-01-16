using Mono.CompilerServices.SymbolWriter;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using System;
using UnityEngine.InputSystem;
using Unity.Mathematics;

public class QuizManager : MonoBehaviourPunCallbacks
{

    public List<Transform> points = new List<Transform>();
    public List<GameObject> MonsterPool = new List<GameObject>();

    private GameObject spawnPoint;
    private GameObject Monsters;
    public GameObject QuestionPanel;
    public GameObject ChoiceAnswerPanel;
    public GameObject FadeEffect;
    public GameObject battleText;
    public PhotonView PV;
    

    void Start()
    {
        spawnPoint = GameObject.Find("SpawnPointsGroup");
        Monsters = GameObject.Find("MonsterGroup");

        for (int i = 0; i < spawnPoint.transform.childCount; i++)
        {
            points.Add(spawnPoint.transform.GetChild(i));
        }

        for (int i = 0; i < Monsters.transform.childCount; i++)
        {
            MonsterPool.Add(Monsters.transform.GetChild(i).gameObject);
        }

        //ДыБо UI
        QuestionPanel = GameObject.Find("QuestionPanel");
        ChoiceAnswerPanel = GameObject.Find("ChoiceAnswerPanel");
        FadeEffect = GameObject.Find("FadeEffect");
        battleText = GameObject.Find("BattleText");

        QuestionPanel.SetActive(false);
        ChoiceAnswerPanel.SetActive(false);
        FadeEffect.SetActive(false);
        battleText.SetActive(false);

        if (PhotonNetwork.IsMasterClient)
        {
            InvokeRepeating("CreateMonster", 2.0f, 3.0f);
        }
    }

    public void CreateMonster()
    {
        PV.RPC("SendCreateMonster", RpcTarget.AllBuffered);   
    }
    [PunRPC]
    void SendCreateMonster()
    {
        int idx = UnityEngine.Random.Range(0, points.Count);
        GameObject _monster = GetMonsterPool();
        _monster?.transform.SetPositionAndRotation(points[idx].position + new Vector3(0, 2.0f, 0), points[idx].rotation);
        _monster?.SetActive(true);
    }

    public GameObject GetMonsterPool()
    { 
        foreach(var _monster in MonsterPool)
        {
            if(_monster.activeSelf == false)
            {
                return _monster;
            }
        }
        return null;
    }

}
