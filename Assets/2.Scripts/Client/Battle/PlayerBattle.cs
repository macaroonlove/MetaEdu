using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Cinemachine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PlayerBattle : MonoBehaviourPunCallbacks
{
    public delegate void MonsterDie();
    public static event MonsterDie OnMonsterDieEvent;

    public List<GameObject> FireballPool = new List<GameObject>();
    public PlayerController controller;
    public GameObject QuestMonster;
    public CinemachineVirtualCamera Battle_Vcam;
    public PhotonView PV;

    [Header("이펙트")]
    public GameObject FirePos;
    public bool AttackState =true;
    public GameObject effect;
    public GameObject smoke;
    public GameObject fail;
    public GameObject finish;

    [Header("애니메이션")]
    public bool hasAnim;
    public int animBattle;
    public int animAttack;
    public int animReact;
    public int animFinish;
    public int animGoldenballReact;

    public Animator anim;
    private QuizManager quizManager;
    private PlayerController _playerController;
    private SkinnedMeshRenderer[] _skin;
    private GameObject[] _otherPlayer;
    private IngamePhotonManager _photonManager;
    void Awake()
    {
        _playerController = GetComponent<PlayerController>();
        PV = GetComponent<PhotonView>();
        _photonManager = GameObject.Find("PhotonManager").GetComponent<IngamePhotonManager>();


        hasAnim = TryGetComponent(out anim);
        animBattle = Animator.StringToHash("Battle");
        animAttack = Animator.StringToHash("Attack");
        animFinish = Animator.StringToHash("Finish");
        animReact = Animator.StringToHash("React");
        animGoldenballReact = Animator.StringToHash("GoldenballReact");

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
        anim.SetLayerWeight(1, 1f);
    }

    new void OnDisable()
    {
        anim.SetLayerWeight(1, 0f);
    }
    // Update is called once per frame
    void Update()
    {
        if (PV.IsMine)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, 2.6f, 1 << 9);
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
                if (!EventSystem.current.IsPointerOverGameObject() && AttackState)
                {
                    anim.SetTrigger(animAttack);
                    _photonManager.AddEx();
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
        _playerController.grammaticalPersonState = false;

        Camera.main.cullingMask = ~(1 << 8) & ~(1 << 9) & ~(1 << 13);
        Cursor.lockState = CursorLockMode.None;

        _otherPlayer = GameObject.FindGameObjectsWithTag("OtherPlayer");

        if(_otherPlayer!= null)
        {
            for (int i = 0; i < _otherPlayer.Length; i++)
            {
                _skin = _otherPlayer[i].GetComponentsInChildren<SkinnedMeshRenderer>();
                for (int j = 0; j < _skin.Length; j++)
                {
                    _skin[j].enabled = false;
                }
            }
        }

        anim.SetBool(animBattle, true);

        PV.RPC("Battle", RpcTarget.All, MonsterID);
        PV.RPC("SendBattleEffect", RpcTarget.OthersBuffered);
    }

    public void OnAttack()
    {
        if(PV.IsMine)
            PV.RPC("Attack", RpcTarget.All);
    }

    public void Renderer()
    {

        for (int i = 0; i < _otherPlayer.Length; i++)
        {
            for (int j = 0; j < _skin.Length; j++)
            {
                _skin[j].enabled = true;
            }
        }
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
        smoke.SetActive(false);
    }

    [PunRPC]
    void SendBattleEffect()
    {
        smoke.SetActive(true);
    }

    [PunRPC]
    void Battle(int MonsterID)
    {
        PhotonNetwork.GetPhotonView(MonsterID).gameObject.SetActive(false);
    }

    void CreateFireballs(int FireballCount)
    {
        for(int i =0; i < FireballCount; i++)
        {
            GameObject fireball = Instantiate(effect) as GameObject;
            fireball.SetActive(false);
            FireballPool.Add(fireball);
        }
    }

    public GameObject GetFireball()
    {
        GameObject reqFireball = null;

        for(int i =0; i<FireballPool.Count; i++)
        {
            if(FireballPool[i].activeSelf == false)
            {
                reqFireball = FireballPool[i];
                break;
            }
        }

        if(reqFireball == null)
        {
            GameObject newFireball = Instantiate<GameObject>(effect) as GameObject;
            reqFireball = newFireball;
        }

        reqFireball.SetActive(true);

        return reqFireball;
    }

    private IEnumerator OnRelease(GameObject fireball)
    {
        var wfs = new WaitForSeconds(1.5f);
        yield return wfs;
        fireball.SetActive(false);
        yield break;
    }

    public void MonsterDieEvent()
    {
        if(OnMonsterDieEvent != null)
        {
            OnMonsterDieEvent.Invoke();
        }
    }

    void FailAttack()
    {
        GameObject g = Instantiate(fail) as GameObject;
        g.transform.position = FirePos.transform.position - new Vector3(-0.2f, 0.35f, -0.39f);
        Destroy(g, 0.5f);
    }

    void FinishlAttack()
    {
        GameObject g = Instantiate(finish) as GameObject;
        g.transform.SetPositionAndRotation(FirePos.transform.position - new Vector3(-0.2f, 1.7f, -0.39f), FirePos.transform.rotation) ;
        Destroy(g, 1.6f);
    }
}
