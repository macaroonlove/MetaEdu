using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ZoomInOut : MonoBehaviour
{
    public CinemachineVirtualCamera Ccamera;
    public float zoomSpeed = 0.5f;

    private Vector2 _tZeroPrev;
    private Vector2 _tOnePrev;
    private float _prevDelta;
    private float _currDelta;
    private float _deltaDiff;

    void Start()
    {
        
    }

    void Update()
    {
        if(Input.touchCount.Equals(2))
        {
            Touch tZero = Input.GetTouch(0);
            Touch tOne = Input.GetTouch(1);

            _tZeroPrev = tZero.position - tZero.deltaPosition;
            _tOnePrev = tOne.position - tOne.deltaPosition;

            _prevDelta = (_tZeroPrev - _tOnePrev).magnitude;
            _currDelta = (tZero.position - tOne.position).magnitude;

            _deltaDiff = _prevDelta - _currDelta;

            Ccamera.m_Lens.FieldOfView += _deltaDiff * zoomSpeed;
            Ccamera.m_Lens.FieldOfView = Mathf.Clamp(Ccamera.m_Lens.FieldOfView, 10, 60);
            //camera.GetCinemachineComponent<Cinemachine3rdPersonFollow>().ShoulderOffset
        }
    }
}
