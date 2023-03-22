using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AnswerInput : MonoBehaviour
{
    QuizManager quizManager;
    PlayerBattle playerBattle;

    public TMP_InputField inputAnswer;
    public TextMeshProUGUI answer;

    private int _correctDescriptiveAnswer;

    void Awake()
    {
        quizManager = GameObject.Find("QuizManager").GetComponent<QuizManager>();
        playerBattle = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBattle>();
    }

    public void CheckAnswer()
    {
        if(quizManager.Question[quizManager.currQuiz].Split("¢È")[0] == "0")
        {
            ChoicAnswer();
        }
        else if (quizManager.Question[quizManager.currQuiz].Split("¢È")[0] == "1")
        {
            ShortAnswer();
        }
        else if (quizManager.Question[quizManager.currQuiz].Split("¢È")[0] == "2")
        {
            DescriptiveAnswer();
        }
    }

    public void ChoicAnswer()
    {
        if (answer.text.Contains(quizManager.choiceAnswer))
        {
            quizManager.QuestionPanel.SetActive(false);
            quizManager.AnswerPanel.SetActive(false);
            quizManager.AnswerPanel.transform.GetChild(0).gameObject.SetActive(false);

            if (playerBattle.hasAnim)
            {
                playerBattle.anim.SetTrigger(playerBattle.animFinish);
            }
        }
        else
        {
            quizManager.QuestionPanel.SetActive(false);
            quizManager.AnswerPanel.SetActive(false);
            quizManager.AnswerPanel.transform.GetChild(0).gameObject.SetActive(false);

            playerBattle.anim.SetTrigger(playerBattle.animReact);

            Invoke("UIanim1", 2.8f);
        }
    }

    public void ShortAnswer()
    {
        if (inputAnswer.text.Contains(quizManager.shortAnswer))
        {
            inputAnswer.text = "";
            quizManager.QuestionPanel.SetActive(false);
            quizManager.AnswerPanel.SetActive(false);
            quizManager.AnswerPanel.transform.GetChild(1).gameObject.SetActive(false);

            if (playerBattle.hasAnim)
            {
                playerBattle.anim.SetTrigger(playerBattle.animFinish);
            }
        }
        else
        {
            quizManager.QuestionPanel.SetActive(false);
            quizManager.AnswerPanel.SetActive(false);
            quizManager.AnswerPanel.transform.GetChild(1).gameObject.SetActive(false);

            playerBattle.anim.SetTrigger(playerBattle.animReact);


            Invoke("UIanim2", 2.8f);
        }
    }

    public void DescriptiveAnswer()
    {
        for (int i = 1; i < quizManager.Question[quizManager.currQuiz].Split("¢È")[2].Split("#").Length; i++)
        {
            if (inputAnswer.text.Contains(quizManager.Question[quizManager.currQuiz].Split("¢È")[2].Split("#")[i]))
            {
                _correctDescriptiveAnswer++;
            }
        }

        if (_correctDescriptiveAnswer >= int.Parse(quizManager.Question[quizManager.currQuiz].Split("¢È")[3]))
        {
            inputAnswer.text = "";
            quizManager.QuestionPanel.SetActive(false);
            quizManager.AnswerPanel.SetActive(false);
            quizManager.AnswerPanel.transform.GetChild(2).gameObject.SetActive(false);
            
            if (playerBattle.hasAnim)
            {
                playerBattle.anim.SetTrigger(playerBattle.animFinish);
            }
        }
        else
        {
            quizManager.QuestionPanel.SetActive(false);
            quizManager.AnswerPanel.SetActive(false);
            quizManager.AnswerPanel.transform.GetChild(2).gameObject.SetActive(false);

            playerBattle.anim.SetTrigger(playerBattle.animReact);

            Invoke("UIanim3", 2.8f);
        }
    }

    void UIanim1()
    {
        quizManager.QuestionPanel.SetActive(true);
        quizManager.AnswerPanel.SetActive(true);
        quizManager.AnswerPanel.transform.GetChild(0).gameObject.SetActive(true);
    }
    void UIanim2()
    {
        quizManager.QuestionPanel.SetActive(true);
        quizManager.AnswerPanel.SetActive(true);
        quizManager.AnswerPanel.transform.GetChild(1).gameObject.SetActive(true);
    }
    void UIanim3()
    {
        quizManager.QuestionPanel.SetActive(true);
        quizManager.AnswerPanel.SetActive(true);
        quizManager.AnswerPanel.transform.GetChild(2).gameObject.SetActive(true);
    }
}