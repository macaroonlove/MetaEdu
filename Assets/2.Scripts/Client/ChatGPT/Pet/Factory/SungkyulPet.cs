using Photon.Pun;
using UnityEngine;

public class SungkyulPet : Pet
{
    public override Pet Evolution()
    {
        GameObject Pet = PhotonNetwork.Instantiate($"Level_{level}", transform.position, transform.rotation);

        Pet.GetComponent<SungkyulPet>().Player = this.Player;
        PhotonNetwork.Destroy(gameObject);
        return Pet.GetComponent<SungkyulPet>();
    }
}
