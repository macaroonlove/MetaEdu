using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Score : MonoBehaviourPunCallbacks
{
    GoldenBall_backup goldenBall;

    [Header("Text")]
    public GameObject player1Text;
    public GameObject player2Text;
    public GameObject player3Text;
    public GameObject player4Text;
    [Header("Effect")]
    public GameObject flame1;
    public GameObject flame2;
    public GameObject flame3;
    public GameObject flame4;
    public GameObject Attack;

    public PhotonView PV;
    public TextMeshProUGUI scoreText;
    public int currScore;

    private GameObject _myFlame;
    new void OnEnable()
    {
        currScore = 0;
        PV = GetComponent<PhotonView>();
    }

    [PunRPC]
    public void SendAnswer(string answer)
    {
        scoreText.text = answer;
    }

    [PunRPC]
    public void SendScore(int score)
    {
        scoreText.text = score.ToString();
    }

    [PunRPC]
    public void OnEffect()
    {
        _myFlame.SetActive(true);
        Invoke("OffEffect", 2.5f);
    }

    public void OffEffect()
    {
        _myFlame.SetActive(false);
    }

    public void OnAttack()
    {
        Attack.SetActive(true);
        Invoke("OffAttak", 2f);
    }

    public void OffAttak()
    {
        Attack.SetActive(false);
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 7)
        {
            if (other.transform.position.x == -4.38f)
            {
                scoreText = player1Text.GetComponent<TextMeshProUGUI>();
                _myFlame = flame1;
            }
            else if (other.gameObject.transform.position.x == -2.48f)
            {
                scoreText = player2Text.GetComponent<TextMeshProUGUI>();
                _myFlame = flame2;
            }
            else if (other.gameObject.transform.position.x == -0.58f)
            {
                scoreText = player3Text.GetComponent<TextMeshProUGUI>();
                _myFlame = flame3;
            }
            else if (other.gameObject.transform.position.x == 1.32f)
            {
                scoreText = player4Text.GetComponent<TextMeshProUGUI>();
                _myFlame = flame4;
            }
        }
    }
}
