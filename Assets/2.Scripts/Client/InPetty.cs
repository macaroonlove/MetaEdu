using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InPetty : MonoBehaviour
{
    void Awake()
    {
        if (!Singleton.Inst.isPatty)
        {
            SceneManager.LoadScene("Petty", LoadSceneMode.Additive);
            Singleton.Inst.isPatty = true;
        }
    }
}