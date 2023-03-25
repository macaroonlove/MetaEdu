using UnityEngine;

public class RotateObject : MonoBehaviour
{
    private Vector3 _mouseDownPos;
    private Vector3 _currentPos;
    private float _rotateSpeed = 0.5f;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _mouseDownPos = Input.mousePosition;
        }

        if (Input.GetMouseButton(0))
        {
            _currentPos = Input.mousePosition;
            Vector3 diff = _currentPos - _mouseDownPos;
            transform.Rotate(Vector3.up, -diff.x * _rotateSpeed, Space.World);
            _mouseDownPos = _currentPos;
        }
    }
}
