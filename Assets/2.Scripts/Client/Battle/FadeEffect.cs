using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class FadeEffect : MonoBehaviour
{
    public Image image;
    private Color color;
    void OnEnable()
    {
        color = image.color;
        image.fillAmount = 1;
        color.a = 0;
        StartCoroutine("FadeIn");
    }

    void OnDisable()
    {
        StopCoroutine("FadeIn");
    }

    public IEnumerator FadeIn()
    {
        while (image.fillAmount != 0)
        {
            color.a += Time.deltaTime  / 2f ;
            //image.color = color;

            if (color.a >0.8)
            {
                //image.fillAmount -= Time.deltaTime;
                color.a -= Time.deltaTime;
            }

            yield return null;
        }
        
    }
}
