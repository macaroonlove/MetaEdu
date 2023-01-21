using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Photon.Pun;
using Agora.Rtc;

public class ShareCam : MonoBehaviourPunCallbacks
{
    private PhotonView PV;

    private string _appId = "";
    private string _channelName = "";
    private string _token = "";
    private long _remoteUid;
    private uint _screenUid;

    internal VideoSurface LocalView; // 내 화면
    internal VideoSurface RemoteView; // 상대 화면
    internal VideoSurface ScreenView;

    internal static IRtcEngine RtcEngine; // 캠 RTC 엔진의 정보를 저장할 변수

    public Sprite noCam;
    public TMP_FontAsset EliceDigital;
    private GameObject cMyName;

    public Sprite[] screenButtonSprite;
    private GameObject myCam;
    private GameObject _shareMenu;
    private RawImage _mainScreen;
    private Image _screenOnOffImage;
    private bool _sharingScreen = false;
    private int _displayNum;

    void Start()
    {
        PV = GetComponent<PhotonView>();
        _appId = Singleton.Inst.Agora_AppID;
        _channelName = PhotonNetwork.CurrentRoom.Name;
        Setup();
        SetupUI();
        JoinChannel();
        InitEventHandler();
    }

    #region 카메라 공유
    void Setup()
    {
        if (RtcEngine == null)
        {
            RtcEngine = Agora.Rtc.RtcEngine.CreateAgoraRtcEngine();
            RtcEngineContext context = new RtcEngineContext(_appId, 0, CHANNEL_PROFILE_TYPE.CHANNEL_PROFILE_COMMUNICATION, AUDIO_SCENARIO_TYPE.AUDIO_SCENARIO_DEFAULT);
            RtcEngine.Initialize(context);
        }
    }

    void SetupUI()
    {
        myCam = GameObject.Find("MyCam");
        cMyName = myCam.transform.GetChild(1).gameObject;
        cMyName.SetActive(false);
        LocalView = myCam.GetComponent<VideoSurface>();
        LocalView.SetForUser(0, "", VIDEO_SOURCE_TYPE.VIDEO_SOURCE_CAMERA);

        if (SceneManager.GetActiveScene().name.Contains("3"))
        {
            _mainScreen = GameObject.Find("mainScreen").GetComponent<RawImage>();
            ScreenView = _mainScreen.GetComponent<VideoSurface>();
            _screenOnOffImage = GameObject.Find("ScreenOnOff").GetComponent<Image>();
            _shareMenu = _screenOnOffImage.transform.GetChild(0).gameObject;
        }
        else
        {
            GameObject.Find("ScreenOnOff").SetActive(false);
        }
    }

    void JoinChannel()
    {
        RtcEngine.SetClientRole(CLIENT_ROLE_TYPE.CLIENT_ROLE_BROADCASTER); // OnUserJoined 또는 OnUserOffline(USER_OFFLINE_BECOME_AUDIENCE) 이벤트 호출
        RtcEngine.EnableVideo();
        RtcEngine.EnableLocalVideo(false);
        RtcEngine.EnableLocalAudio(false);
        RtcEngine.JoinChannel(_token, _channelName);
    }

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

    #region 화면공유
    // 화면공유 아이콘을 눌렀을 때
    public void ControlScreenVideo()
    {
        if (!_sharingScreen) // 화면공유중이 아닐 때
        {
            SIZE t = new SIZE(360, 240);
            SIZE s = new SIZE(360, 240);
            var info = RtcEngine.GetScreenCaptureSources(t, s, true);
            _displayNum = 0;
            for (int i = 0; i < info.Length; i++) if (info[i].sourceName.Contains("DISPLAY")) _displayNum++;
            if (_displayNum == 1)
            {
                StartScreenShare(0);
            }
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
            RtcEngine.StopSecondaryScreenCapture();
            updateChannelPublishOptions(false);
            _screenOnOffImage.sprite = screenButtonSprite[0];
            _sharingScreen = false;
        }
    }
    // 화면공유를 할 디스플레이를 선택했을 때
    public void StartScreenShare(int i)
    {
        SIZE t = new SIZE(360, 240);
        SIZE s = new SIZE(360, 240);
        var info = RtcEngine.GetScreenCaptureSources(t, s, true);
        // 전체 화면을 공유할 첫 번째 소스 ID를 가져옵니다.
        ulong dispId = info[0].sourceId;

        ScreenCaptureConfiguration config = new();
        //config.displayId = System.Convert.ToUInt32(dispId);
        config.windowId = System.Convert.ToUInt32(dispId);
        config.screenRect = new Rectangle();
        config.isCaptureWindow = true;
        config.parameters = default;
        RtcEngine.StartSecondaryScreenCapture(config);

        // 화면의 일부를 공유하려면 Rectangle 클래스를 사용하여 화면 너비와 크기를 지정합니다.
        //int a = RtcEngine.StartScreenCaptureByWindowId(dispId, new Rectangle(), default(ScreenCaptureParameters));
        //Debug.Log(a);

        // 화면 트랙을 게시하고 로컬 비디오 트랙을 게시 취소합니다.
        updateChannelPublishOptions(true);

        // 로컬 뷰에 화면 트랙을 표시합니다.
        setupLocalVideo(_displayNum, i);
        _screenOnOffImage.sprite = screenButtonSprite[1];
        _shareMenu.SetActive(false);
        // 화면 공유 상태를 업데이트합니다.
        _sharingScreen = true;
    }

