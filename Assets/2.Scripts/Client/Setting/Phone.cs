using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Phone : MonoBehaviour
{
    // music
    public TextMeshProUGUI song;
    public TextMeshProUGUI singer;

    void Start()
    {
        string song = SoundManager.Instance.InGame_BGM(false);
        this.song.text = song.Split("!@#")[0];
        singer.text = song.Split("!@#")[1];

        if (SceneManager.GetActiveScene().name.Equals("5.Goldenball"))
        {
            transform.GetChild(0).GetChild(2).GetComponent<Toggle>().interactable = false;
            transform.GetChild(0).GetChild(3).GetComponent<Toggle>().interactable = false;
        }
    }

    public void MusicPlayerPN(int i)
    {
        if (i.Equals(-1)) SoundManager.Instance.currentMusic--;
        else if (i.Equals(1)) SoundManager.Instance.currentMusic++;

        if (SoundManager.Instance.currentMusic.Equals(-1)) SoundManager.Instance.currentMusic = 3;
        else if (SoundManager.Instance.currentMusic.Equals(4)) SoundManager.Instance.currentMusic = 0;

        string song = SoundManager.Instance.InGame_BGM(true);
        this.song.text = song.Split("!@#")[0];
        singer.text = song.Split("!@#")[1];
    }

    public void MusicPlayerPU(bool isOn)
    {
        SoundManager.Instance.BGMPause(isOn);
    }
    // map
   
}
