using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPSCamera : MonoBehaviour
{
    //private Camera _mainCamera;
    //private RaycastHit _rayHit;
    //private Ray _ray;
    //private Vector3 _middleVec = new Vector3(0.5f, 0.5f, 0f);
    //void Start()
    //{
    //    _mainCamera = Camera.main;
    //}

    //void LateUpdate()
    //{
    //    _ray = _mainCamera.ViewportPointToRay(_middleVec);

    //    if(Physics.Raycast(_ray, out _rayHit, 1 << 11))
    //    {

    //    }
    //}
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wall"))
            other.GetComponent<MeshRenderer>().enabled = false;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Wall"))
            other.GetComponent<MeshRenderer>().enabled = true;
    }
}
