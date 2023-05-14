using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Phone : MonoBehaviour
{
    // music
    public TextMeshProUGUI song;
    public TextMeshProUGUI singer;
    private int _currentMusic = 0;

    public void MusicPlayerPN(int i)
    {
        Debug.Log("¤µ");
        if (i.Equals(-1)) _currentMusic--;
        else if (i.Equals(1)) _currentMusic++;

        string song = SoundManager.Instance.InGame_BGM(_currentMusic);
        this.song.text = song.Split("!@#")[0];
        this.singer.text = song.Split("!@#")[1];
    }

    public void MusicPlayerPU(bool isOn)
    {
        SoundManager.Instance.BGMPause(isOn);
    }
    // map
   
}
