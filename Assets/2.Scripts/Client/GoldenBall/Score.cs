using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Score : MonoBehaviourPunCallbacks
{
    [Header("Text")]
    public TextMeshProUGUI player1Text;
    public TextMeshProUGUI player2Text;
    public TextMeshProUGUI player3Text;
    public TextMeshProUGUI player4Text;

    [Header("Effect")]
    public GameObject Attack;

    public PhotonView PV;
    public TextMeshProUGUI scoreText;
    public int currScore;

    private PlayerBattle _playerBattle;

    void Awake()
    {
        _playerBattle = GetComponent<PlayerBattle>(); 
    }
    new public void OnEnable()
    {
        if (SceneManager.GetActiveScene().name.Equals("5.Goldenball"))
        {
            PV = GetComponent<PhotonView>();
            currScore = 0;
            player1Text = GameObject.Find("Podium1").transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
            player2Text = GameObject.Find("Podium2").transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
            player3Text = GameObject.Find("Podium3").transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
            player4Text = GameObject.Find("Podium4").transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        }
    }

    private void Update()
    {
        if (transform.position.x == 3f)
        {
            scoreText = player1Text;
        }
        else if (transform.position.x == 1.15f)
        {
            scoreText = player2Text;
        }
        else if (transform.position.x == -1.15f)
        {
            scoreText = player3Text;
        }
        else if (transform.position.x == -3f)
        {
            scoreText = player4Text;
        }
    }

    [PunRPC]
    public void SendAnswer(string answer)
    {
        if (transform.position.x == 3f)
        {
            scoreText.text = answer;
        }
        else if (transform.position.x == 1.15f)
        {
            scoreText.text = answer;
        }
        else if (transform.position.x == -1.15f)
        { 
            scoreText.text = answer;
        }
        else if (transform.position.x == -3f)
        {
            scoreText.text = answer;
        }
    }

    [PunRPC]
    public void SendScore(int score)
    {
        if (transform.position.x == 3f)
        {
            scoreText.text = score.ToString();
        }
        else if (transform.position.x == 1.15f)
        {
            scoreText.text = score.ToString();
        }
        else if (transform.position.x == -1.15f)
        {
            scoreText.text = score.ToString();
        }
        else if (transform.position.x == -3f)
        {
            scoreText.text = score.ToString();
        }
    }

}