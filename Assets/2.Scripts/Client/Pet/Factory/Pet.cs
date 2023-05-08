using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Pet : MonoBehaviour
{
    public int ex;
    public int level;
    public GameObject Player;
    public abstract Pet Evolution();
}
