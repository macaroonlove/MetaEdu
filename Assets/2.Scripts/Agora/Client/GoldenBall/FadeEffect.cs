using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class FadeEffect : MonoBehaviour
{
    public Image image;
    public IEnumerator FadeInStart()
    {
        for (float f = 1f; f > 0; f -= 0.005f)
        {
            Color c = image.color;
            c.a = f;
            image.color = c;
            yield return null;
        }
        yield break;
    }

    public IEnumerator FadeOutStart()
    {
        for (float f = 0f; f < 0.8; f += 0.01f)
        {
            Color c = image.color;
            c.a = f;
            image.color = c;
            yield return null;
        }
        StartCoroutine(FadeInStart());
        yield break;
    }

}
