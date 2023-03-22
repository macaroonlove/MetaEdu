using UnityEngine;

public class ErrorToast : MonoBehaviour
{
    void OnEnable()
    {
        Invoke(nameof(Off), 3f);
    }

    void Off()
    {
        gameObject.SetActive(false);
    }
}
