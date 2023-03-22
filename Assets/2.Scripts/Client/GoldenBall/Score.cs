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

    [PunRPC]
    public void SendAnswer(string answer)
    {
        if (transform.position.x == 3f)
        {
            scoreText = player1Text;
            scoreText.text = answer;
        }
        else if (transform.position.x == 1.15f)
        {
            scoreText = player2Text;
            scoreText.text = answer;
        }
        else if (transform.position.x == -1.15f)
        {
            scoreText = player3Text;
            scoreText.text = answer;
        }
        else if (transform.position.x == -3f)
        {
            scoreText = player4Text;
            scoreText.text = answer;
        }
    }

    [PunRPC]
    public void SendScore(int score)
    {
        if (transform.position.x == 3f)
        {
            scoreText = player1Text;
            scoreText.text = score.ToString();
        }
        else if (transform.position.x == 1.15f)
        {
            scoreText = player2Text;
            scoreText.text = score.ToString();
        }
        else if (transform.position.x == -1.15f)
        {
            scoreText = player3Text;
            scoreText.text = score.ToString();
        }
        else if (transform.position.x == -3f)
        {
            scoreText = player4Text;
            scoreText.text = score.ToString();
        }
    }

    [PunRPC]
    public void OnEffect()
    {
        if (transform.position.x == 3f)
        {
            GameObject f = Instantiate(Attack, new Vector3(2.07999992f, 1.5f, 1.12199998f), new Quaternion(0.0513652265f, 0.620391846f, 0.0645749494f, 0.779939532f));
            f.SetActive(true);
            Destroy(f, 2.2f);
        }
        else if (transform.position.x == 1.15f)
        {
            GameObject f = Instantiate(Attack, new Vector3(0.785000026f, 1.5f, 0.458999991f), new Quaternion(0.0278918445f, 0.838105083f, 0.0776553676f, 0.539232433f));
            f.SetActive(true);
            Destroy(f, 2.2f);
        }
        else if (transform.position.x == -1.15f)
        {
            GameObject f = Instantiate(Attack, new Vector3(-0.806999981f, 1.5f, 0.49000001f), new Quaternion(-0.0052672876f, 0.983378053f, 0.0823442042f, 0.161738858f));
            f.SetActive(true);
            Destroy(f, 2.2f);
        }
        else if (transform.position.x == -3f)
        {
            GameObject f = Instantiate(Attack, new Vector3(-2.0940001f, 1.5f, 1.18200004f), new Quaternion(-0.0270708576f, 0.990850031f, 0.0779453665f, -0.106807724f));
            f.SetActive(true);
            Destroy(f, 2.2f);
        }
    }
    public void OnAttackFlame()
    {
        GameObject a = Instantiate(Attack, new Vector3(0.108000003f, 2.46199989f, 3.23099995f),new Quaternion(-0.128541797f, -0.40415743f, 0.0574816652f, 0.903786302f));
        a.SetActive(true);
        Destroy(a, 2f);
    }
}