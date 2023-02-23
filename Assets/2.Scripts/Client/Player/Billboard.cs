using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    Transform cam;
    GameObject nickText;
    void Start()
    {
        cam = GameObject.Find("Main Camera").transform;
        nickText = gameObject.transform.GetChild(0).gameObject;
    }

    void Update()
    {
        try
        {
            if (Singleton.Inst.showName == 1 && nickText.activeSelf)
            {
                nickText.SetActive(false);
            }
            else if (Singleton.Inst.showName == 0 && !nickText.activeSelf)
            {
                nickText.SetActive(true);
            }
            else if (Singleton.Inst.showName == 0)
            {
                transform.LookAt(transform.position + cam.rotation * Vector3.forward, cam.rotation * Vector3.up);
            }
        }
        catch
        {
            cam = GameObject.Find("Main Camera").transform;
            nickText = gameObject.transform.GetChild(0).gameObject;
        }
    }
}
