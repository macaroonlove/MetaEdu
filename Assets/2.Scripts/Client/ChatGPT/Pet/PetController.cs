using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PetController : MonoBehaviour
{
    public GameObject Player;
    public GameObject SpeechBubble;

    private Vector3 _target;
    private Vector3 _offset = new Vector3(0.834f, 0, 0.681f);
    private Vector3 newTarget;
    private Quaternion _petHead;
    private PlayerController _playerController;
    private PhotonView _pv;
    private Animator _petAnim;
    void Start()
    {
        Player = GetComponent<SungkyulPet>().Player;
        if (GetComponent<PhotonView>().IsMine)
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
            _target = Player.transform.position + Player.transform.forward * _offset.z + Player.transform.right * _offset.x;
            transform.position = Vector3.Lerp(transform.position, _target, Time.deltaTime * 5);
            if ((_target - transform.position).normalized != new Vector3(0, 0, 0))
            {
                _petHead = Quaternion.LookRotation((_target - transform.position).normalized);
                transform.rotation = Quaternion.Lerp(transform.rotation, _petHead, Time.deltaTime * 3);
            }
            if (Vector3.Distance(transform.position, Player.transform.position) < 0.95f)
            {
                transform.LookAt(Player.transform);
            }
        }
        else
        {
            ChatPosition();
        }
    }

    public void ChatPosition()
    {

        RaycastHit groundHit;
        if (Physics.Raycast(transform.position + new Vector3(0,10,0), Vector3.down, out groundHit, Mathf.Infinity))
        {
            if(groundHit.collider.gameObject.layer == 6)
            {
                Vector3 newPetPosition = new Vector3(0, groundHit.point.y, 0);
                newTarget = Player.transform.position + Player.transform.forward * 3.271f + Player.transform.right * -0.345f;
                transform.position = Vector3.Lerp(transform.position, newTarget, Time.deltaTime * 3);
                transform.position = new Vector3(transform.position.x, groundHit.point.y, transform.position.z);
            }
        }

        transform.LookAt(Player.transform);
    }
}
