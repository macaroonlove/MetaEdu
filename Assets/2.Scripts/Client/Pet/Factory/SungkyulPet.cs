using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SungkyulPet : Pet
{
    public override Pet Evolution()
    {
        GameObject Pet = null;
        if (level == 1)
        {
            Pet = PhotonNetwork.Instantiate("Level_1", transform.position, transform.rotation);
        }
        else if(level == 2)
        {
            Pet = PhotonNetwork.Instantiate("Level_2", transform.position, transform.rotation);    
        }
        else if(level == 3)
        {
            Pet = PhotonNetwork.Instantiate("Level_3", transform.position, transform.rotation);
        }
        else if(level == 4)
        {
            Pet = PhotonNetwork.Instantiate("Level_4", transform.position, transform.rotation);
        }   
        else if(level == 5)
        {
            Pet = PhotonNetwork.Instantiate("Level_5", transform.position, transform.rotation);
        }
        Pet.GetComponent<Pet>().Player = this.Player;
        PhotonNetwork.Destroy(gameObject);
        return Pet.GetComponent<Pet>();
    }
}