    // 채널 정보 업데이트
    private void updateChannelPublishOptions(bool publishMediaPlayer)
    {
        ChannelMediaOptions channelOptions = new ChannelMediaOptions();
        channelOptions.publishScreenTrack.SetValue(publishMediaPlayer); // 화면에서 캡처한 비디오를 게시할지 여부
        channelOptions.publishSecondaryScreenTrack.SetValue(publishMediaPlayer);
        //channelOptions.publishMicrophoneTrack.SetValue(true);
        channelOptions.publishCameraTrack.SetValue(!publishMediaPlayer);
        RtcEngine.UpdateChannelMediaOptions(channelOptions);
    }

    private void setupLocalVideo(int displayNum, int i) // 화면공유
    {
        ScreenRect(displayNum, i);
        ScreenView.SetForUser(0, "", VIDEO_SOURCE_TYPE.VIDEO_SOURCE_SCREEN);
        PV.RPC(nameof(ScreenViewRPC), RpcTarget.OthersBuffered, (int)_screenUid, displayNum, i);
    }

    [PunRPC]
    void ScreenViewRPC(int uid, int displayNum, int i)
    {
        ScreenRect(displayNum, i);
        ScreenView.SetForUser((uint)uid, _channelName, VIDEO_SOURCE_TYPE.VIDEO_SOURCE_REMOTE);
    }

    void ScreenRect(int displayNum, int i)
    {
        if (displayNum == 1)
        {
            _mainScreen.uvRect = new Rect(0, 0, 1, 1);
        }
        else if (displayNum == 2)
        {
            if (i == 0)
                _mainScreen.uvRect = new Rect(0, 0, 0.5f, 1);
            else if (i == 1)
                _mainScreen.uvRect = new Rect(0.5f, 0, 0.5f, 1);
        }
        else if (displayNum == 3)
        {
            if (i == 0)
                _mainScreen.uvRect = new Rect(0, 0, 0.333f, 1);
            else if (i == 1)
                _mainScreen.uvRect = new Rect(0.333f, 0, 0.333f, 1);
            else if (i == 2)
                _mainScreen.uvRect = new Rect(0.666f, 0, 0.333f, 1);
        }
    }
    #endregion

    #region 이벤트
    void InitEventHandler()
    {
        AgoraEventHandler handler = new AgoraEventHandler(this, noCam, EliceDigital, myCam);
        RtcEngine.InitEventHandler(handler);
    }

    internal class AgoraEventHandler : IRtcEngineEventHandler
    {
        private GameObject _myCam;
        private Sprite noCam;
        private TMP_FontAsset EliceDigital;
        private readonly ShareCam videoSample;
        private TextMeshProUGUI NoTR;
        private TextMeshProUGUI CNTMP;

