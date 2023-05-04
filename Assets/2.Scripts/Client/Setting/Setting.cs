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
    private Transform _settingPanel;

    /* 기본 */
    private Transform _basicPanel;
    private TMP_InputField _changeName;
    private Image _enableName;
    private Image _disableName;
    private Slider _playerRot;

    /* 디스플레이 */
    private Transform _displayPanel;
    private TMP_Dropdown _resolution;
    private TMP_Dropdown _screenMode;
    private Slider _brightness;
    private TMP_Dropdown _shadow;
    private TMP_Dropdown _fvDis;
    private TMP_Dropdown _tvDis;

    private bool _isFull;
    private Volume _volume;
    private VolumeProfile _volumeProfile;
    private UnityEngine.Rendering.Universal.ColorAdjustments _colorAdj;
    private Light _directional;
    private CinemachineVirtualCamera _fCamera; // 1인칭
    private CinemachineVirtualCamera _tCamera; // 3인칭

    /* 소리 */
    private Transform _soundPanel;
    private Slider _bgm;
    private Slider _soundEffect;

    /* 기타 */
    private Color _whitebtn = new Color(1, 1, 1, 0.392f);
    private Color _blackbtn = new Color(0, 0, 0, 0.392f);

    void Start()
    {
        StartCoroutine("PostPetty");
        _settingPanel = GameObject.Find("Canvas").transform.GetChild(0).GetChild(6);
        _basicPanel = _settingPanel.GetChild(4).transform;
        _displayPanel = _settingPanel.GetChild(6).transform;
        _soundPanel = _settingPanel.GetChild(7).transform;
        // 기본
        _basicPanel.GetChild(2).GetChild(1).TryGetComponent(out _changeName);
        _basicPanel.GetChild(3).GetChild(1).TryGetComponent(out _enableName);
        _basicPanel.GetChild(3).GetChild(2).TryGetComponent(out _disableName);
        _basicPanel.GetChild(6).GetChild(1).TryGetComponent(out _playerRot);
        // 디스플레이
        _displayPanel.GetChild(1).GetChild(1).TryGetComponent(out _resolution);
        _displayPanel.GetChild(2).GetChild(1).TryGetComponent(out _screenMode);
        _displayPanel.GetChild(3).GetChild(1).TryGetComponent(out _brightness);
        _displayPanel.GetChild(4).GetChild(1).TryGetComponent(out _shadow);
        _displayPanel.GetChild(5).GetChild(1).TryGetComponent(out _fvDis);
        _displayPanel.GetChild(6).GetChild(1).TryGetComponent(out _tvDis);
        _isFull = _screenMode.value.Equals(0) ? true : false;
        GameObject.Find("Volume").TryGetComponent(out _volume);
        _volumeProfile = _volume.profile;
        _volumeProfile.TryGet(out _colorAdj);
        GameObject.Find("Directional").TryGetComponent(out _directional);
        GameObject.Find("3rd_Vcam").TryGetComponent(out _tCamera);

        // 소리
        _soundPanel.GetChild(1).GetChild(1).TryGetComponent(out _bgm);
        _soundPanel.GetChild(2).GetChild(1).TryGetComponent(out _soundEffect);
    }

    IEnumerator PostPetty()
    {
        yield return new WaitForSeconds(0.8f);
        _fCamera = GameObject.Find(PhotonNetwork.LocalPlayer.NickName).transform.GetChild(0).GetChild(0).GetComponent<CinemachineVirtualCamera>();
        yield return new WaitForSeconds(0.2f);
        _changeName.text = PhotonNetwork.LocalPlayer.NickName;
        SetEnablename(Singleton.Inst.showName);
        _playerRot.value = Singleton.Inst.rotSpeed;

        _resolution.value = Singleton.Inst.resolution;
        _screenMode.value = Singleton.Inst.screenMode;
        _brightness.value = Singleton.Inst.brightness;
        _shadow.value = Singleton.Inst.shadow;
        _fvDis.value = Singleton.Inst.fvDis;
        _tvDis.value = Singleton.Inst.tvDis;

        _bgm.value = Singleton.Inst.bgmSound;
        _soundEffect.value = Singleton.Inst.effectSound;
    }

    #region 기본
    public void SetName()
    {
        if(!_changeName.text.Equals(""))
        {
            var request = new UpdateUserDataRequest() { Data = new Dictionary<string, string>() { { "NickName", _changeName.text } } };
            PlayFabClientAPI.UpdateUserData(request, ChangeName, (Error) => _changeName.text = PhotonNetwork.LocalPlayer.NickName);
        }
    }

    void ChangeName(UpdateUserDataResult result)
    {
        PhotonNetwork.LocalPlayer.NickName = _changeName.text;
    }

    public void SetEnablename(int tf)
    {
        Singleton.Inst.showName = tf;
        if(tf.Equals(0))
        {
            _enableName.color = _whitebtn;
            _disableName.color = _blackbtn;
        }
        else
        {
            _enableName.color = _blackbtn;
            _disableName.color = _whitebtn;
        }
    }
    #endregion

    #region 디스플레이
    public void SetResolution()
    {
        if(_resolution.value.Equals(0))
        {
            Screen.SetResolution(1280, 720, _isFull);
            Singleton.Inst.resolution = 0;
        }
        else if (_resolution.value.Equals(1))
        {
            Screen.SetResolution(1920, 1080, _isFull);
            Singleton.Inst.resolution = 1;
        }
        else if (_resolution.value.Equals(2))
        {
            Screen.SetResolution(2560, 1440, _isFull);
            Singleton.Inst.resolution = 2;
        }
        else if (_resolution.value.Equals(3))
        {
            Screen.SetResolution(3840, 2160, _isFull);
            Singleton.Inst.resolution = 3;
        }
    }

    public void SetScreenMode()
    {
        if (_screenMode.value.Equals(1))
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
        _colorAdj.postExposure.value = _brightness.value * 0.1f;
        Singleton.Inst.brightness = (int)_brightness.value;
    }

    public void SetGraphicQuality(int i)
    {
        QualitySettings.SetQualityLevel(i);
    }

    public void SetShadow()
    {
        if (_shadow.value.Equals(0))
        {
            _directional.shadows = LightShadows.Soft;
            Singleton.Inst.shadow = 0;
        }
        else if (_shadow.value.Equals(1))
        {
            _directional.shadows = LightShadows.Hard;
            Singleton.Inst.shadow = 1;
        }
        else if (_shadow.value.Equals(2))
        {
            _directional.shadows = LightShadows.None;
            Singleton.Inst.shadow = 2;
        }
    }

    public void SetFVDis()
    {
        if (_fvDis.value.Equals(0))
        {
            _fCamera.m_Lens.FarClipPlane = 300;
            Singleton.Inst.fvDis = 0;
        }
        else if (_fvDis.value.Equals(1))
        {
            _fCamera.m_Lens.FarClipPlane = 500;
            Singleton.Inst.fvDis = 1;
        }
        else if (_fvDis.value.Equals(2))
        {
            _fCamera.m_Lens.FarClipPlane = 700;
            Singleton.Inst.fvDis = 2;
        }
        else if (_fvDis.value.Equals(3))
        {
            _fCamera.m_Lens.FarClipPlane = 1000;
            Singleton.Inst.fvDis = 3;
        }
    }

    public void SetTVDis()
    {
        if (_tvDis.value.Equals(0))
        {
            _tCamera.m_Lens.FarClipPlane = 300;
            Singleton.Inst.tvDis = 0;
        }
        else if (_tvDis.value.Equals(1))
        {
            _tCamera.m_Lens.FarClipPlane = 500;
            Singleton.Inst.tvDis = 1;
        }
        else if (_tvDis.value.Equals(2))
        {
            _tCamera.m_Lens.FarClipPlane = 700;
            Singleton.Inst.tvDis = 2;
        }
        else if (_tvDis.value.Equals(3))
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
