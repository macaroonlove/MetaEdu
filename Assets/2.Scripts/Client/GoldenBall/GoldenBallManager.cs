using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class GoldenBallManager : MonoBehaviour
{
    Singleton singleton;
    Score score;

    [Header("Quiz MGR")]
    public string questionList;
    public List<string> Question = new List<string>();
    public GameObject FullScreen;
    [Header("Text")]
    public TextMeshProUGUI LED_QuestionText;
    public TextMeshProUGUI QuestionText;
    public TextMeshProUGUI AnswerText;
    public TextMeshProUGUI TimeText;

    public bool countTime = true;

    private float currTime;
    private int currQuiz;
    private List<string> choiceAnswerList = new List<string>();
    private string choiceAnswer;
    private string shortAnswer;
    private int descriptiveAnswer;
    void Start()
    {
        currTime = 5;
        currQuiz = 0;
        singleton = GameObject.Find("SingletonManager").GetComponent<Singleton>();
        score = GameObject.Find("ScoreManager").GetComponent<Score>();
        questionList = singleton.question[singleton.currSelect];
        AddQuestion();
        CreateQuiz();
    }
    void Update()
    {
        if (currQuiz < Question.Count && countTime)
        {
            currTime -= Time.deltaTime;
            TimeText.text = ((int)currTime).ToString();
            if (currTime < 0)
            {
                CheckAnswer();
                Invoke("CreateQuiz", 5);
                countTime = false;
            }
        }
    }

    void AddQuestion()
    {
        for(int i=1; i<questionList.Split("▤").Length; i++)
        {
            Question.Add(questionList.Split("▤")[i]);
        }
        Shuffle(Question);
    }
    
    void CreateQuiz()
    {
        if (Question[currQuiz].Split("▥")[0] == "0")
        {
            LED_QuestionText.text = Question[currQuiz].Split("▥")[1];
            for(int i = 2; i < Question[currQuiz].Split("▥").Length; i++)
            {
                choiceAnswerList.Add(Question[currQuiz].Split("▥")[i]);
            }

            choiceAnswer = Question[currQuiz].Split("▥")[2];
            choiceAnswerList.RemoveAll(s => s == "");
            Shuffle(choiceAnswerList);

            for(int i = 0; i < choiceAnswerList.Count; i++)
            {
                LED_QuestionText.text += "\n" + (i + 1) + ". " + choiceAnswerList[i];
            }
            choiceAnswerList.Clear();
            QuestionText.text = LED_QuestionText.text;
        }
        else if(Question[currQuiz].Split("▥")[0] == "1")
        {
            LED_QuestionText.text = Question[currQuiz].Split("▥")[1];
            QuestionText.text = LED_QuestionText.text;
        }
        else if (Question[currQuiz].Split("▥")[0] == "2")
        {
            LED_QuestionText.text = Question[currQuiz].Split("▥")[1];
            QuestionText.text = LED_QuestionText.text;
        }

        countTime = true;
        currTime = 10;
    }

    void CheckAnswer()
    {
        if (Question[currQuiz].Split("▥")[0] == "0")
        {
            if (AnswerText.text.Contains(choiceAnswer))
            {
                score.currScore += 100;
            }
            else
            {
                Debug.Log("틀림");
            }
        }
        else if (Question[currQuiz].Split("▥")[0] == "1")
        {
            if (AnswerText.text.Contains(Question[currQuiz].Split("▥")[2]))
            {
                score.currScore += 100;
            }
            else
            {
                Debug.Log("틀림");
            }
        }
        else if (Question[currQuiz].Split("▥")[0] == "2")
        {
            for(int i = 1; i < Question[currQuiz].Split("▥")[2].Split("#").Length; i++)
            {
                if (AnswerText.text.Contains(Question[currQuiz].Split("▥")[2].Split("#")[i]))
                {
                    descriptiveAnswer++;
                }
            }
            if (descriptiveAnswer >= int.Parse(Question[currQuiz].Split("▥")[3]))
            {
                score.currScore += 100;
            }
            else
            {
                Debug.Log("틀림");
            }
        }

        descriptiveAnswer = 0;
        AnswerText.text = "";
        currQuiz += 1;
    }

    void Shuffle(List<string> list)
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

    public void OnFullScreen()
    {
        FullScreen.SetActive(true);
    }
    public void OffFullScreen()
    {
        FullScreen.SetActive(false);
    }
}
