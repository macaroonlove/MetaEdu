using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetFactory : AbsFactory
{
    public override Pet CreatePet(Vector3 position, Quaternion rotation, int Level)
    {
        GameObject sungkyulPet = null;
        if (Level == 1)
        {
            sungkyulPet = PhotonNetwork.Instantiate("Level_1", position, rotation);
        }
        else if (Level == 2)
        {
            sungkyulPet = PhotonNetwork.Instantiate("Level_2", position, rotation);
        }
        else if (Level == 3)
        {
            sungkyulPet = PhotonNetwork.Instantiate("Level_3", position, rotation);
        }
        else if (Level == 4)
        {
            sungkyulPet = PhotonNetwork.Instantiate("Level_4", position, rotation);
        }

        return sungkyulPet.GetComponent<SungkyulPet>();
    }
}
