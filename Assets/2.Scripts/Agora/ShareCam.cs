using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;
using Photon.Pun;
using Agora.Rtc;

public class ShareCam : MonoBehaviourPunCallbacks
{
    private PhotonView PV;

    private string _appId = "";
    private string _channelName = "";
    private string _token = "";

    internal VideoSurface LocalView; // 내 화면

    internal static IRtcEngineEx RtcEngine; // 캠 RTC 엔진의 정보를 저장할 변수

    public uint Uid1;
    public uint Uid2 = 10;

    public Transform soundPanel;
    private uint _currUint = 0;

    public Sprite noCam;
    public TMP_FontAsset EliceDigital;
    private GameObject cMyName;

    public Sprite[] camButtonSprite;
    public Sprite[] voiceButtonSprite;
    public Sprite[] screenButtonSprite;
    
    private GameObject _shareMenu;
    private Image _camOnOffImage;
    private Image _voiceOnOffImg;
    private Image _screenOnOffImage;
    private bool _sharingScreen = false;
    private int _displayNum;

    void Start()
    {
        PV = GetComponent<PhotonView>();
        _appId = Singleton.Inst.Agora_AppID;
        _channelName = PhotonNetwork.CurrentRoom.Name.Split("#")[2];
        Uid1 = (uint)Random.Range(10000, 99999);
        InitEngine();
        SetupUI();
        JoinChannel();
    }


    void InitEngine()
    {
        RtcEngine = Agora.Rtc.RtcEngine.CreateAgoraRtcEngineEx();
        AgoraEventHandler handler = new AgoraEventHandler(this);
        RtcEngineContext context = new RtcEngineContext(_appId, 0, CHANNEL_PROFILE_TYPE.CHANNEL_PROFILE_LIVE_BROADCASTING, AUDIO_SCENARIO_TYPE.AUDIO_SCENARIO_DEFAULT);
        RtcEngine.Initialize(context);
        RtcEngine.InitEventHandler(handler);
    }

    void SetupUI()
    {
        #region 카메라 UI 세팅
        _camOnOffImage = GameObject.Find("CamOnOff").GetComponent<Image>();
        _voiceOnOffImg = GameObject.Find("VoiceOnOff").GetComponent<Image>();
        #endregion

        #region 화면공유 UI 세팅
        if (SceneManager.GetActiveScene().name.Contains("3"))
        {
            _screenOnOffImage = GameObject.Find("ScreenOnOff").GetComponent<Image>();
            _shareMenu = _screenOnOffImage.transform.GetChild(0).gameObject;
        }
        else
            GameObject.Find("ScreenOnOff").SetActive(false);
        #endregion
    }

    #region 채널 가입·탈퇴
    void JoinChannel()
    {
        RtcEngine.EnableAudio();
        RtcEngine.EnableVideo();
        RtcEngine.EnableLocalVideo(false);
        RtcEngine.EnableLocalAudio(false);
        RtcEngine.SetClientRole(CLIENT_ROLE_TYPE.CLIENT_ROLE_BROADCASTER);

        ChannelMediaOptions options = new ChannelMediaOptions();
        options.autoSubscribeAudio.SetValue(true);
        options.autoSubscribeVideo.SetValue(true);

        options.publishCameraTrack.SetValue(true);
        options.publishScreenTrack.SetValue(false);
        options.enableAudioRecordingOrPlayout.SetValue(true);
        options.clientRoleType.SetValue(CLIENT_ROLE_TYPE.CLIENT_ROLE_BROADCASTER);
        RtcEngine.JoinChannel(_token, _channelName, this.Uid1, options);
    }

    void ScreenShareJoinChannel()
    {
        ChannelMediaOptions options = new ChannelMediaOptions();
        options.autoSubscribeAudio.SetValue(false);
        options.autoSubscribeVideo.SetValue(false);
        options.publishCameraTrack.SetValue(false);
        options.publishScreenTrack.SetValue(true);
        options.enableAudioRecordingOrPlayout.SetValue(false);
#if UNITY_ANDROID || UNITY_IPHONE
            options.publishScreenCaptureAudio.SetValue(true);
            options.publishScreenCaptureVideo.SetValue(true);
#endif
        options.clientRoleType.SetValue(CLIENT_ROLE_TYPE.CLIENT_ROLE_BROADCASTER);
        var ret = RtcEngine.JoinChannelEx(_token, new RtcConnection(_channelName, this.Uid2), options);
        Debug.Log("JoinChannelEx returns: " + ret);
    }

