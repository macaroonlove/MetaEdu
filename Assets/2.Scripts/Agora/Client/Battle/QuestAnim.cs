using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestAnim : MonoBehaviour
{
    public Animator anim;
    private bool open = false;

    private void OnEnable()
    {
        anim.SetTrigger("isStart");
    }
    private void OnDisable()
    {
        anim.SetTrigger("isIDLE");
    }
    public void questAnim()
    {
        if (open == true)
        {
            anim.SetTrigger("isOpen");
            open = false;
        }
        else if (open == false)
        {
            anim.SetTrigger("isClose");
            open = true;
        }
    }
}
