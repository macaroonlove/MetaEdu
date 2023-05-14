using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Sound
{
    Bgm,
    Effect,
    MaxCount
}

public class SoundManager : MonoBehaviour
{
    private AudioSource[] _audioSources = new AudioSource[(int)Sound.MaxCount];

    /* Singleton */
    private static SoundManager _instance;

    public static SoundManager Instance
    {
        get
        {
            if (ReferenceEquals(_instance, null))
            {
                _instance = new SoundManager();
            }
            return _instance;
        }
    }

    void Awake()
    {
        if (ReferenceEquals(_instance, null))
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        else
        {
            Destroy(this);
        }

        InitSound();
    }

    public void InitSound()
    {
        GameObject root = GameObject.Find("@SoundManager");

        if (ReferenceEquals(root, null))
        {
            root = new GameObject("@SoundManager");
            Object.DontDestroyOnLoad(root);

            for (int i = 0; i < (int)Sound.MaxCount; i++)
            {
                GameObject go = new GameObject(System.Enum.GetNames(typeof(Sound))[i]);
                _audioSources[i] = go.AddComponent<AudioSource>();
                go.transform.parent = root.transform;
            }

            _audioSources[(int)Sound.Bgm].loop = true;
        }
    }

    public void Play(AudioClip audioClip, Sound soundtype = Sound.Effect, float pitch = 1.0f)
    {
        if (ReferenceEquals(audioClip, null))
            return;

        AudioSource audioSource = _audioSources[(int)soundtype];
        audioSource.pitch = pitch;

        if (soundtype.Equals(Sound.Bgm))
        {
            if (audioSource.isPlaying)
                audioSource.Stop();

            audioSource.clip = audioClip;
            audioSource.Play();
        }

        if (soundtype.Equals(Sound.Effect))
        {
            audioSource.PlayOneShot(audioClip);
        }
    }

    public void PlayWithSpatialBlend(AudioSource audioSource, AudioClip audioClip, float pitch = 1.0f)
    {
        audioSource.pitch = pitch;
        audioSource.spatialBlend = 1;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.minDistance = 1;
        audioSource.maxDistance = 20;
        audioSource.PlayOneShot(audioClip);
    }

    public void BGMVolume(float volume)
    {
        _audioSources[(int)Sound.Bgm].volume = volume;
    }

    public void EffectVolume(float volume)
    {
        _audioSources[(int)Sound.Effect].volume = volume;
    }

    #region Sound Resources
    public AudioClip login;
    public AudioClip[] game;
    public AudioClip effect;
    public AudioClip[] goldenBall;
    public AudioClip[] battle;
    public int currentMusic = 0;
    public void Login_BGM()
    {
        this.Play(login, Sound.Bgm);
    }

    public void BGMPause(bool isOn)
    {
        AudioSource audioSource = _audioSources[0];
        if (isOn)
        {
            audioSource.UnPause();
        }
        else
        {
            audioSource.Pause();
        }        
    }

    public string InGame_BGM(bool i)
    {
        if(i)
            this.Play(game[currentMusic], Sound.Bgm);

        return game[currentMusic].name;
    }

    public void EffectExample()
    {
        this.Play(effect);
    }

    public void GoldenBallWrong()
    {
        this.Play(goldenBall[0]);
    }

    public void GoldenBallCorrect()
    {
        this.Play(goldenBall[1]);
    }

    public void FireballSound()
    {
        this.Play(battle[0], Sound.Effect, 2f);
    }
    #endregion
}