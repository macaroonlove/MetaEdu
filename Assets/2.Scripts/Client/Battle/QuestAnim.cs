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
        anim.SetBool("isStart", true);
    }
    private void OnDisable()
    {
        anim.SetBool("isStart", false);
        anim.SetTrigger("isClose");
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
