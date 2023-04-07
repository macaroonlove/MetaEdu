using TMPro;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AnswerInput : MonoBehaviour
{
    QuizManager quizManager;
    PlayerBattle playerBattle;
    QuestMonsterAI questMonsterAI;

    public TMP_InputField inputAnswer;
    public TextMeshProUGUI answer;
    public int wrong;

    private int _correctDescriptiveAnswer;
    private Animator QuestAnim;

    void Awake()
    {
        quizManager = GameObject.Find("QuizManager").GetComponent<QuizManager>();
        playerBattle = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBattle>();
        questMonsterAI = playerBattle.transform.Find("QuestMonster1").GetComponent<QuestMonsterAI>();
        QuestAnim = GameObject.Find("BattleScene").GetComponent<Animator>();
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
            PannelDisable(0);
            if (playerBattle.hasAnim)
            {
                playerBattle.anim.SetTrigger(playerBattle.animFinish);
            }
        }
        else
        {
            if(wrong < 2)
            {
                QuestAnim.SetTrigger("isIDLE");
                playerBattle.anim.SetTrigger(playerBattle.animReact);
                Invoke("UIanim", 2.8f);
                wrong++;
            }
            else
            {
                PannelDisable(0);
                questMonsterAI.RunMonster();
                wrong = 0;
                playerBattle.Invoke("MonsterDieEvent", 3f);
            }
        }
    }

    public void ShortAnswer()
    {
        if (inputAnswer.text.Contains(quizManager.shortAnswer))
        {
            inputAnswer.text = "";
            PannelDisable(1);
            if (playerBattle.hasAnim)
            {
                playerBattle.anim.SetTrigger(playerBattle.animFinish);
            }
        }
        else
        {
            if (wrong < 2)
            {
                QuestAnim.SetTrigger("isIDLE");
                playerBattle.anim.SetTrigger(playerBattle.animReact);
                Invoke("UIanim", 2.8f);
                wrong++;
            }
            else
            {
                PannelDisable(1);
                questMonsterAI.RunMonster();
                wrong = 0;
                playerBattle.Invoke("MonsterDieEvent", 3f);
            }
        }
    }

    public void DescriptiveAnswer()
    {
        for (int i = 1; i < quizManager.Question[quizManager.currQuiz].Split("¢È")[2].Split("#").Length; i++)
        {
            if (inputAnswer.text.Contains(quizManager.Question[quizManager.currQuiz].Split("¢È")[2].Split("#")[i].Replace(" ","")))
            {
                _correctDescriptiveAnswer++;
            }
        }

        if (_correctDescriptiveAnswer >= int.Parse(quizManager.Question[quizManager.currQuiz].Split("¢È")[3]))
        {
            inputAnswer.text = "";
            PannelDisable(2);
            if (playerBattle.hasAnim)
            {
                playerBattle.anim.SetTrigger(playerBattle.animFinish);
                _correctDescriptiveAnswer = 0;
            }
        }
        else
        {
            if (wrong < 2)
            {
                QuestAnim.SetTrigger("isIDLE");
                playerBattle.anim.SetTrigger(playerBattle.animReact);
                Invoke("UIanim", 2.8f);
                wrong++;
            }
            else
            {
                PannelDisable(2);
                questMonsterAI.RunMonster();
                wrong = 0;
                playerBattle.Invoke("MonsterDieEvent", 3f);
            }
        }
    }

    public void UIanim()
    {
        QuestAnim.SetTrigger("isStart");
    }

    public void PannelDisable(int a)
    {
        quizManager.QuestionPanel.SetActive(false);
        quizManager.AnswerPanel.SetActive(false);
        quizManager.AnswerPanel.transform.GetChild(a).gameObject.SetActive(false);
    }
}