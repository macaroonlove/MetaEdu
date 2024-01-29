using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetFactory : AbsFactory
{
    public override Pet CreatePet(Vector3 position, Quaternion rotation, int Level)
    {
        GameObject pet = PhotonNetwork.Instantiate($"Level_{Level}", position, rotation);
        
        if(pet.GetComponent<Pet>() is SungkyulPet sungkyulPet)
        {
            return sungkyulPet;
        }

        return null;
    }
}
