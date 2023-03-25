using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Singleton : MonoBehaviour
{
    public static Singleton Inst = null;

    [Header("API ID")]
    public string Playfab_ID;
    public string Agora_AppID;
    public string Photon_AppID;
    public string localUid;
    [Header("Basic Setting")]
    public int showName;
    public float rotSpeed;
    [Header("Control Setting")]
    public bool controled;
    [Header("Display Setting")]
    public int resolution;
    public int screenMode;
    public int brightness;
    public int shadow;
    public int fvDis;
    public int tvDis;
    [Header("Sound Setting")]
    public float bgmSound;
    public float effectSound;
    [Header("Quiz MGR")]
    public string questions = "";
    public List<string> question = new();
    public int currSelect = 0;

    public bool isPatty = false;

    void Awake()
    {
        Application.targetFrameRate = 60;
        Resources.UnloadUnusedAssets();
        //PlayerPrefs.DeleteAll();
        if (ReferenceEquals(Inst, null))
        {
            Inst = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
            Destroy(this.gameObject);
    }

    void Start()
    {
        Load();
    }

    void Load()
    {
        try
        {
            showName = PlayerPrefs.GetInt("ShowName");
            rotSpeed = PlayerPrefs.GetFloat("RotSpeed");
            resolution = PlayerPrefs.GetInt("Resolution");
            screenMode = PlayerPrefs.GetInt("ScreenMode");
            brightness = PlayerPrefs.GetInt("Brightness") - 20;
            shadow = PlayerPrefs.GetInt("Shadow");
            fvDis = PlayerPrefs.GetInt("FVDis");
            tvDis = PlayerPrefs.GetInt("TVDis");
            bgmSound = PlayerPrefs.GetFloat("BGMSound");
            effectSound = PlayerPrefs.GetFloat("EffectSound");
        }
        catch
        {
            showName = 0;
            rotSpeed = 1.0f;
            resolution = 1;
            screenMode = 0;
            brightness = 10;
            shadow = 0;
            fvDis = 1;
            tvDis = 1;
            bgmSound = 1;
            effectSound = 1;
        }
        SoundManager.Instance.BGMVolume(bgmSound);
        SoundManager.Instance.EffectVolume(effectSound);
    }

    public void QuestionInit()
    {
        Transform quizContent = GameObject.Find("Quiz_Content").transform;
        string a = questions;
        question.Clear();
        for (int i = 1; i < a.Split("��").Length; i++)
        {
            string b = a.Split("��")[i];
            question.Add(b);
            Transform c = quizContent.GetChild(i - 1);
            c.gameObject.SetActive(true);
            c.GetChild(1).GetComponent<TextMeshProUGUI>().text = b.Split("��")[0];
        }
    }

    void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("ShowName", showName);
        PlayerPrefs.SetFloat("RotSpeed", rotSpeed);
        PlayerPrefs.SetInt("Resolution", resolution);
        PlayerPrefs.SetInt("ScreenMode", screenMode);
        PlayerPrefs.SetInt("Brightness", brightness + 20);
        PlayerPrefs.SetInt("Shadow", shadow);
        PlayerPrefs.SetInt("FVDis", fvDis);
        PlayerPrefs.SetInt("TVDis", tvDis);
        PlayerPrefs.SetFloat("BGMSound", bgmSound);
        PlayerPrefs.SetFloat("EffectSound", effectSound);
        PlayerPrefs.Save();
    }
}