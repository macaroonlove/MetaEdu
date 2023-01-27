using System;
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
    private uint secondChannelUid = 100;
    private RtcConnection _screenConn = new();

    internal VideoSurface LocalView; // 내 화면
    internal VideoSurface RemoteView; // 상대 화면
    internal VideoSurface ScreenView;

    internal static IRtcEngine RtcEngine; // 캠 RTC 엔진의 정보를 저장할 변수
    internal static IRtcEngineEx RtcEngineEx; // 스크린 RTC 엔진의 정보를 저장할 변수

    public Sprite noCam;
    public TMP_FontAsset EliceDigital;
    private GameObject cMyName;

    public Sprite[] screenButtonSprite;
    private GameObject myCam;
    private GameObject _shareMenu;
    private RawImage _mainScreen;
    private Image _screenOnOffImage;
    private bool _sharingScreen = false;
    private int _displayNum = 0;

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


    void Setup()
    {
        if (RtcEngine == null)
        {
            RtcEngine = Agora.Rtc.RtcEngine.CreateAgoraRtcEngine();
            RtcEngineContext context = new RtcEngineContext(_appId, 0, CHANNEL_PROFILE_TYPE.CHANNEL_PROFILE_LIVE_BROADCASTING, AUDIO_SCENARIO_TYPE.AUDIO_SCENARIO_DEFAULT);
            RtcEngine.Initialize(context);
        }
    }

    void SetupUI()
    {
        #region 카메라 UI 세팅
        myCam = GameObject.Find("MyCam");
        cMyName = myCam.transform.GetChild(1).gameObject;
        cMyName.SetActive(false);
        LocalView = myCam.GetComponent<VideoSurface>();
        LocalView.SetForUser(0, "", VIDEO_SOURCE_TYPE.VIDEO_SOURCE_CAMERA);
        #endregion

        #region 화면공유 UI 세팅
        if (SceneManager.GetActiveScene().name.Contains("3"))
        {
            _mainScreen = GameObject.Find("mainScreen").GetComponent<RawImage>();
            ScreenView = _mainScreen.GetComponent<VideoSurface>();
            _screenOnOffImage = GameObject.Find("ScreenOnOff").GetComponent<Image>();
            _shareMenu = _screenOnOffImage.transform.GetChild(0).gameObject;
        }
        else
            GameObject.Find("ScreenOnOff").SetActive(false);
        #endregion
    }

    #region 카메라
    void JoinChannel()
    {

        RtcEngine.SetClientRole(CLIENT_ROLE_TYPE.CLIENT_ROLE_BROADCASTER); // OnUserJoined 또는 OnUserOffline(USER_OFFLINE_BECOME_AUDIENCE) 이벤트 호출
        RtcEngine.EnableVideo();
        RtcEngine.EnableLocalVideo(false);
        RtcEngine.EnableLocalAudio(false);
        RtcEngine.JoinChannel(_token, _channelName);
    }

    public void Test()
    {
        string callid = "";
        RtcEngine.GetCallId(ref callid);
        Debug.Log(callid);
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
    void SetupEx()
    {
        if (RtcEngineEx == null)
        {
            RtcEngineEx = Agora.Rtc.RtcEngine.InstanceEx;
            RtcEngineContext context = new RtcEngineContext(_appId, 0, CHANNEL_PROFILE_TYPE.CHANNEL_PROFILE_LIVE_BROADCASTING, AUDIO_SCENARIO_TYPE.AUDIO_SCENARIO_DEFAULT);
            RtcEngineEx.Initialize(context);
        }
    }

    void JoinChannelEx()
    {
        RtcEngineEx.SetClientRole(CLIENT_ROLE_TYPE.CLIENT_ROLE_BROADCASTER);
        ChannelMediaOptions channelOptions = new ChannelMediaOptions();
        //channelOptions.publishScreenTrack.SetValue(true);
        //channelOptions.publishSecondaryScreenTrack.SetValue(true);
        _screenConn.channelId = _channelName;
        _screenConn.localUid = secondChannelUid;
        RtcEngineEx.JoinChannelEx(_token, _screenConn, channelOptions);
        VideoCanvas vc = new VideoCanvas();
        vc.sourceType = VIDEO_SOURCE_TYPE.VIDEO_SOURCE_REMOTE;
        RtcEngineEx.SetupRemoteVideoEx(vc, _screenConn);
    }

    // 화면공유 아이콘을 눌렀을 때
    public void ControlScreenVideo()
    {
        if (!_sharingScreen) // 화면공유중이 아닐 때
        {
            SIZE t = new SIZE(360, 240);
            var info = RtcEngine.GetScreenCaptureSources(t, t, true);
            Debug.Log(info);
            if (_displayNum == 0)
            {
                for (int i = 0; i < info.Length; i++) if (info[i].sourceName.Contains("DISPLAY")) _displayNum++;
            }
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
            DisableScreen();
            _screenOnOffImage.sprite = screenButtonSprite[0];
            _sharingScreen = false;
        }
    }

    // 화면공유를 할 디스플레이를 선택했을 때
    public void StartScreenShare(int i)
    {
        SetupEx();
        JoinChannelEx();
        EnableScreen(i);
    }

    void EnableScreen(int i)
    {
        updateChannelPublishOptions(true);

        SIZE t = new SIZE(360, 240);
        var info = RtcEngineEx.GetScreenCaptureSources(t, t, true);
        // 전체 화면을 공유할 첫 번째 소스 ID를 가져옵니다.
        ulong dispId = info[i].sourceId;

        RtcEngineEx.StartScreenCaptureByDisplayId(System.Convert.ToUInt32(dispId), new Rectangle(), default(ScreenCaptureParameters));

        //setupLocalVideo(_displayNum, i);
        _screenOnOffImage.sprite = screenButtonSprite[1];
        _shareMenu.SetActive(false);
        // 화면 공유 상태를 업데이트합니다.
        _sharingScreen = true;
    }

    // 채널 정보 업데이트
    private void updateChannelPublishOptions(bool publishMediaPlayer)
    {
        ChannelMediaOptions channelOptions = new ChannelMediaOptions();
        channelOptions.publishScreenTrack.SetValue(publishMediaPlayer);
        channelOptions.publishSecondaryScreenTrack.SetValue(publishMediaPlayer);
        RtcEngineEx.UpdateChannelMediaOptionsEx(channelOptions, _screenConn);
    }

    //private void setupLocalVideo(int displayNum, int i) // 화면공유
    //{
    //    ScreenView.SetEnable(true);
    //    ScreenView.SetForUser(0, "", VIDEO_SOURCE_TYPE.VIDEO_SOURCE_SCREEN);
    //    //PV.RPC(nameof(ScreenViewRPC), RpcTarget.OthersBuffered, displayNum, i);
    //}

    //[PunRPC]
    //void ScreenViewRPC(int displayNum, int i)
    //{
    //    ScreenView.SetEnable(true);
    //    ScreenView.SetForUser(secondChannelUid, _channelName, VIDEO_SOURCE_TYPE.VIDEO_SOURCE_REMOTE);
    //}

    void DisableScreen()
    {
        RtcEngineEx.StopScreenCapture();
        updateChannelPublishOptions(false);
        RtcEngineEx.LeaveChannelEx(_screenConn);
        if (ScreenView != null)
            ScreenView.SetEnable(false);
        //RtcEngineEx.Dispose();
        RtcEngineEx = null;
    }

    #endregion

    #region 이벤트
    void InitEventHandler()
    {
        AgoraEventHandler handler = new AgoraEventHandler(this, noCam, EliceDigital, PV, myCam);
        RtcEngine.InitEventHandler(handler);
    }

    internal class AgoraEventHandler : IRtcEngineEventHandler
    {
        private PhotonView PV;
        private GameObject _myCam;
        private Sprite noCam;
        private TMP_FontAsset EliceDigital;
        private readonly ShareCam videoSample;
        private TextMeshProUGUI NoTR;
        private TextMeshProUGUI CNTMP;

        internal AgoraEventHandler(ShareCam sampleVideo, Sprite NC, TMP_FontAsset ED, PhotonView PVO, GameObject Cam)
        {
            PV = PVO;
            _myCam = Cam;
            videoSample = sampleVideo;
            noCam = NC;
            EliceDigital = ED;
        }

        Color CamColor()
        {
            float[] xyz = { 0.529f, 0.529f, 0.529f };
            int nc = UnityEngine.Random.Range(0, 2);
            for (int i = 0; i < 3; i++)
            {
                if (i != nc)
                {
                    xyz[i] = UnityEngine.Random.Range(135.0f, 255.0f) / 255.0f;
                }
            }
            return new Color(xyz[0], xyz[1], xyz[2]);
        }

        public override void OnJoinChannelSuccess(RtcConnection connection, int elapsed)
        {
            Debug.Log(connection.localUid);
            if (connection.localUid != videoSample.secondChannelUid)
            {
                string _id = connection.localUid.ToString();
                _myCam.name = _id;
                Singleton.Inst.localUid = _id;
                GameObject.Find(PhotonNetwork.LocalPlayer.NickName).GetComponent<PlayerController>().WaitNick(_id);
                _myCam.transform.GetChild(0).GetChild(0).GetComponent<Image>().color = CamColor();
            }
            else
            {
                videoSample.ScreenView.SetEnable(true);
                videoSample.ScreenView.SetForUser(0, "", VIDEO_SOURCE_TYPE.VIDEO_SOURCE_SCREEN);
            }
        }

        public override void OnUserJoined(RtcConnection connection, uint remoteUid, int elapsed)
        {
            Debug.Log(connection.localUid + ", " + remoteUid);
            if (connection.localUid != videoSample.secondChannelUid)
            {
                videoSample.RemoteView = CamSurface(remoteUid.ToString());
                videoSample.RemoteView.SetForUser(remoteUid, connection.channelId, VIDEO_SOURCE_TYPE.VIDEO_SOURCE_REMOTE);
                //videoSample._remoteUid = remoteUid;
                videoSample.RemoteView.SetEnable(true);
            }
            else
            {
                videoSample.ScreenView.SetEnable(true);
                videoSample.ScreenView.SetForUser(connection.localUid, connection.channelId, VIDEO_SOURCE_TYPE.VIDEO_SOURCE_REMOTE);
            }
        }

        public override void OnRemoteVideoStateChanged(RtcConnection connection, uint remoteUid, REMOTE_VIDEO_STATE state, REMOTE_VIDEO_STATE_REASON reason, int elapsed)
        {
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
        RtcEngine.LeaveChannel();
        RtcEngine.EnableLocalVideo(false);
        if (RtcEngineEx != null)
        {
            DisableScreen();
        }
    }

    void OnApplicationQuit()
    {
        if (RtcEngine != null)
        {
            Leave();
            RtcEngine.Dispose();
            RtcEngine = null;
        }
        if (RtcEngineEx != null)
        {
            DisableScreen();
        }
    }
}