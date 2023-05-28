using UnityEngine;
using Cinemachine;
using PlayFab.Json;

public class QuestionWindow : MonoBehaviour
{
    private Camera _mainCamera;
    private CinemachineVirtualCamera _questCam;
    private PlayerInputPress _playerInput;
    private PlayerController _playerController;
    private GameObject _aggroUI;
    private bool _isWatch = false;

    void Start()
    {
        _mainCamera = Camera.main;
        transform.GetChild(0).GetComponent<Canvas>().worldCamera = _mainCamera;
        transform.GetChild(1).TryGetComponent(out _questCam);
        _aggroUI = transform.GetChild(0).GetChild(1).gameObject;
    }

    public void SharingQuestion()
    {
        QuestionManager.Inst.CallCloudScriptFunction("AddTitleData");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (ReferenceEquals(_playerInput, null))
            {
                other.TryGetComponent(out _playerInput);
                other.TryGetComponent(out _playerController);
            }
            _aggroUI.SetActive(true);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (_playerInput.interact)
            {
                if (!_isWatch)
                {
                    _aggroUI.SetActive(false);
                    _playerController.enabled = false;
                    _mainCamera.cullingMask = _mainCamera.cullingMask & ~(1 << 7) & ~(1 << 8) & ~(1 << 12);
                    _questCam.Priority = 30;
                    _isWatch = true;
                }
                else
                {
                    _aggroUI.SetActive(true);
                    _playerController.enabled = true;
                    _mainCamera.cullingMask = -1;
                    _questCam.Priority = 1;
                    _isWatch = false;
                }
                _playerInput.interact = false;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _aggroUI.SetActive(false);
        }
    }
}