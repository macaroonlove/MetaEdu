using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestAnim : MonoBehaviour
{
    public Animator anim;
    private bool open = false;

    QuizManager quizManager;
    public TextMeshProUGUI text;
    private void OnEnable()
    {
        anim.SetBool("isStart", true);
//text.GetComponent<TextMeshPro>().text = quizManager.Quest[Random.RandomRange(1, quizManager.Quest.Count)];
    }
    private void OnDisable()
    {
        anim.SetBool("isStart", false);
        anim.SetTrigger("IsClose");
    }

    private void Awake()
    {
        anim = GetComponent<Animator>();
        quizManager = GameObject.Find("QuizManager").GetComponent<QuizManager>();

    }
    public void questAnim()
    {
        if (open == true)
        {
            anim.SetTrigger("IsOpen");
            open = false;
        }
        else if (open == false)
        {

            anim.SetTrigger("IsClose");
            text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
            open = true;
        }
    }

    public void textDis()
    {
        text.color = new Color(text.color.r, text.color.g, text.color.b, 1);
    }
}