        internal AgoraEventHandler(ShareCam sampleVideo, Sprite NC, TMP_FontAsset ED, GameObject Cam)
        {
            _myCam = Cam;
            videoSample = sampleVideo;
            noCam = NC;
            EliceDigital = ED;
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

        public override void OnJoinChannelSuccess(RtcConnection connection, int elapsed)
        {
            string _id = connection.localUid.ToString();
            _myCam.name = _id;
            videoSample._screenUid = connection.localUid;
            Singleton.Inst.localUid = _id;
            Debug.Log(GameObject.Find(PhotonNetwork.LocalPlayer.NickName));
            GameObject.Find(PhotonNetwork.LocalPlayer.NickName).GetComponent<PlayerController>().WaitNick(_id);

            _myCam.transform.GetChild(0).GetChild(0).GetComponent<Image>().color = CamColor();
        }

        public override void OnUserJoined(RtcConnection connection, uint remoteUid, int elapsed)
        {
            videoSample.RemoteView = CamSurface(remoteUid.ToString());
            videoSample.RemoteView.SetForUser(remoteUid, connection.channelId, VIDEO_SOURCE_TYPE.VIDEO_SOURCE_REMOTE);
            videoSample._remoteUid = remoteUid;
            videoSample.RemoteView.SetEnable(true);
        }

        public override void OnLocalAudioStateChanged(RtcConnection connection, LOCAL_AUDIO_STREAM_STATE state, LOCAL_AUDIO_STREAM_ERROR error) // EnableLocalAudio
        {
            Debug.Log("로컬 오디오" + connection.localUid);
        }

        public override void OnRemoteAudioStateChanged(RtcConnection connection, uint remoteUid, REMOTE_AUDIO_STATE state, REMOTE_AUDIO_STATE_REASON reason, int elapsed)
        {
            Debug.Log("리모트 오디오" + connection.localUid);
        }

        public override void OnRemoteVideoStateChanged(RtcConnection connection, uint remoteUid, REMOTE_VIDEO_STATE state, REMOTE_VIDEO_STATE_REASON reason, int elapsed)
        {
            Debug.Log(reason);
            if (reason.Equals(REMOTE_VIDEO_STATE_REASON.REMOTE_VIDEO_STATE_REASON_REMOTE_MUTED))
            {
                Transform go = GameObject.Find($"{remoteUid}").transform;
                go.GetChild(0).gameObject.SetActive(true);
                go.GetChild(1).gameObject.SetActive(false);

            }
            else if (reason.Equals(REMOTE_VIDEO_STATE_REASON.REMOTE_VIDEO_STATE_REASON_REMOTE_UNMUTED))
            {
                Transform go = GameObject.Find($"{remoteUid}").transform;
                go.GetChild(0).gameObject.SetActive(false);
                go.GetChild(1).gameObject.SetActive(true);
            }
        }

        public VideoSurface CamSurface(string goName)
        {
            GameObject WC = new GameObject();
            GameObject goNo = new GameObject();
            GameObject goNoI = new GameObject();
            GameObject goNoN = new GameObject();
            GameObject goCN = new GameObject();

            Vector2 camsize = new Vector2((int)((Screen.width / 1920) * 200), (int)((Screen.height / 1080) * 200));

            if (WC == null || goNo == null || goNoI == null || goNoN == null || goCN == null) return null;

            WC.name = goName;
            goNo.name = "NoCam";
            goNoI.name = "NoCamImage";
            goNoN.name = "NCMyName";
            goCN.name = "CMyName";
            WC.AddComponent<RawImage>().uvRect = new Rect(0, 0, -1, -1);
            WC.AddComponent<Button>();
            goNo.AddComponent<Image>();
            goNoI.AddComponent<Image>().sprite = noCam;
            goNoN.AddComponent<TextMeshProUGUI>();
            goCN.AddComponent<TextMeshProUGUI>();

            GameObject Cam_Content = GameObject.Find("Cam_Content");
            if (Cam_Content != null)
            {
                WC.transform.parent = Cam_Content.transform;
                goNo.transform.parent = WC.transform;
                goNoI.transform.parent = goNo.transform;
                goNoN.transform.parent = goNoI.transform;
                goCN.transform.parent = WC.transform;
            }

            WC.GetComponent<RectTransform>().sizeDelta = camsize;

            goNo.GetComponent<RectTransform>().sizeDelta = camsize;

            RectTransform NRT = goNoI.GetComponent<RectTransform>();
            NRT.sizeDelta = goNo.GetComponent<RectTransform>().sizeDelta = camsize;
            goNoI.GetComponent<Image>().color = CamColor();

            goNoN.GetComponent<RectTransform>().sizeDelta = new Vector2(170, 50);

            NoTR = goNoN.GetComponent<TextMeshProUGUI>();
            NoTR.fontSize = 36;
            NoTR.alignment = TextAlignmentOptions.Center;
            NoTR.font = EliceDigital;

            RectTransform CNTR = goCN.GetComponent<RectTransform>();
            CNTR.anchorMin = new Vector2(1, 0);
            CNTR.anchorMax = new Vector2(1, 0);
            CNTR.pivot = new Vector2(1, 0);
            CNTMP = goCN.GetComponent<TextMeshProUGUI>();
            CNTMP.color = Color.black;
            CNTMP.fontSize = 25;
            CNTMP.alignment = TextAlignmentOptions.BottomRight;
            CNTMP.font = EliceDigital;

            goCN.SetActive(false);

            VideoSurface videoSurface = WC.AddComponent<VideoSurface>();
            return videoSurface;
        }

        public override void OnUserOffline(RtcConnection connection, uint uid, USER_OFFLINE_REASON_TYPE reason)
        {
            GameObject go = GameObject.Find($"{uid}");
            if (go != null) Destroy(go);
            videoSample.RemoteView.SetEnable(false);
        }
    }
    #endregion



    public void Leave()
    {
        RtcEngine.LeaveChannel();
        RtcEngine.EnableLocalVideo(false);
        RtcEngine.DisableVideo();
        if (RemoteView != null)
            RemoteView.SetEnable(false);
        LocalView.SetEnable(false);
    }

    void OnDisable()
    {
        Leave();
    }

    void OnApplicationQuit()
    {
        if (RtcEngine != null)
        {
            Leave();
            RtcEngine.Dispose();
            RtcEngine = null;

        }
    }
}
