using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using TMPro;

public class Chair : MonoBehaviour
{
    public int tableGroup;
    public GameObject menu;

    private GameObject _mCanvas;
    private GameObject _jump;
    private GameObject _interact;
    private bool _chairRot = false;
    private MeshRenderer _mr;
    private GameObject _player;
    private PlayerController _playerController;
    private PlayerInputPress _playerInput;
    private Vector3 pos;
    private bool _isSit = false;

    void OnEnable()
    {
        if (SceneManager.GetActiveScene().name.Equals("3_1.ClassRoom"))
            _chairRot = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (ReferenceEquals(_mr, null))
            {
                _mr = GetComponent<MeshRenderer>();
                _player = GameObject.Find(PhotonNetwork.LocalPlayer.NickName);
                pos = new Vector3(transform.position.x, _player.transform.position.y, transform.position.z);
                _player.TryGetComponent(out _playerController);
                _player.TryGetComponent(out _playerInput);
                _mCanvas = GameObject.Find("Mobile_Canvas").transform.GetChild(1).gameObject;
                _jump = _mCanvas.transform.GetChild(1).gameObject;
                _interact = _mCanvas.transform.GetChild(2).gameObject;
            }
            _jump.SetActive(false);
            _interact.SetActive(true);
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
                    SitMenu.tableGroup = this.tableGroup;
                    _player.transform.SetPositionAndRotation(pos, _chairRot ? Quaternion.Euler(0, 0, 0) : transform.parent.rotation);
                    _mr.enabled = false;
                    _isSit = true;
                    menu.SetActive(true);
                    _mCanvas.SetActive(false);
                }
                else
                {
                    _mr.enabled = true;
                    _isSit = false;
                    _mCanvas.SetActive(true);
                    menu.SetActive(false);
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
            _jump.SetActive(true);
            _interact.SetActive(false);
            _mr.enabled = false;
        }
    }
}
