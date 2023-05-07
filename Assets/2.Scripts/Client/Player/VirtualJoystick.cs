using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using Photon.Pun;
using UnityEngine.InputSystem;

public class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public bool _lr = true;

    public float movementRange
    {
        get => m_MovementRange;
        set => m_MovementRange = value;
    }

    [FormerlySerializedAs("movementRange")]
    [SerializeField]
    private float m_MovementRange = 50;

    private Vector3 m_StartPos;
    private Vector2 m_PointerDownPos;
    private PlayerInputPress _pip;

    private void OnEnable()
    {
        m_StartPos = ((RectTransform)transform).anchoredPosition;
        GameObject.Find(PhotonNetwork.LocalPlayer.NickName).TryGetComponent(out _pip);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData == null)
            throw new System.ArgumentNullException(nameof(eventData));

        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponentInParent<RectTransform>(), eventData.position, eventData.pressEventCamera, out m_PointerDownPos);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData == null)
            throw new System.ArgumentNullException(nameof(eventData));

        if (Input.touchCount.Equals(1))
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponentInParent<RectTransform>(), eventData.position, eventData.pressEventCamera, out var position);
            var delta = position - m_PointerDownPos;

            delta = Vector2.ClampMagnitude(delta, movementRange);
            ((RectTransform)transform).anchoredPosition = m_StartPos + (Vector3)delta;

            var newPos = new Vector2(delta.x / movementRange, delta.y / movementRange);
            if (_lr)
                _pip.MoveInput(newPos);
            else
            {
                _pip.LookInput(delta);
                m_PointerDownPos = position;
            }
        }            
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        ((RectTransform)transform).anchoredPosition = m_StartPos;
        if (_lr)
        {
            _pip.MoveInput(Vector2.zero);
        }
        else
        {
            _pip.LookInput(Vector2.zero);
        }
    }
}