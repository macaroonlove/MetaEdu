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
    private Vector3 _offset = new Vector3(0.834f, 0, 0.681f);
    private Quaternion _petHead;
    private PlayerController _playerController;
    private PhotonView _pv;
    private Animator _petAnim;
    void Start()
    {
        Player = GetComponent<SungkyulPet>().Player;

        if(GetComponent<PhotonView>().IsMine)
        {
            _playerController = Player.GetComponent<PlayerController>();
            _petAnim = GetComponent<Animator>();
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
            _petHead = Quaternion.LookRotation((_target - transform.position).normalized);
            transform.rotation = Quaternion.Lerp(transform.rotation, _petHead, Time.deltaTime * 3);
            if (Vector3.Distance(transform.position, Player.transform.position) < 0.95f)
            {
                transform.LookAt(Player.transform);
            }
        }
        else
            ChatPosition();
    }

    public void ChatPosition()
    {
        Vector3 newTarget = Player.transform.position + Player.transform.forward * 3.271f + Player.transform.right * -0.345f;
        transform.position = Vector3.Lerp(transform.position, newTarget, Time.deltaTime * 3);
        transform.LookAt(Player.transform);
    }
}
