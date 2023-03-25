using UnityEngine;
using UnityEngine.EventSystems;

public class RightClick : MonoBehaviour, IPointerClickHandler
{
    public GameObject addAnswer;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button.Equals(PointerEventData.InputButton.Right))
        {
            gameObject.SetActive(false);
            if (!addAnswer.activeSelf)
            {
                addAnswer.SetActive(true);
            }
        }
    }
}
