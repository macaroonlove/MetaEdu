using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Cinemachine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerController : MonoBehaviourPunCallbacks
{
    PhotonView PV;

    private PlayerInput playerInput;
    private Animator anim;
    private CharacterController controller;
    private PlayerInputPress inputPress;
    private GameObject mainCamera;
    //private Camera MCC;
    private CinemachineVirtualCamera vcamOne;
    private CinemachineVirtualCamera vcamThree;
    //private TPSCamera tpsCamera;

    private const float threshold = 0.01f;
    private bool hasAnim;

    public bool grammaticalPersonState;

    [Header("플레이어 이동")]
    public float MoveSpeed = 2.0f;              // 걷기 속도
    public float RunSpeed = 5.0f;               // 뛰기 속도
    public float RotationSmoothTime = 0.12f;    // 회전 속도(3인칭)
    public float RotationSpeed = 1.0f;          // 회전 속도(1인칭)
    public float SpeedChangeRate = 10.0f;       // 걷다가 뛰거나
    public float JumpHeight = 1.5f;
    public float Gravity = -15.0f;
    public float JumpTimeout = 0.5f;
    public float FallTimeout = 0.15f;
    private float _speed;
    private float _animationBlend;
    private float _targetRotation = 0.0f;
    private float _rotationVelocity;
    private float _verticalVelocity;
    private float _terminalVelocity = 53.0f;
    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;

    [Header("플레이어 애니메이션")]
    private int _animIDSpeed;
    private int _animIDGrounded;
    private int _animIDJump;
    private int _animIDFreeFall;
    private int _animIDMotionSpeed;
    private int _animIDSitting;
    private int _animAsking;
    private int _animClap;
    private int _animShout;

    [Header("플레이어 소리")]
    public AudioClip LandingAudioClip;
    public AudioClip[] FootstepAudioClips;
    [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

    [Header("바닥")]
    public bool Ground = true;
    public float GroundOffset = -0.14f;
    public float GroundRadius = 0.28f;
    public LayerMask GroundLayers;

    [Header("시네머신")]
    public GameObject CinemachineCameraTarget;
    public float Topclamp = 70.0f;
    public float BottomClamp = -30.0f;
    public float CameraAngleOverride = 0.0f;
    public bool LockCameraPosition = false;
    private RaycastHit _hit;
    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;

    [Header("잡다")]
    private GameObject _chatting;
    private GameObject _roomListPanel;
    private GameObject _createRoomPanel;
    private GameObject _createQuizPanel;
    private GameObject _settingPanel;
    private TMP_InputField _nickName;
    private Slider _playerRot;

    [Header("의자")]
    private bool _isSit = false;
    //private Chair _chair;

    private bool IsCurrentDeviceMouse
    {
        get
        {
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
            return playerInput.currentControlScheme == "KeyboardMouse";
#else
    				return false;
#endif
        }
    }

    void Awake()
    {
        if (SceneManager.GetActiveScene().name.Equals("5.Goldenball"))
        {
            playerInput.enabled = false;
            enabled = false;
            Cursor.lockState = CursorLockMode.None;
        }
            

        PV = GetComponent<PhotonView>();

        Transform cv = GameObject.FindGameObjectWithTag("Canvas").transform;
        _nickName = cv.GetChild(10).GetChild(4).GetChild(2).GetChild(1).GetComponent<TMP_InputField>();
        _playerRot = cv.GetChild(10).GetChild(4).GetChild(6).GetChild(1).GetComponent<Slider>();
        _nickName.onEndEdit.AddListener(ChangeNick);
        _playerRot.onValueChanged.AddListener(ChangeRotSp);

        if (mainCamera == null && PV.IsMine)
        {
            mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            _chatting = GameObject.Find("ChatOnOff").transform.GetChild(0).GetChild(1).gameObject;
            _roomListPanel = cv.GetChild(0).gameObject;
            _settingPanel = cv.GetChild(10).gameObject;
            _createRoomPanel = cv.GetChild(8).gameObject;
            _createQuizPanel = cv.GetChild(9).gameObject;
            vcamOne = CinemachineCameraTarget.transform.GetChild(0).GetComponent<CinemachineVirtualCamera>();
            vcamThree = GameObject.Find("3rd_Vcam").GetComponent<CinemachineVirtualCamera>();
            //tpsCamera = vcamThree.GetComponent<TPSCamera>();
        }
    }

    #region 세팅
    #region 닉네임 변경
    void ChangeNick(string text)
    {
        if (text != "")
        {
            WaitNick(Singleton.Inst.localUid);
        }
    }

    public void WaitNick(string _id)
    {
        if (PV.IsMine)
        {
            //Debug.Log("Wait: " + _id);
            PV.RPC(nameof(RPCNick), RpcTarget.All, _id);
        }
    }

    [PunRPC]
    void RPCNick(string myCam)
    {
        //Debug.Log("RPC: " + myCam);
        StartCoroutine(AgoraNick(myCam));
    }

    IEnumerator AgoraNick(string myCam)
    {
        yield return new WaitForSeconds(1.5f);
        string pn = PV.IsMine ? PhotonNetwork.LocalPlayer.NickName : PV.Owner.NickName;
        transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = pn;
        try
        {
            Transform mct = GameObject.Find(myCam).transform;
            mct.GetChild(0).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = pn;
            mct.GetChild(1).GetComponent<TextMeshProUGUI>().text = pn;
        }
        catch
        {
            StartCoroutine(AgoraNick(myCam));
        }
        
    }
    #endregion

    #region 회전량 변경
    void ChangeRotSp(float value)
    {
        RotationSpeed = value;
        Singleton.Inst.rotSpeed = value;
    }
    #endregion
    #endregion

    void Start()
    {
        if (PV.IsMine)
        {
            Cursor.lockState = CursorLockMode.Locked;

            vcamThree.Follow = transform.GetChild(0);
            vcamThree.LookAt = transform.GetChild(0);
            //tpsCamera.enabled = true;

            _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;

            hasAnim = TryGetComponent(out anim);
            controller = GetComponent<CharacterController>();
            inputPress = GetComponent<PlayerInputPress>();
            playerInput = GetComponent<PlayerInput>();

            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;

            AssignmentAnimID();
        }
    }

    private void AssignmentAnimID()
    {
        _animIDSpeed = Animator.StringToHash("Speed");
        _animIDGrounded = Animator.StringToHash("Grounded");
        _animIDJump = Animator.StringToHash("Jump");
        _animIDFreeFall = Animator.StringToHash("FreeFall");
        _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        _animIDSitting = Animator.StringToHash("Sitting");
        _animAsking = Animator.StringToHash("Asking");
        _animClap = Animator.StringToHash("Clap");
        _animShout = Animator.StringToHash("Shout");
    }

    void Update()
    {
        if (PV.IsMine)
        {
            hasAnim = TryGetComponent(out anim);

            if (!_isSit)
            {
                GrammaticalPerson();
                JumpAndGravity();
                GroundedCheck();
                Move();
            }
        }
    }

    void LateUpdate()
    {
        if (PV.IsMine)
        {
            LimitInput();
            if (Cursor.lockState == CursorLockMode.Locked) CameraRotation();
        }
    }

    void LimitInput()
    {
        if (Input.anyKeyDown)
        {
            if (_settingPanel.activeSelf || _createRoomPanel.activeSelf || _createQuizPanel.activeSelf || _chatting.activeSelf)
            {
                if (playerInput.enabled)
                    playerInput.enabled = false;
            }
            else
            {
                if (!playerInput.enabled)
                    playerInput.enabled = true;
            }
        }
    }

    void GrammaticalPerson()
    {
        if(grammaticalPersonState)
        {
            if (inputPress.eye)
            {
                vcamThree.Priority = 10;
                vcamOne.Priority = 11;
                Camera.main.cullingMask = ~(1 << LayerMask.NameToLayer("Face"));
            }
            else
            {
                vcamThree.Priority = 11;
                vcamOne.Priority = 10;
                Camera.main.cullingMask = -1;
            }
        }
    }

    public void Interaction()
    {
        if (hasAnim)
        {
            if (_isSit)
            {
                anim.SetBool(_animIDSitting, false);
                _isSit = false;
            }
            else
            {
                anim.SetBool(_animIDSitting, true);
                _isSit = true;
            } 
        }
    }

    public void AnimTrigger(int num)
    {
        if (hasAnim)
        {
            if (num.Equals(0))
                anim.SetTrigger(_animAsking);
            else if(num.Equals(1))
                anim.SetTrigger(_animClap);
            else if(num.Equals(2))
                anim.SetTrigger(_animShout);
        }
    }

    void Move()
    {
        float targetSpeed = inputPress.run ? RunSpeed : MoveSpeed;

        // 입력이 없으면 목표속도를 0으로 설정
        if (inputPress.move == Vector2.zero || Cursor.lockState == CursorLockMode.None) targetSpeed = 0.0f;

        // magnitude: 벡터의 길이 반환
        float currentHorizontalSpeed = new Vector3(controller.velocity.x, 0.0f, controller.velocity.z).magnitude;

        float speedOffset = 0.1f;
        float inputMagnitude = inputPress.analogMovement ? inputPress.move.magnitude : 1f;

        // 목표속도로 가속 또는 감속
        if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);
            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        }
        else
        {
            _speed = targetSpeed;
        }

        _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
        if (_animationBlend < 0.01f) _animationBlend = 0f;

        // 입력방향 정규화
        Vector3 inputDirection = new Vector3(inputPress.move.x, 0.0f, inputPress.move.y).normalized;

        if (inputPress.eye)
        {
            if (inputPress.move != Vector2.zero)
            {
                inputDirection = transform.right * inputPress.move.x + transform.forward * inputPress.move.y;
            }

            controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
        }
        else
        {
            // 이동에 대한 입력이 있으면 플레이어가 이동할 때, 카메라를 기준으로 플레이어를 회전
            if (inputPress.move != Vector2.zero && Cursor.lockState == CursorLockMode.Locked)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);

                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }

            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
        }

        if (hasAnim)
        {
            anim.SetFloat(_animIDSpeed, _animationBlend);
            anim.SetFloat(_animIDMotionSpeed, inputMagnitude);
        }
    }

    void CameraRotation()
    {
        // sqrMagnitude: 2차원에서의 두 점 사이의 거리(빠름), Distance: 3차원에서의 두 점 사이의 거리(느림)
        // 입력이 있고, 카메라 위치가 고정되지 않았다면
        if (inputPress.look.sqrMagnitude >= threshold && !LockCameraPosition)
        {
            float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

            if (!inputPress.eye)
                _cinemachineTargetYaw += inputPress.look.x * RotationSpeed * deltaTimeMultiplier;
            else
                _rotationVelocity = inputPress.look.x * RotationSpeed * deltaTimeMultiplier;


            _cinemachineTargetPitch += inputPress.look.y * RotationSpeed * deltaTimeMultiplier;
        }

        if (!inputPress.eye)
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, Topclamp);

        if (inputPress.look == Vector2.zero)
            _rotationVelocity = 0.0f;

        if (!inputPress.eye)
            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride, _cinemachineTargetYaw, 0.0f);
        else
        {
            CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);
            transform.Rotate(Vector3.up * _rotationVelocity);
        }
    }

    static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    void JumpAndGravity()
    {
        if (Ground)
        {
            _fallTimeoutDelta = FallTimeout;

            if (hasAnim)
            {
                anim.SetBool(_animIDJump, false);
                anim.SetBool(_animIDFreeFall, false);
            }

            // 바닥에 닿았을 때 속도가 떨어지는 것을 멈추기
            if (_verticalVelocity < 0.0f)
            {
                _verticalVelocity = -2f;
            }

            if (inputPress.jump && _jumpTimeoutDelta <= 0.0f)
            {
                _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                if (hasAnim)
                {
                    anim.SetBool(_animIDJump, true);
                }
            }

            if (_jumpTimeoutDelta >= 0.0f)
            {
                _jumpTimeoutDelta -= Time.deltaTime;
            }
        }
        else
        {
            _jumpTimeoutDelta = JumpTimeout;

            if (_fallTimeoutDelta >= 0.0f)
            {
                _fallTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                if (hasAnim)
                {
                    anim.SetBool(_animIDFreeFall, true);
                }
            }

            inputPress.jump = false;
        }

        if (_verticalVelocity < _terminalVelocity)
        {
            _verticalVelocity += Gravity * Time.deltaTime;
        }
    }

    void GroundedCheck()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundOffset, transform.position.z);
        Ground = Physics.CheckSphere(spherePosition, GroundRadius, GroundLayers, QueryTriggerInteraction.Ignore);

        if (hasAnim)
        {
            anim.SetBool(_animIDGrounded, Ground);
        }
    }

    private void OnFootstep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f && PV.IsMine)
        {
            if (FootstepAudioClips.Length > 0)
            {
                var index = Random.Range(0, FootstepAudioClips.Length);
                AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(controller.center), FootstepAudioVolume);
            }
        }
    }

    private void OnLand(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f && PV.IsMine)
        {
            AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(controller.center), FootstepAudioVolume);
        }
    }

    //private void OnDestroy()
    //{
    //    tpsCamera.enabled = false;
    //}

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Potal") && PV.IsMine)
        {
            _roomListPanel.SetActive(true);
        }
        else if (other.CompareTag("Fog") && PV.IsMine)
        {
            RoomChangeManager.Instance.RoomOut("Battle#4.Battle", 20, 2);
        }
        else if (other.CompareTag("Bus") && PV.IsMine)
        {
            RoomChangeManager.Instance.RoomOut("null", 4, 3);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Potal" && PV.IsMine)
        {
            _roomListPanel.SetActive(false);
            _createRoomPanel.SetActive(false);
        }
    }
}
