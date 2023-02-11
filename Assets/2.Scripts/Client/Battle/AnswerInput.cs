using Cinemachine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AnswerInput : MonoBehaviour
{
    QuizManager quizManager;

    public delegate void MonsterDie();
    public static event MonsterDie OnMonsterDie;
    public TMP_InputField inputAnswer;
    public TextMeshProUGUI answer;

    private int _correctDescriptiveAnswer;

    void Awake()
    {
        quizManager = GameObject.Find("QuizManager").GetComponent<QuizManager>();
        if (gameObject.name == "Descriptive" || gameObject.name == "Short")
        {
            inputAnswer = transform.GetChild(0).GetComponent<TMP_InputField>();
        }
        else
            answer = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    }

    public void CheckAnswer()
    {
        if(quizManager.Question[quizManager.currQuiz].Split("▥")[0] == "0")
        {
            ChoicAnswer();
        }
        else if (quizManager.Question[quizManager.currQuiz].Split("▥")[0] == "1")
        {
            ShortAnswer();
        }
        else if (quizManager.Question[quizManager.currQuiz].Split("▥")[0] == "2")
        {
            DescriptiveAnswer();
        }
    }

    public void ChoicAnswer()
    {
        if (answer.text.Contains(quizManager.choiceAnswer))
        {
            OnMonsterDie();
            quizManager.QuestionPanel.SetActive(false);
            quizManager.AnswerPanel.SetActive(false);
            quizManager.AnswerPanel.transform.GetChild(0).gameObject.SetActive(false);
            Camera.main.cullingMask = -1;
        }
        else
        {
            Debug.Log("틀림");
        }
    }

    public void ShortAnswer()
    {
        if (inputAnswer.text.Contains(quizManager.shortAnswer))
        {
            OnMonsterDie();
            inputAnswer.text = "";
            quizManager.QuestionPanel.SetActive(false);
            quizManager.AnswerPanel.SetActive(false);
            quizManager.AnswerPanel.transform.GetChild(1).gameObject.SetActive(false);
            Camera.main.cullingMask = -1;
        }
        else
        {
            Debug.Log("틀림");
        }
    }

    public void DescriptiveAnswer()
    {
        for (int i = 1; i < quizManager.Question[quizManager.currQuiz].Split("▥")[2].Split("#").Length; i++)
        {
            if (inputAnswer.text.Contains(quizManager.Question[quizManager.currQuiz].Split("▥")[2].Split("#")[i]))
            {
                _correctDescriptiveAnswer++;
            }
        }

        if (_correctDescriptiveAnswer >= int.Parse(quizManager.Question[quizManager.currQuiz].Split("▥")[3]))
        {
            OnMonsterDie();
            inputAnswer.text = "";
            quizManager.QuestionPanel.SetActive(false);
            quizManager.AnswerPanel.SetActive(false);
            quizManager.AnswerPanel.transform.GetChild(2).gameObject.SetActive(false);
            Camera.main.cullingMask = -1;
        }
        else
        {
            Debug.Log("틀림");
        }

    }
}