    private void ScreenShareLeaveChannel()
    {
        RtcEngine.LeaveChannelEx(new RtcConnection(_channelName, Uid2));
    }
    #endregion

    #region 온·오프 버튼
    public void OnCam()
    {
        _camOnOffImage.sprite = camButtonSprite[1];
        LocalView.transform.GetChild(0).gameObject.SetActive(false); // 가림막 Off
        cMyName.SetActive(true);
        LocalView.SetEnable(true);
        RtcEngine.EnableLocalVideo(true);
    }

    public void OffCam()
    {
        _camOnOffImage.sprite = camButtonSprite[0];
        LocalView.transform.GetChild(0).gameObject.SetActive(true); // 가림막 On
        cMyName.SetActive(false);
        LocalView.SetEnable(false);
        RtcEngine.EnableLocalVideo(false);
    }

    public void OnAudio()
    {
        _voiceOnOffImg.sprite = voiceButtonSprite[1];
        RtcEngine.EnableLocalAudio(true);
    }

    public void OffAudio()
    {
        _voiceOnOffImg.sprite = voiceButtonSprite[0];
        RtcEngine.EnableLocalAudio(false);
    }
    #endregion

    #region 음량 조절
    void CamClick(BaseEventData eventData)
    {
        PointerEventData pointerData = (PointerEventData)eventData;
        if (pointerData.button.Equals(PointerEventData.InputButton.Right))
        {
            _currUint = uint.Parse(pointerData.pointerPress.name);
            soundPanel.parent.gameObject.SetActive(true);
            soundPanel.position = pointerData.position;
        }
    }

    public void CamBackground()
    {
        soundPanel.parent.gameObject.SetActive(false);
    }

    public void UserSound(float volume)
    {
        RtcEngine.AdjustUserPlaybackSignalVolume(_currUint, (int)volume);
    }

    public void UserMute(bool isOn)
    {
        RtcEngine.MuteRemoteAudioStream(_currUint, isOn);
    }
    #endregion

    #region 화면공유
    // 화면공유 아이콘을 누르면
    public void ControlScreenVideo()
    {
        if (!_sharingScreen) // 화면공유중이 아닐 때
        {
            SIZE t = new SIZE(360, 240);
            var info = RtcEngine.GetScreenCaptureSources(t, t, true);
            _displayNum = 0;
            for (int i = 0; i < info.Length; i++) if (info[i].sourceName.Contains("DISPLAY")) _displayNum++;
            if (_displayNum == 1)
                StartScreenShare(0);
            else if (_shareMenu.activeSelf)
            {
                for (int i = 0; i < 6; i++)
                    _shareMenu.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(i).gameObject.SetActive(false);
                _shareMenu.SetActive(false);
            }
            else
            {
                for (int i = 0; i < _displayNum; i++)
                    _shareMenu.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(i).gameObject.SetActive(true);
                _shareMenu.SetActive(true);
            }
        }
        else // 화면공유 중일 때
        {
            StopScreenShare();
        }
    }

    // 화면공유 시작
    public void StartScreenShare(int i)
    {
#if UNITY_ANDROID || UNITY_IPHONE
            var parameters2 = new ScreenCaptureParameters2();
            parameters2.captureAudio = true;
            parameters2.captureVideo = true;
            var nRet = RtcEngine.StartScreenCapture(parameters2);
            this.Log.UpdateLog("StartScreenCapture :" + nRet);
#else
        RtcEngine.StopScreenCapture();

        SIZE t = new SIZE(360, 240);
        var info = RtcEngine.GetScreenCaptureSources(t, t, true);
        // 전체 화면을 공유할 첫 번째 소스 ID를 가져옵니다.
        ulong dispId = info[i].sourceId;

        var nRet = RtcEngine.StartScreenCaptureByDisplayId((uint)dispId, default(Rectangle),
            new ScreenCaptureParameters { captureMouseCursor = true, frameRate = 30 });

        ScreenShareJoinChannel();

        // 로컬 뷰에 화면 트랙을 표시합니다.
        //setupLocalVideo(_displayNum, i);
        _screenOnOffImage.sprite = screenButtonSprite[1];
        _shareMenu.SetActive(false);
        // 화면 공유 상태를 업데이트합니다.
        _sharingScreen = true;
#endif
    }

