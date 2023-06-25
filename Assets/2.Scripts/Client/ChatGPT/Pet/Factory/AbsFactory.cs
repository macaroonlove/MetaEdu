using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbsFactory
{
    public abstract Pet CreatePet(Vector3 position, Quaternion rotation, int Level);
}
