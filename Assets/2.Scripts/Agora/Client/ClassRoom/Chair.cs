using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;
using TMPro;

public class Chair : MonoBehaviour
{
    public TextMeshProUGUI stateText;
    public GameObject menu;

    private MeshRenderer _mr;
    private GameObject _player;
    private PlayerController _playerController;
    private PlayerInputPress _playerInput;
    private Vector3 pos;
    private bool _isSit = false;

    void OnEnable()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (_mr == null)
            {
                _mr = GetComponent<MeshRenderer>();
                _player = GameObject.Find(PhotonNetwork.LocalPlayer.NickName);
                pos = new Vector3(transform.position.x, _player.transform.position.y, transform.position.z);
                _playerController = _player.GetComponent<PlayerController>();
                _playerInput = _player.GetComponent<PlayerInputPress>();
            }
            stateText.text = "��ȣ�ۿ� Ű�� ���� ���ڿ� ���� �� �ֽ��ϴ�.";
            _mr.enabled = true;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (_playerInput.interact && SitMenu.isGetUp)
            {
                if (!_isSit)
                {
                    _player.transform.position = pos;
                    _player.transform.rotation = Quaternion.Euler(0, 0, 0);
                    _mr.enabled = false;
                    _isSit = true;
                    menu.SetActive(true);
                    stateText.text = "��ȣ�ۿ� Ű�� ���� �Ͼ �� �ֽ��ϴ�.";
                }
                else
                {
                    _mr.enabled = true;
                    _isSit = false;
                    menu.SetActive(false);
                    stateText.text = "��ȣ�ۿ� Ű�� ���� ���ڿ� ���� �� �ֽ��ϴ�.";
                }
                _playerController.Interaction();
                _playerInput.interact = false;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            stateText.text = "";
            _mr.enabled = false;
        }
    }
}
