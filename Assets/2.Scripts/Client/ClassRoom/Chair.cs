using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using TMPro;

public class Chair : MonoBehaviour
{
    public int tableGroup;
    public TextMeshProUGUI stateText;
    public GameObject menu;

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
            if (_mr == null)
            {
                _mr = GetComponent<MeshRenderer>();
                _player = GameObject.Find(PhotonNetwork.LocalPlayer.NickName);
                pos = new Vector3(transform.position.x, _player.transform.position.y, transform.position.z);
                _playerController = _player.GetComponent<PlayerController>();
                _playerInput = _player.GetComponent<PlayerInputPress>();
            }
            stateText.text = "상호작용 키를 눌러 의자에 앉을 수 있습니다.";
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
                    _player.transform.position = pos;
                    _player.transform.rotation = _chairRot ? Quaternion.Euler(0, 0, 0) : transform.parent.rotation;
                    _mr.enabled = false;
                    _isSit = true;
                    menu.SetActive(true);
                    stateText.text = "상호작용 키를 눌러 일어날 수 있습니다.";
                }
                else
                {
                    _mr.enabled = true;
                    _isSit = false;
                    menu.SetActive(false);
                    stateText.text = "상호작용 키를 눌러 의자에 앉을 수 있습니다.";
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
