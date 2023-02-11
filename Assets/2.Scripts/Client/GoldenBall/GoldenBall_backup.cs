using Photon.Pun;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GoldenBall_backup : MonoBehaviour
{
    GoogleSheet googleSheet;
    Score score;
    FadeEffect fadeEffect;

    [Header("Quiz MGR")]
    public Dictionary<int, List<string>> Quizs;
    public List<string> Question = new List<string>();
    public List<string> Answer = new List<string>();
    public GameObject FullScreen;
    [Header("Text")]
    public TextMeshProUGUI LED_QuestionText;
    public TextMeshProUGUI QuestionText;
    public TMP_InputField AnswerText;
    public TextMeshProUGUI TimeText;

    public bool countTime = true;

    private float _currTime;
    private int _currQuiz;
    private string _answer;
    private bool _start = false;
    void Start()
    {
        _currTime = 5;
        googleSheet = GameObject.Find("SingletonManager").GetComponent<GoogleSheet>();
        score = GameObject.Find("ScoreManager").GetComponent<Score>();
        FullScreen = GameObject.Find("FullScreenQuestion");
        LED_QuestionText = GameObject.Find("LED_Question").transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        QuestionText = FullScreen.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();
        AnswerText = FullScreen.transform.GetChild(1).GetChild(1).GetComponent<TMP_InputField>();
        TimeText = GameObject.Find("Time").GetComponent<TextMeshProUGUI>();
        fadeEffect = GameObject.Find("FadeEffect").GetComponent<FadeEffect>();
        Quizs = googleSheet.Quizs;
        AddQuestion();
    }
    void Update()
    {
        if (Question != null && countTime)
        {
            _currTime -= Time.deltaTime;
            TimeText.text = ((int)_currTime).ToString();
            if (_currTime < 0)
            {
                if(_start)
                {
                    CheckAnswer();
                    Invoke("CreateQuiz", 5);
                    countTime = false;
                }
                else
                {
                    Invoke("CreateQuiz", 1);
                    countTime = false;
                }
            }
        }
    }

    void AddQuestion()
    {
        int QuizTitle = UnityEngine.Random.Range(0, Quizs.Count);
        for (int i = 0; i < Quizs[QuizTitle].Count; i++)
        {
            if (i%2 != 0)
            {
                Answer.Add(Quizs[QuizTitle][i]);
            }
            else if(i%2 == 0)
            {
                Question.Add(Quizs[QuizTitle][i]);
            }
        }
    }

    void CreateQuiz()
    {
        if(Question.Count !=0)
        {
            if(score.scoreText != null)
            {
                score.PV.RPC("SendScore", RpcTarget.AllViaServer, score.currScore);
            }
            _currQuiz = UnityEngine.Random.Range(0, Question.Count);
            LED_QuestionText.text = Question[_currQuiz];
            QuestionText.text = LED_QuestionText.text;
            countTime = true;
            _currTime = 11;
            Question.Remove(Question[_currQuiz]);
            _start = true;
        }
    }

    void CheckAnswer()
    {
        print(AnswerText.text);
        if (AnswerText.text != null)
        {
            score.PV.RPC("SendAnswer", RpcTarget.AllViaServer, AnswerText.text);
        }
        if (Answer[_currQuiz].Contains(AnswerText.text) && AnswerText.text != "")
        {
            score.currScore += 100;
            AnswerText.text = "";
            FullScreen.SetActive(false);
        }
        else
        {
            fadeEffect.StartCoroutine("FadeOutStart");
            score.PV.RPC("OnEffect", RpcTarget.AllViaServer);
            score.OnAttack();
            AnswerText.text = "";
            FullScreen.SetActive(false);
        }
        Answer.Remove(Answer[_currQuiz]);
    }

    public void OnFullScreen()
    {
        FullScreen.SetActive(true);
    }
    public void OffFullScreen()
    {
        FullScreen.SetActive(false);
    }
}
