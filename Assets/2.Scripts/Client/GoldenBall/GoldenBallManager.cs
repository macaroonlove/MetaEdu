using Photon.Pun;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GoldenBallManager : MonoBehaviourPunCallbacks, IPunObservable
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

    [Header("Ranking")]
    public TextMeshProUGUI[] RankingText;
    public GameObject Ranking;


    public bool countTime = true;
    public PhotonView PV;

    private int _QuizTitle;
    private float _currTime;
    private int _currQuiz;
    private bool _start = false;
    private bool _end = false;

    private int[] _finalSocre = new int[4];
    private int[] _rank = new int[4];
    private int _count;
    void Start()
    {
        _currTime = 8;
        googleSheet = GameObject.Find("SingletonManager").GetComponent<GoogleSheet>();
        score = GameObject.FindGameObjectWithTag("Player").GetComponent<Score>();
        fadeEffect = GameObject.Find("FadeEffect").GetComponent<FadeEffect>();
        PV = GetComponent<PhotonView>();
        Quizs = googleSheet.Quizs;
        Invoke("AddQuestion", 4f);
    }

    void Update()
    {
        if (countTime)
        {
            if(PhotonNetwork.IsMasterClient)
            {
                _currTime -= Time.deltaTime;
            }
            TimeText.text = ((int)_currTime).ToString();
            if (_currTime < 0)
            {
                if (_start)
                {
                    CheckAnswer();
                    Invoke("CreateQuiz", 6);
                    countTime = false;
                }
                else
                {
                    Invoke("CreateQuiz", 2);
                    countTime = false;
                }
            }
        }
    }


    void AddQuestion()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            _QuizTitle = UnityEngine.Random.Range(0, Quizs.Count);
            PV.RPC("SendAddQuestion", RpcTarget.All, _QuizTitle);
        }


    }

    [PunRPC]
    void SendAddQuestion(int QuizTitle)
    {
        for (int i = 0; i < Quizs[QuizTitle].Count; i++)
        {
            if (i % 2 != 0)
            {
                Answer.Add(Quizs[QuizTitle][i]);
            }
            else if (i % 2 == 0)
            {
                Question.Add(Quizs[QuizTitle][i]);
            }
        }
    }

    void CreateQuiz()
    {
        if (Answer.Count > 0)
        {
            score.PV.RPC("SendScore", RpcTarget.All, score.currScore);

            if (PhotonNetwork.IsMasterClient)
            {
                _currQuiz = UnityEngine.Random.Range(0, Question.Count);
                PV.RPC("SendCreateQuiz", RpcTarget.AllViaServer, _currQuiz);
            }

            _currTime = 11;
            _start = true;
            countTime = true;
        }
        /*
        else
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                if(_end)
                {
                    countTime = false;
                }
                else
                {
                    countTime = true;
                    _end = true;
                }
            }
        }
        */
        
        if (Answer.Count == 0)
        {
            score.PV.RPC("SendScore", RpcTarget.AllViaServer, score.currScore);
            PV.RPC("Rank", RpcTarget.AllViaServer);
        }
    }

    [PunRPC]
    void SendCreateQuiz(int currQuiz)
    {
        LED_QuestionText.text = Question[currQuiz];
        QuestionText.text = LED_QuestionText.text;
        Question.Remove(Question[currQuiz]);
    }

    void CheckAnswer()
    {
        score.PV.RPC("SendAnswer", RpcTarget.All, AnswerText.text);

        AnswerText.text = Regex.Replace(AnswerText.text, @"[^0-9a-zA-Z°¡-ÆR]", "");
        Answer[_currQuiz] = Regex.Replace(Answer[_currQuiz], @"[^0-9a-zA-Z°¡-ÆR]", "");

        if (AnswerText.text.Replace(" ", "") == Answer[_currQuiz].Replace(" ", "") && AnswerText.text != "")
        {
            score.currScore += 100;
            AnswerText.text = "";
            FullScreen.SetActive(false);
        }
        else
        {
            fadeEffect.StartCoroutine("FadeOutStart");
            score.PV.RPC("OnEffect", RpcTarget.All);
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

    [PunRPC]
    public void Rank()
    {
        TimeText.gameObject.SetActive(false);

        int.TryParse(score.player1Text.text, out _finalSocre[0]);
        int.TryParse(score.player2Text.text, out _finalSocre[1]);
        int.TryParse(score.player3Text.text, out _finalSocre[2]);
        int.TryParse(score.player4Text.text, out _finalSocre[3]);

        for (int i = 0; i < 4; i++)
        {
            _count = 1;
            for (int j = 0; j < 4; j++)
            {
                if (_finalSocre[i] < _finalSocre[j])
                {
                    _count++;
                }
            }
            _rank[i] = _count++;
        }

        Ranking.SetActive(true);

        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            RankingText[i].text = _rank[i].ToString() + ".  " + PhotonNetwork.PlayerList[i].NickName + "  :  " + _finalSocre[i]; ;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(_QuizTitle);
            stream.SendNext(_currTime);
            stream.SendNext(_currQuiz);
        }
        else
        {
            // Network player, receive data
            this._QuizTitle = (int)stream.ReceiveNext();
            this._currTime = (float)stream.ReceiveNext();
            this._currQuiz = (int)stream.ReceiveNext();
        }
    }
}
