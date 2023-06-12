using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class QuizManager : MonoBehaviourPunCallbacks
{

    [Header("Quiz MGR")]
    public QuestionData questionList;
    public List<TextMeshProUGUI> choiceAnswers;
    public List<string> choiceAnswerList = new List<string>();
    public string choiceAnswer;
    public string shortAnswer;
    public string descriptiveAnswer;
    public int currQuiz;

    public List<Transform> points = new List<Transform>();
    public List<GameObject> MonsterPool = new List<GameObject>();
        
    public GameObject QuestionPanel;
    public GameObject AnswerPanel;
    public GameObject FadeEffect;
    public GameObject battleText;
    public PhotonView PV;

    private GameObject _spawnPoint;
    private GameObject _Monsters;
    private TextMeshProUGUI _questionText;
    void Start()
    {
        _spawnPoint = GameObject.Find("SpawnPointsGroup");
        _Monsters = GameObject.Find("MonsterGroup");

        //ÄûÁî UI
        QuestionPanel = GameObject.Find("QuestionPanel");
        AnswerPanel = GameObject.Find("AnswerPanel");
        battleText = GameObject.Find("BattleText");
        _questionText = QuestionPanel.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();

        QuestionPanel.SetActive(false);
        AnswerPanel.SetActive(false);
        battleText.SetActive(false);

        for (int i = 0; i < _spawnPoint.transform.childCount; i++)
        {
            points.Add(_spawnPoint.transform.GetChild(i));
        }

        for (int i = 0; i < _Monsters.transform.childCount; i++)
        {
            MonsterPool.Add(_Monsters.transform.GetChild(i).gameObject);
        }

        InvokeRepeating("CreateMonster", 2.0f, 3.0f);

        if (!QuestionManager.Inst.questionDatas.Count.Equals(0)) InitQuiz();
        else NoQuiz();
    }

    void NoQuiz()
    {
        int num1 = Random.Range(0, 19);
        int num2 = Random.Range(0, 19);
        questionList = new QuestionData();
        for (int i = 0; i < 10; i++)
        {
            questionList.answer.Add("0¢È" + num1 + "x" + num2 + " = ?¢È" + num1 * num2 + "¢È" + Random.Range(0, 361) + "¢È" + Random.Range(0, 361) + "¢È" + Random.Range(0, 361) + "¢È¢È¢È¢È¢È");
        }
    }

    public void InitQuiz()
    {
        questionList = QuestionManager.Inst.questionDatas[QuestionManager.Inst.selectQuestion];
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

    public void CreateQuiz()
    {
        currQuiz = Random.Range(0, questionList.answer.Count);
        if (questionList.answer[currQuiz].Split("¢È")[0] == "0")
        {
            _questionText.text = questionList.answer[currQuiz].Split("¢È")[1];
            for (int i = 2; i < questionList.answer[currQuiz].Split("¢È").Length; i++)
            {
                choiceAnswerList.Add(questionList.answer[currQuiz].Split("¢È")[i]);
            }

            choiceAnswer = questionList.answer[currQuiz].Split("¢È")[2];
            choiceAnswerList.RemoveAll(s => s == "");
            Shuffle(choiceAnswerList);

            for(int i=0; i< choiceAnswerList.Count; i++)
            {
                choiceAnswers[i].text = choiceAnswerList[i];
            }

        }
        else if (questionList.answer[currQuiz].Split("¢È")[0] == "1")
        {
            _questionText.text = questionList.answer[currQuiz].Split("¢È")[1];
            shortAnswer = questionList.answer[currQuiz].Split("¢È")[2];
        }
        else if (questionList.answer[currQuiz].Split("¢È")[0] == "2")
        {
            _questionText.text = questionList.answer[currQuiz].Split("¢È")[1];
            descriptiveAnswer = questionList.answer[currQuiz].Split("¢È")[2];
        }
    }

    public void Shuffle(List<string> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int a = UnityEngine.Random.Range(0, list.Count);
            int b = UnityEngine.Random.Range(0, list.Count);
            string temp;
            temp = list[a];
            list[a] = list[b];
            list[b] = temp;
        }
    }
}
