using TMPro;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AnswerInput : MonoBehaviour
{
    QuizManager quizManager;
    PlayerBattle playerBattle;
    QuestMonsterAI questMonsterAI;

    public TMP_InputField inputAnswer;
    public TextMeshProUGUI answer;
    public static int wrong;

    private int _correctDescriptiveAnswer;
    private Animator QuestAnim;
    private IngamePhotonManager _photonManager;
    private void OnEnable()
    {
        wrong = 0;
    }
    void Awake()
    {
        quizManager = GameObject.Find("QuizManager").GetComponent<QuizManager>();
        playerBattle = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBattle>();
        questMonsterAI = playerBattle.transform.Find("QuestMonster1").GetComponent<QuestMonsterAI>();
        QuestAnim = GameObject.Find("BattleScene").GetComponent<Animator>();
        _photonManager = GameObject.Find("PhotonManager").GetComponent<IngamePhotonManager>();
    }

    public void ChoicAnswer()
    {
        if (answer.text.Contains(quizManager.choiceAnswer))
        {
            for (int i = 0; i < 4; i++)
            {
                quizManager.AnswerPanel.transform.GetChild(0).GetChild(i).gameObject.SetActive(true);
            }
            PannelDisable(0);
            if (playerBattle.hasAnim)
            {
                playerBattle.anim.SetTrigger(playerBattle.animFinish);
                Invoke("DelayFinishAttackSound", 1.7f);
                _photonManager.AddEx(20);
            }
        }
        else
        {
            if (wrong < 2)
            {
                QuestAnim.SetTrigger("isIDLE");
                playerBattle.anim.SetTrigger(playerBattle.animReact);
                Invoke("DelayAttackFailSound", 2.0f);
                Invoke("UIanim", 2.8f);
                wrong++;
            }
            else
            {
                playerBattle.anim.SetTrigger(playerBattle.animReact);
                PannelDisable(0);
                Invoke("DelayAttackFailSound", 2.0f);
                Invoke("EscapeMonster", 2.8f);
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
                Invoke("DelayFinishAttackSound", 1.7f);
                _photonManager.AddEx(20);
            }
        }
        else
        {
            if (wrong < 2)
            {
                inputAnswer.text = "";
                QuestAnim.SetTrigger("isIDLE");
                playerBattle.anim.SetTrigger(playerBattle.animReact);
                Invoke("DelayAttackFailSound", 2.0f);
                Invoke("UIanim", 2.8f);
                wrong++;
            }
            else
            {
                inputAnswer.text = "";
                playerBattle.anim.SetTrigger(playerBattle.animReact);
                PannelDisable(1);
                Invoke("DelayAttackFailSound", 2.0f);
                Invoke("EscapeMonster", 2.8f);
            }
        }
    }

    public void DescriptiveAnswer()
    {
        for (int i = 1; i < quizManager.Question[quizManager.currQuiz].Split("▥")[2].Split("#").Length; i++)
        {
            if (inputAnswer.text.Contains(quizManager.Question[quizManager.currQuiz].Split("▥")[2].Split("#")[i].Replace(" ","")))
            {
                _correctDescriptiveAnswer++;
            }
        }

        if (_correctDescriptiveAnswer >= int.Parse(quizManager.Question[quizManager.currQuiz].Split("▥")[3]))
        {
            inputAnswer.text = "";
            PannelDisable(2);
            if (playerBattle.hasAnim)
            {
                playerBattle.anim.SetTrigger(playerBattle.animFinish);
                Invoke("DelayFinishAttackSound", 1.7f);
                _correctDescriptiveAnswer = 0;
                _photonManager.AddEx(20);
            }
        }
        else
        {
            if (wrong < 2)
            {
                inputAnswer.text = "";
                QuestAnim.SetTrigger("isIDLE");
                playerBattle.anim.SetTrigger(playerBattle.animReact);
                Invoke("DelayAttackFailSound", 2.0f);
                Invoke("UIanim", 2.8f);
                wrong++;
            }
            else
            {
                inputAnswer.text = "";
                playerBattle.anim.SetTrigger(playerBattle.animReact);
                PannelDisable(2);
                Invoke("DelayAttackFailSound", 2.0f);
                Invoke("EscapeMonster",2.8f);
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

    public void EscapeMonster()
    {
        questMonsterAI.RunMonster();
        playerBattle.Invoke("MonsterDieEvent", 3f);
        Invoke("DelayEscapeSound", 1f);
    }
    public void DelayEscapeSound()
    {
        SoundManager.Instance.EscapeSound();
    }
    public void DelayFinishAttackSound()
    {
        SoundManager.Instance.FinishAttackSound();
    }
    public void DelayAttackFailSound()
    {
        SoundManager.Instance.AttackFailSound();
    }
}