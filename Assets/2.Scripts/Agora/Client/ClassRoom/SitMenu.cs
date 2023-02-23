using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;
using TMPro;

public class SitMenu : MonoBehaviour, IPointerDownHandler, IDragHandler ,IEndDragHandler
{
    private PlayerController _playerController;
    private Animator _anim;
    [SerializeField]
    private Transform _targetTr; // 이동될 UI

    private Vector2 _startingPoint;
    private Vector2 _moveBegin;
    private Vector2 _moveOffset;

    public static bool isGetUp = true;
    public Canvas _dashBoard;
    public TextMeshProUGUI _stateText;
    private RectTransform _dashBoardRect;
    private Vector2 _vt = new Vector2(3.84f, 2.16f);

    [SerializeField]
    private TextMeshProUGUI _minimalizeText;

    private void Awake()
    {
        _anim = GetComponent<Animator>();
        _dashBoardRect = _dashBoard.GetComponent<RectTransform>();
        // 이동 대상 UI를 지정하지 않은 경우, 자동으로 부모로 초기화
        if (_targetTr == null)
            _targetTr = transform.parent;
    }

    private void OnEnable()
    {
        if(_playerController == null)
            _playerController = GameObject.Find(PhotonNetwork.LocalPlayer.NickName).GetComponent<PlayerController>();
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        _startingPoint = _targetTr.position;
        _moveBegin = eventData.position;
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        _moveOffset = eventData.position - _moveBegin;
        _targetTr.position = _startingPoint + _moveOffset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        float x = Mathf.Clamp(transform.position.x, 50, 1990);
        float y = Mathf.Clamp(transform.position.y, -70, 1050);
        _targetTr.position = new Vector2(x, y);
    }

    public void AnimTrigger(int num)
    {
        _playerController.AnimTrigger(num);
    }

    public void FullScreen(bool isOn)
    {
        if (isOn)
        {
            _dashBoard.renderMode = RenderMode.ScreenSpaceCamera;
            _dashBoard.sortingOrder = -1;
            _stateText.text = "";
            isGetUp = false;
        }
        else
        {
            _dashBoard.renderMode = RenderMode.WorldSpace;
            _dashBoardRect.sizeDelta = _vt;
            _dashBoardRect.anchoredPosition = Vector2.zero;
            _stateText.text = "상호작용 키를 눌러 일어날 수 있습니다.";
            isGetUp = true;
        }
    }

    public void Minimalize(bool isOn)
    {
        _anim.SetBool("isMin", isOn);
        _minimalizeText.text = isOn ? "최대화" : "최소화";
    }

    void OnDestroy()
    {
        isGetUp = true;
    }
}