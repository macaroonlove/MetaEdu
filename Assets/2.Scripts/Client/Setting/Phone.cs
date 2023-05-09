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
        if (i.Equals(-1)) _currentMusic--;
        else if (i.Equals(1)) _currentMusic++;

        if (_currentMusic.Equals(-1)) _currentMusic = 3;
        else if (_currentMusic.Equals(4)) _currentMusic = 0;

        string song = SoundManager.Instance.InGame_BGM(_currentMusic);
        this.song.text = song.Split("!@#")[0];
        singer.text = song.Split("!@#")[1];
    }

    public void MusicPlayerPU(bool isOn)
    {
        SoundManager.Instance.BGMPause(isOn);
    }
    // map
   
}
