using Photon.Pun;
using System.Collections.Generic;
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
        InvokeRepeating("BeepSound", 0f, 1.0f);
    }

    void FixedUpdate()
    {
        if (countTime)
        {
            _currTime -= Time.deltaTime;
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

    public void BeepSound()
    {
        if(_currTime >= 0 &&_currTime < 5)
        {
            SoundManager.Instance.GoldenBallTime();
        }
    }

    void AddQuestion()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            _QuizTitle = UnityEngine.Random.Range(0, Quizs.Count);
            PV.RPC("SendAddQuestion", RpcTarget.AllViaServer, _QuizTitle);
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
        
        if (Answer.Count == 0)
        {
            score.PV.RPC("SendScore", RpcTarget.AllViaServer, score.currScore);
            Invoke("Rank", 1f);

        }
    }

    [PunRPC]
    void SendCreateQuiz(int currQuiz)
    {
        _currQuiz = currQuiz;
        LED_QuestionText.text = Question[_currQuiz];
        QuestionText.text = LED_QuestionText.text;
        Question.Remove(Question[_currQuiz]);
    }

    void CheckAnswer()
    {
        score.PV.RPC("SendAnswer", RpcTarget.All, AnswerText.text);

        AnswerText.text = Regex.Replace(AnswerText.text, @"[^0-9a-zA-Z��-�R]", "");
        Answer[_currQuiz] = Regex.Replace(Answer[_currQuiz], @"[^0-9a-zA-Z��-�R]", "");

        if (AnswerText.text.Contains(Answer[_currQuiz].Replace(" ","")) && AnswerText.text != "")
        {
            SoundManager.Instance.GoldenBallCorrect();
            score.currScore += 100;
            AnswerText.text = "";
            FullScreen.SetActive(false);
        }
        else
        {
            SoundManager.Instance.GoldenBallWrong();
            fadeEffect.StartCoroutine("FadeOutStart");
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
            if(_rank[i] == 1)
            {
                if(RankingText[0].text != "")
                {
                    if (RankingText[1].text != "")
                    {
                        if (RankingText[2].text != "")
                        {
                            RankingText[3].text = _rank[i].ToString() + ".  " + PhotonNetwork.PlayerList[i].NickName + "  :  " + _finalSocre[i];
                        }
                        else
                            RankingText[2].text = _rank[i].ToString() + ".  " + PhotonNetwork.PlayerList[i].NickName + "  :  " + _finalSocre[i];
                    }
                    else
                        RankingText[1].text = _rank[i].ToString() + ".  " + PhotonNetwork.PlayerList[i].NickName + "  :  " + _finalSocre[i];
                }
                else
                    RankingText[0].text = _rank[i].ToString() + ".  " + PhotonNetwork.PlayerList[i].NickName + "  :  " + _finalSocre[i];        
            }
            else if (_rank[i] == 2)
            {
                if (RankingText[1].text != "")
                {
                    if (RankingText[2].text != "")
                    {
                        RankingText[3].text = _rank[i].ToString() + ".  " + PhotonNetwork.PlayerList[i].NickName + "  :  " + _finalSocre[i];
                    }
                    else
                        RankingText[2].text = _rank[i].ToString() + ".  " + PhotonNetwork.PlayerList[i].NickName + "  :  " + _finalSocre[i];
                }
                else
                    RankingText[1].text = _rank[i].ToString() + ".  " + PhotonNetwork.PlayerList[i].NickName + "  :  " + _finalSocre[i];
            }
            else if (_rank[i] == 3)
            {
                if (RankingText[2].text != "")
                {
                    RankingText[3].text = _rank[i].ToString() + ".  " + PhotonNetwork.PlayerList[i].NickName + "  :  " + _finalSocre[i];
                }
                else
                    RankingText[2].text = _rank[i].ToString() + ".  " + PhotonNetwork.PlayerList[i].NickName + "  :  " + _finalSocre[i];
            }
            else if (_rank[i] == 4)
            {
                RankingText[3].text = _rank[i].ToString() + ".  " + PhotonNetwork.PlayerList[i].NickName + "  :  " + _finalSocre[i];
            }
        }
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            //stream.SendNext(_QuizTitle);
            stream.SendNext(_currTime);
            //stream.SendNext(_currQuiz);
        }
        else
        {
            // Network player, receive data
            //this._QuizTitle = (int)stream.ReceiveNext();
            this._currTime = (float)stream.ReceiveNext();
            //this._currQuiz = (int)stream.ReceiveNext();
        }
    }
}
