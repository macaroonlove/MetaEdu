using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.InputSystem;
using Photon.Pun;
using PlayFab;
using PlayFab.ClientModels;
using Cinemachine;
using TMPro;

public class Setting : MonoBehaviour
{
    private Transform SettingPanel;

    /* 기본 */
    private Transform basic;
    private TMP_InputField changeName;
    private Image enableName;
    private Image disableName;
    private Slider _playerRot;

    public InputActionAsset keySetting;

    /* 디스플레이 */
    private Transform DisplaySetting;
    private TMP_Dropdown Resolution;
    private TMP_Dropdown ScreenMode;
    private Slider Brightness;
    private TMP_Dropdown Shadow;
    private TMP_Dropdown FVDis;
    private TMP_Dropdown TVDis;

    private bool _isFull;
    private Volume _volume;
    private VolumeProfile _volumeProfile;
    private UnityEngine.Rendering.Universal.ColorAdjustments _colorAdj;
    private Light _directional;
    private CinemachineVirtualCamera _fCamera; // 1인칭
    private CinemachineVirtualCamera _tCamera; // 3인칭

    /* 사운드 */
    private Transform _soundPanel;
    private Slider _bgm;
    private Slider _soundEffect;

    void Start()
    {
        StartCoroutine("PostPetty");
        SettingPanel = GameObject.Find("Canvas").transform.GetChild(10);

        basic = SettingPanel.GetChild(4).transform;
        changeName = basic.GetChild(2).GetChild(1).GetComponent<TMP_InputField>();
        enableName = basic.GetChild(3).GetChild(1).GetComponent<Image>();
        disableName = basic.GetChild(3).GetChild(2).GetComponent<Image>();
        _playerRot = basic.GetChild(6).GetChild(1).GetComponent<Slider>();

        DisplaySetting = SettingPanel.GetChild(6).transform;
        Resolution = DisplaySetting.GetChild(1).GetChild(1).GetComponent<TMP_Dropdown>();
        ScreenMode = DisplaySetting.GetChild(2).GetChild(1).GetComponent<TMP_Dropdown>();
        Brightness = DisplaySetting.GetChild(3).GetChild(1).GetComponent<Slider>();
        Shadow = DisplaySetting.GetChild(4).GetChild(1).GetComponent<TMP_Dropdown>();
        FVDis = DisplaySetting.GetChild(5).GetChild(1).GetComponent<TMP_Dropdown>();
        TVDis = DisplaySetting.GetChild(6).GetChild(1).GetComponent<TMP_Dropdown>();

        _isFull = ScreenMode.value.Equals(0) ? true : false;
        _volume = GameObject.Find("Volume").GetComponent<Volume>();
        _volumeProfile = _volume.profile;
        _volumeProfile.TryGet(out _colorAdj);
        _directional = GameObject.Find("Directional").GetComponent<Light>();
        _tCamera = GameObject.Find("3rd_Vcam").GetComponent<CinemachineVirtualCamera>();

        _soundPanel = SettingPanel.GetChild(7).transform;
        _bgm = _soundPanel.GetChild(1).GetChild(1).GetComponent<Slider>();
        _soundEffect = _soundPanel.GetChild(2).GetChild(1).GetComponent<Slider>();
    }

    IEnumerator PostPetty()
    {
        yield return new WaitForSeconds(0.8f);
        _fCamera = GameObject.Find(PhotonNetwork.LocalPlayer.NickName).transform.GetChild(0).GetChild(0).GetComponent<CinemachineVirtualCamera>();
        yield return new WaitForSeconds(0.2f);
        changeName.text = PhotonNetwork.LocalPlayer.NickName;
        SetEnablename(Singleton.Inst.showName);
        _playerRot.value = Singleton.Inst.rotSpeed;

        Resolution.value = Singleton.Inst.resolution;
        ScreenMode.value = Singleton.Inst.screenMode;
        Brightness.value = Singleton.Inst.brightness;
        Shadow.value = Singleton.Inst.shadow;
        FVDis.value = Singleton.Inst.fvDis;
        TVDis.value = Singleton.Inst.tvDis;

        _bgm.value = Singleton.Inst.bgmSound;
        _soundEffect.value = Singleton.Inst.effectSound;
    }

