using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionWindow : MonoBehaviour
{
    public GameObject questWindow;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            questWindow.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            questWindow.SetActive(false);
        }
    }
}