    // 화면공유 중지
    private void StopScreenShare()
    {
        ScreenShareLeaveChannel();
        
        _screenOnOffImage.sprite = screenButtonSprite[0];
        _sharingScreen = false;
        RtcEngine.StopScreenCapture();
    }

    #endregion

    #region 이벤트
    internal class AgoraEventHandler : IRtcEngineEventHandler
    {
        private readonly ShareCam videoSample;
        private TextMeshProUGUI NoTR;
        private TextMeshProUGUI CNTMP;

        internal AgoraEventHandler(ShareCam sampleVideo)
        {
            videoSample = sampleVideo;
        }

        public override void OnJoinChannelSuccess(RtcConnection connection, int elapsed)
        {
            if (connection.localUid == videoSample.Uid1)
            {
                MakeVideoView(connection.localUid);
            }
            else if (connection.localUid == videoSample.Uid2)
            {
                MakeVideoView(0, "", VIDEO_SOURCE_TYPE.VIDEO_SOURCE_SCREEN);
            }
        }

        public override void OnLeaveChannel(RtcConnection connection, RtcStats stats)
        {
            if (connection.localUid == videoSample.Uid1)
            {
                DestroyVideoView(videoSample.Uid1.ToString());
            }
            else if (connection.localUid == videoSample.Uid2)
            {
                GameObject MS = GameObject.Find("mainScreen");
                Destroy(MS.GetComponent<VideoSurface>());
                MS.GetComponent<RawImage>().texture = null;
            }
        }

        public override void OnUserJoined(RtcConnection connection, uint uid, int elapsed)
        {
            if (uid != videoSample.Uid1)
            {
                var go = GameObject.Find(uid.ToString());
                if (ReferenceEquals(go, null))
                {
                    MakeVideoView(uid, videoSample._channelName, VIDEO_SOURCE_TYPE.VIDEO_SOURCE_REMOTE);
                }
            }
        }

        public override void OnUserEnableLocalVideo(RtcConnection connection, uint remoteUid, bool enabled) // 새로운 사용자가 들어왔을 때, 
        {
            if(remoteUid != videoSample.Uid2)
            {
                if (enabled)
                {
                    Transform go = GameObject.Find($"{remoteUid}").transform;
                    go.GetChild(0).gameObject.SetActive(false);
                    go.GetChild(1).gameObject.SetActive(true);
                    go.GetComponent<VideoSurface>().SetEnable(true);
                }
                else
                {
                    Transform go = GameObject.Find($"{remoteUid}").transform;
                    go.GetChild(0).gameObject.SetActive(true);
                    go.GetChild(1).gameObject.SetActive(false);
                    go.GetComponent<VideoSurface>().SetEnable(false);
                }
            }
        }

        public override void OnUserOffline(RtcConnection connection, uint uid, USER_OFFLINE_REASON_TYPE reason)
        {
            if (uid != videoSample.Uid1 && uid != videoSample.Uid2)
            {
                DestroyVideoView(uid.ToString());
            }
        }

        #region Video Render UI
        private void MakeVideoView(uint uid, string channelId = "", VIDEO_SOURCE_TYPE videoSourceType = VIDEO_SOURCE_TYPE.VIDEO_SOURCE_CAMERA)
        {
            VideoSurface videoSurface = new VideoSurface();

            if (videoSourceType == VIDEO_SOURCE_TYPE.VIDEO_SOURCE_CAMERA)
            {
                string _id = uid.ToString();
                videoSurface = MakeImageSurface(_id);
                videoSample.LocalView = videoSurface;
                videoSample.cMyName = videoSurface.transform.GetChild(1).gameObject;
                Singleton.Inst.localUid = _id;
                GameObject.Find(PhotonNetwork.LocalPlayer.NickName).GetComponent<PlayerController>().WaitNick(_id);
            }
            else if (videoSourceType == VIDEO_SOURCE_TYPE.VIDEO_SOURCE_SCREEN)
            {
                videoSurface = MakeScreenSurface();
            }
            else
            {
                if(uid == 10)
                {
                    videoSurface = MakeScreenSurface();
                }
                else
                {
                    videoSurface = MakeImageSurface(uid.ToString());
                }
            }
            if (ReferenceEquals(videoSurface, null)) return;
            // configure videoSurface
            videoSurface.SetForUser(channelId == "" ? 0 : uid, channelId, videoSourceType);
            videoSurface.SetEnable(true);
        }

