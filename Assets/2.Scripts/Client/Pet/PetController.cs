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
    private Vector3 _offset = new Vector3(0.634f, 1.65f, -0.361f);
    private PlayerController _playerController;
    private PhotonView _pv;
    void Start()
    {
        Player = GetComponent<SungkyulPet>().Player;

        if(GetComponent<PhotonView>().IsMine)
        {
            _playerController = Player.GetComponent<PlayerController>();
            SpeechBubble = transform.GetChild(0).gameObject;
        }
        else
            GetComponent<PetController>().enabled = false;
    }

    void Update()
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

    public void ChatPosition()
    {
        Vector3 newTarget = Player.transform.position + Player.transform.forward * 1.271f + Player.transform.up * 1.75f + Player.transform.right * 0.145f;
        transform.position = Vector3.Lerp(transform.position, newTarget, Time.deltaTime * 3);
        transform.LookAt(_target);
    }
}
