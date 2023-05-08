using Cinemachine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetController : MonoBehaviour
{
    public GameObject Player;
    public GameObject SpeechBubble;
    private Vector3 _target;
    private Vector3 _offset = new Vector3(1.068f, 1.5f, -0.521f);
    private PlayerController _playerController;
    void Start()
    {
        Player = GetComponent<SungkyulPet>().Player;
        _playerController = Player.GetComponent<PlayerController>();
        SpeechBubble = transform.GetChild(0).gameObject;
    }

    void Update()
    {
        if (!ReferenceEquals(Player, null))
        {
            if (!_playerController.GPTState)
            {
                _target = Player.transform.position + Player.transform.forward * _offset.z + Player.transform.up * _offset.y + Player.transform.right * _offset.x;
                transform.position = Vector3.Lerp(transform.position, _target, Time.deltaTime * 5);
                transform.LookAt(_target);
            }
            else
                ChatPosition();
        }
        else
            gameObject.GetComponent<PetController>().enabled = false;
    }

    public void ChatPosition()
    {
        Vector3 newTarget = Player.transform.position + Player.transform.forward * 2.721f + Player.transform.up * 1.74f + Player.transform.right * -0.483f;
        transform.position = Vector3.Lerp(transform.position, newTarget, Time.deltaTime * 3);
        transform.LookAt(_target);
    }
}
