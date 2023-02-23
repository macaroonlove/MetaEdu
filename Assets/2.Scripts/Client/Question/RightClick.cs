using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RightClick : MonoBehaviour, IPointerClickHandler
{
    public GameObject addAnswer;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            gameObject.SetActive(false);
            if (!addAnswer.activeSelf)
            {
                addAnswer.SetActive(true);
            }
        }
    }
}