    #region 기본
    public void SetName()
    {
        if(changeName.text != "")
        {
            var request = new UpdateUserDataRequest() { Data = new Dictionary<string, string>() { { "NickName", changeName.text } } };
            PlayFabClientAPI.UpdateUserData(request, ChangeName, (Error) => changeName.text = PhotonNetwork.LocalPlayer.NickName);
        }
    }

    void ChangeName(UpdateUserDataResult result)
    {
        PhotonNetwork.LocalPlayer.NickName = changeName.text;
    }

    public void SetEnablename(int tf)
    {
        Singleton.Inst.showName = tf;
        if(tf == 0)
        {
            enableName.color = new Color(1, 1, 1, 0.392f);
            disableName.color = new Color(0, 0, 0, 0.392f);
        }
        else
        {
            enableName.color = new Color(0, 0, 0, 0.392f);
            disableName.color = new Color(1, 1, 1, 0.392f);
        }
    }
    #endregion

    #region 디스플레이
    public void SetResolution()
    {
        if(Resolution.value == 0)
        {
            Screen.SetResolution(1280, 720, _isFull);
            Singleton.Inst.resolution = 0;
        }
        else if (Resolution.value == 1)
        {
            Screen.SetResolution(1920, 1080, _isFull);
            Singleton.Inst.resolution = 1;
        }
        else if (Resolution.value == 2)
        {
            Screen.SetResolution(2560, 1440, _isFull);
            Singleton.Inst.resolution = 2;
        }
        else if (Resolution.value == 3)
        {
            Screen.SetResolution(3840, 2160, _isFull);
            Singleton.Inst.resolution = 3;
        }
    }

    public void SetScreenMode()
    {
        if (ScreenMode.value == 1)
        {
            _isFull = false;
            Screen.SetResolution(Screen.width, Screen.height, false);
            Singleton.Inst.screenMode = 1;
        }
        else
        {
            _isFull = true;
            Screen.SetResolution(Screen.width, Screen.height, true);
            Singleton.Inst.screenMode = 0;
        }
    }

    public void SetBrightness()
    {
        _colorAdj.postExposure.value = Brightness.value * 0.1f;
        Singleton.Inst.brightness = (int)Brightness.value;
    }

    public void SetShadow()
    {
        if (Shadow.value == 0)
        {
            _directional.shadows = LightShadows.Soft;
            Singleton.Inst.shadow = 0;
        }
        else if (Shadow.value == 1)
        {
            _directional.shadows = LightShadows.Hard;
            Singleton.Inst.shadow = 1;
        }
        else if (Shadow.value == 2)
        {
            _directional.shadows = LightShadows.None;
            Singleton.Inst.shadow = 2;
        }
    }

    public void SetFVDis()
    {
        if (FVDis.value == 0)
        {
            _fCamera.m_Lens.FarClipPlane = 300;
            Singleton.Inst.fvDis = 0;
        }
        else if (FVDis.value == 1)
        {
            _fCamera.m_Lens.FarClipPlane = 500;
            Singleton.Inst.fvDis = 1;
        }
        else if (FVDis.value == 2)
        {
            _fCamera.m_Lens.FarClipPlane = 700;
            Singleton.Inst.fvDis = 2;
        }
        else if (FVDis.value == 3)
        {
            _fCamera.m_Lens.FarClipPlane = 1000;
            Singleton.Inst.fvDis = 3;
        }
    }

    public void SetTVDis()
    {
        if (TVDis.value == 0)
        {
            _tCamera.m_Lens.FarClipPlane = 300;
            Singleton.Inst.tvDis = 0;
        }
        else if (TVDis.value == 1)
        {
            _tCamera.m_Lens.FarClipPlane = 500;
            Singleton.Inst.tvDis = 1;
        }
        else if (TVDis.value == 2)
        {
            _tCamera.m_Lens.FarClipPlane = 700;
            Singleton.Inst.tvDis = 2;
        }
        else if (TVDis.value == 3)
        {
            _tCamera.m_Lens.FarClipPlane = 1000;
            Singleton.Inst.tvDis = 3;
        }
    }
    #endregion

    #region 사운드
    public void SetBGMVolume(float volume)
    {
        SoundManager.Instance.BGMVolume(volume);
        Singleton.Inst.bgmSound = volume;
    }

    public void SetEffectVolume(float volume)
    {
        SoundManager.Instance.EffectVolume(volume);
        Singleton.Inst.effectSound = volume;
    }
    #endregion
}