        Color CamColor()
        {
            float[] xyz = { 0.529f, 0.529f, 0.529f };
            int nc = Random.Range(0, 2);
            for (int i = 0; i < 3; i++)
            {
                if (i != nc)
                {
                    xyz[i] = Random.Range(135.0f, 255.0f) / 255.0f;
                }
            }
            return new Color(xyz[0], xyz[1], xyz[2]);
        }

        private VideoSurface MakeScreenSurface()
        {
            GameObject MS = GameObject.Find("mainScreen");

            if (MS == null) return null;

            VideoSurface videoSurface = MS.AddComponent<VideoSurface>();
            return videoSurface;
        }

        private VideoSurface MakeImageSurface(string goName)
        {
            GameObject WC = new GameObject();
            GameObject goNo = new GameObject();
            GameObject goNoI = new GameObject();
            GameObject goNoN = new GameObject();
            GameObject goCN = new GameObject();

            if (WC == null || goNo == null || goNoI == null || goNoN == null || goCN == null) return null;

            WC.name = goName;
            goNo.name = "NoCam";
            goNoI.name = "NoCamImage";
            goNoN.name = "NCMyName";
            goCN.name = "CMyName";
            WC.AddComponent<RawImage>().uvRect = new Rect(0, 0, -1, -1);
            EventTrigger et = WC.AddComponent<EventTrigger>();
            et.AddListener(EventTriggerType.PointerClick, videoSample.CamClick);
            goNo.AddComponent<Image>();
            goNoI.AddComponent<Image>().sprite = videoSample.noCam;
            goNoN.AddComponent<TextMeshProUGUI>();
            goCN.AddComponent<TextMeshProUGUI>();

            GameObject Cam_Content = GameObject.Find("Cam_Content");
            if (Cam_Content != null)
            {
                WC.transform.SetParent(Cam_Content.transform, false);
                goNo.transform.SetParent(WC.transform, false);
                goNoI.transform.SetParent(goNo.transform, false);
                goNoN.transform.SetParent(goNoI.transform, false);
                goCN.transform.SetParent(WC.transform, false);
            }

            goNo.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
            goNo.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
            goNo.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
            goNo.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);

            RectTransform NRT = goNoI.GetComponent<RectTransform>();
            NRT.anchorMin = new Vector2(0, 0);
            NRT.anchorMax = new Vector2(1, 1);
            NRT.offsetMin = new Vector2(0, 0);
            NRT.offsetMax = new Vector2(0, 0);
            goNoI.GetComponent<Image>().color = CamColor();

            goNoN.GetComponent<RectTransform>().sizeDelta = new Vector2(170, 50);

            TextMeshProUGUI NoTR = goNoN.GetComponent<TextMeshProUGUI>();
            NoTR.fontSize = 36;
            NoTR.alignment = TextAlignmentOptions.Center;
            NoTR.font = videoSample.EliceDigital;

            RectTransform CNTR = goCN.GetComponent<RectTransform>();
            CNTR.anchorMin = new Vector2(1, 0);
            CNTR.anchorMax = new Vector2(1, 0);
            CNTR.pivot = new Vector2(1, 0);
            TextMeshProUGUI CNTMP = goCN.GetComponent<TextMeshProUGUI>();
            CNTMP.color = Color.black;
            CNTMP.fontSize = 25;
            CNTMP.alignment = TextAlignmentOptions.BottomRight;
            CNTMP.font = videoSample.EliceDigital;

            goCN.SetActive(false);

            VideoSurface videoSurface = WC.AddComponent<VideoSurface>();
            return videoSurface;
        }

        void DestroyVideoView(string name)
        {
            var go = GameObject.Find(name);
            if (!ReferenceEquals(go, null))
            {
                Destroy(go);
            }
        }
        #endregion
    }
    #endregion

    private void OnDestroy()
    {
        Debug.Log("OnDestroy");
        if (RtcEngine == null) return;
        RtcEngine.InitEventHandler(null);
        RtcEngine.LeaveChannel();
        RtcEngine.Dispose();
    }

    void OnApplicationQuit()
    {
        Debug.Log("OnQuit");
        if (RtcEngine == null) return;
        RtcEngine.InitEventHandler(null);
        RtcEngine.LeaveChannel();
        RtcEngine.Dispose();
    }
}