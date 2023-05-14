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
    public PhotonView PV;

    private string _appId = "";
    private string _channelName = "";
    private string _token = "";

    internal VideoSurface LocalView; // 내 화면

    internal static IRtcEngineEx RtcEngine; // 캠 RTC 엔진의 정보를 저장할 변수

    public uint Uid1;
    public uint Uid2 = 10;

    public Transform soundPanel;
    private uint _currUint = 0;

    public Sprite camScreen;
    public Sprite camCircle;
    public TMP_FontAsset EliceDigital;
    private GameObject cMyName;

    public Button prevButton;
    public Button nextButton;
    private GameObject Cam_Content;
    private int _maxIndex = 0;
    private int _currentIndex = 0;
    private GameObject _shareMenu;
    private Toggle _sharingToggle;
    private bool _sharingScreen = false;
    private int _displayNum;

    void Start()
    {
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
        RtcEngine.AdjustRecordingSignalVolume(400);
    }

    void SetupUI()
    {
        Cam_Content = GameObject.Find("Cam_Content");

        #region 화면공유 UI 세팅
        if (SceneManager.GetActiveScene().name.Contains("3"))
        {
            GameObject.Find("Sharing").TryGetComponent(out _sharingToggle);
            _shareMenu = _sharingToggle.transform.GetChild(1).gameObject;
        }
        else
        {
            GameObject.Find("Sharing").TryGetComponent(out _sharingToggle);
            _sharingToggle.interactable = false;
        }
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

    public void SmallRoom(bool isOn)
    {
        OffCam();
        OffAudio();
        if(_sharingScreen)
            StopScreenShare();
        RtcEngine.LeaveChannel();
        Transform[] camList = Cam_Content.GetComponentsInChildren<Transform>();
        for(int i = 1; i < camList.Length; i++)
        {
            Destroy(camList[i].gameObject);
        }
        if (isOn)
        {
            _channelName = PhotonNetwork.CurrentRoom.Name.Split("#")[2] + SitMenu.tableGroup;
        }
        else
        {
            _channelName = PhotonNetwork.CurrentRoom.Name.Split("#")[2];
        }
        JoinChannel();
    }

    #endregion

    #region 온·오프 버튼
    public void OnCam()
    {
        LocalView.transform.GetChild(0).gameObject.SetActive(false); // 가림막 Off
        cMyName.SetActive(true);
        LocalView.SetEnable(true);
        RtcEngine.EnableLocalVideo(true);
    }

    public void OffCam()
    {
        LocalView.transform.GetChild(0).gameObject.SetActive(true); // 가림막 On
        cMyName.SetActive(false);
        LocalView.SetEnable(false);
        RtcEngine.EnableLocalVideo(false);
    }

    public void OnAudio()
    {
        RtcEngine.EnableLocalAudio(true);
    }

    public void OffAudio()
    {
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
    public void ControlScreenVideo(bool isOn)
    {
        if (!_sharingScreen && isOn) // 화면공유중이 아닐 때
        {
            SIZE t = new SIZE(360, 240);
            var info = RtcEngine.GetScreenCaptureSources(t, t, true);
            _displayNum = 0;
            for (int i = 0; i < info.Length; i++) if (info[i].sourceName.Contains("DISPLAY")) _displayNum++;
            if (_displayNum.Equals(1))
                StartScreenShare(0);
            else if (_shareMenu.activeSelf)
            {
                for (int i = 0; i < 6; i++)
                    _shareMenu.transform.GetChild(1).GetChild(1).GetChild(i).gameObject.SetActive(false);
                _shareMenu.SetActive(false);
            }
            else
            {
                for (int i = 0; i < _displayNum; i++)
                    _shareMenu.transform.GetChild(1).GetChild(1).GetChild(i).gameObject.SetActive(true);
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
#if UNITY_ANDROID
            var parameters2 = new ScreenCaptureParameters2();
            parameters2.captureAudio = true;
            parameters2.captureVideo = true;
            var nRet = RtcEngine.StartScreenCapture(parameters2);
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
        _shareMenu.SetActive(false);
        // 화면 공유 상태를 업데이트합니다.
        _sharingScreen = true;
#endif
    }

    // 화면공유 중지
    private void StopScreenShare()
    {
        ScreenShareLeaveChannel();
        
        _sharingScreen = false;
        RtcEngine.StopScreenCapture();
    }

    #endregion

    #region 이벤트
    internal class AgoraEventHandler : IRtcEngineEventHandler
    {
        private readonly ShareCam videoSample;
        private PlayerController _playerctl;

        internal AgoraEventHandler(ShareCam sampleVideo)
        {
            videoSample = sampleVideo;
        }

        public override void OnJoinChannelSuccess(RtcConnection connection, int elapsed)
        {
            if (connection.localUid.Equals(videoSample.Uid1))
            {
                MakeVideoView(connection.localUid);
            }
            else if (connection.localUid.Equals(videoSample.Uid2))
            {
                MakeVideoView(0, "", VIDEO_SOURCE_TYPE.VIDEO_SOURCE_SCREEN);
            }
        }

        public override void OnLeaveChannel(RtcConnection connection, RtcStats stats)
        {
            if (connection.localUid.Equals(videoSample.Uid1))
            {
                DestroyVideoView(videoSample.Uid1.ToString());
            }
            else if (connection.localUid.Equals(videoSample.Uid2))
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
                    go.parent.GetChild(1).gameObject.SetActive(true);
                    go.GetComponent<VideoSurface>().SetEnable(true);
                }
                else
                {
                    Transform go = GameObject.Find($"{remoteUid}").transform;
                    go.GetChild(0).gameObject.SetActive(true);
                    go.parent.GetChild(1).gameObject.SetActive(false);
                    go.GetComponent<VideoSurface>().SetEnable(false);
                }
            }
        }

        public override void OnUserOffline(RtcConnection connection, uint uid, USER_OFFLINE_REASON_TYPE reason)
        {
            if (uid != videoSample.Uid1 && uid != videoSample.Uid2)
            {
                DestroyVideoView(uid.ToString());
                videoSample._maxIndex--;
                CamRenewal();
            }
        }

        #region Video Render UI
        private void MakeVideoView(uint uid, string channelId = "", VIDEO_SOURCE_TYPE videoSourceType = VIDEO_SOURCE_TYPE.VIDEO_SOURCE_CAMERA)
        {
            if(ReferenceEquals(_playerctl, null))
                GameObject.Find(PhotonNetwork.LocalPlayer.NickName).TryGetComponent(out _playerctl);

            VideoSurface videoSurface; // = new VideoSurface();

            if (videoSourceType.Equals(VIDEO_SOURCE_TYPE.VIDEO_SOURCE_CAMERA))
            {
                string _id = uid.ToString();
                videoSurface = MakeImageSurface(_id);
                videoSample.LocalView = videoSurface;
                videoSample.cMyName = videoSurface.transform.parent.GetChild(1).gameObject;
                Singleton.Inst.localUid = _id;
                _playerctl.WaitNick(_id);
                videoSample._maxIndex++;
                CamRenewal();
            }
            else if (videoSourceType.Equals(VIDEO_SOURCE_TYPE.VIDEO_SOURCE_SCREEN))
            {
                videoSurface = MakeScreenSurface();
            }
            else
            {
                if(uid.Equals(10))
                {
                    videoSurface = MakeScreenSurface();
                }
                else
                {
                    _playerctl.WaitNick(videoSample.Uid1.ToString());
                    videoSurface = MakeImageSurface(uid.ToString());
                    videoSample._maxIndex++;
                    CamRenewal();
                }
            }
            if (ReferenceEquals(videoSurface, null)) return;
            // configure videoSurface
            videoSurface.SetForUser(channelId.Equals("") ? 0 : uid, channelId, videoSourceType);
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
            GameObject MS = null;
            if (SitMenu.groupChannel)
            {
                if(SitMenu.tableGroup.Equals(1))
                    MS = GameObject.Find("oneScreen");
                else if (SitMenu.tableGroup.Equals(2))
                    MS = GameObject.Find("twoScreen");
                else if (SitMenu.tableGroup.Equals(3))
                    MS = GameObject.Find("threeScreen");
                else if (SitMenu.tableGroup.Equals(4))
                    MS = GameObject.Find("fourScreen");
                else if (SitMenu.tableGroup.Equals(5))
                    MS = GameObject.Find("fiveScreen");
                else if (SitMenu.tableGroup.Equals(6))
                    MS = GameObject.Find("sixScreen");
            }
            else
            {
                MS = GameObject.Find("mainScreen");
            }

            if (ReferenceEquals(MS, null)) return null;

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

            if (ReferenceEquals(WC, null) || ReferenceEquals(goNo, null) || ReferenceEquals(goNoI, null) || ReferenceEquals(goNoN, null) || ReferenceEquals(goCN, null)) return null;

            WC.name = "Mask";
            goNo.name = goName;
            goNoI.name = "NoCam";
            goNoN.name = "NCMyName";
            goCN.name = "CMyName";
            WC.AddComponent<Image>().sprite = videoSample.camScreen;
            WC.AddComponent<Mask>();
            EventTrigger et = WC.AddComponent<EventTrigger>();
            if (goName.Equals(videoSample.Uid1))
            {
                et.AddListener(EventTriggerType.PointerClick, videoSample.CamClick);
            }
            goNo.AddComponent<RawImage>().uvRect = new Rect(0, 0, -1, -1);
            goNoI.AddComponent<Image>().sprite = videoSample.camCircle;
            goNoN.AddComponent<TextMeshProUGUI>();
            goCN.AddComponent<TextMeshProUGUI>();

            if (videoSample.Cam_Content != null)
            {
                WC.transform.SetParent(videoSample.Cam_Content.transform, false);
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

            goNoN.GetComponent<RectTransform>().sizeDelta = new Vector2(120, 50);

            TextMeshProUGUI NoTR = goNoN.GetComponent<TextMeshProUGUI>();
            NoTR.fontSize = 30;
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

            VideoSurface videoSurface = goNo.AddComponent<VideoSurface>();
            return videoSurface;
        }

        void CamRenewal()
        {
            if(videoSample._maxIndex > 5)
            {
                videoSample.prevButton.interactable = (videoSample._currentIndex <= 1) ? false : true;
                videoSample.nextButton.interactable = (videoSample._currentIndex >= videoSample._maxIndex) ? false : true;
            }
            else
            {
                videoSample.prevButton.interactable = false;
                videoSample.nextButton.interactable = false;
            }
        }

        void DestroyVideoView(string name)
        {
            var go = GameObject.Find(name).transform.parent.gameObject;
            if (!ReferenceEquals(go, null))
            {
                Destroy(go);
            }
        }
        #endregion
    }
    #endregion

    public void camButton(int num)
    {
        if (num.Equals(-1))
        {
            Cam_Content.transform.GetChild(_currentIndex).gameObject.SetActive(false);
            Cam_Content.transform.GetChild(_currentIndex+6).gameObject.SetActive(true);
            _currentIndex++;
        }
        else if (num.Equals(-2))
        {
            Cam_Content.transform.GetChild(_currentIndex + 5).gameObject.SetActive(false);
            Cam_Content.transform.GetChild(_currentIndex - 1).gameObject.SetActive(true);
            _currentIndex--;
        }
    }

    private void OnDestroy()
    {
        if (ReferenceEquals(RtcEngine, null)) return;
        RtcEngine.InitEventHandler(null);
        RtcEngine.LeaveChannel();
        RtcEngine.Dispose();
    }

    void OnApplicationQuit()
    {
        if (ReferenceEquals(RtcEngine, null)) return;
        RtcEngine.InitEventHandler(null);
        RtcEngine.LeaveChannel();
        RtcEngine.Dispose();
    }
}