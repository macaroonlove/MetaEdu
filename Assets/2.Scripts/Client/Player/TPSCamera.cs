using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class TPSCamera : MonoBehaviour
{
    Renderer ObstacleRenderer;
    public Transform Character;

    void Start()
    {
        Character = GetComponent<CinemachineVirtualCamera>().Follow;
    }

    void Update()
    {
        float Distance = Vector3.Distance(transform.position, Character.position);
        Vector3 Direction = (Character.position - transform.position).normalized;

        RaycastHit hit;
        Debug.DrawRay(transform.position, Direction * Distance, Color.green);
        if (Physics.Raycast(transform.position, Direction, out hit, Distance))
        {
            // 2.맞았으면 Renderer를 얻어온다.
            ObstacleRenderer = hit.transform.gameObject.GetComponentInChildren<Renderer>();
            if (ObstacleRenderer != null)
            {
                // 3. Metrial의 Aplha를 바꾼다.
                Material Mat = ObstacleRenderer.material;
                Color matColor = Mat.color;
                matColor.a = 0.5f;
                Mat.color = matColor;
            }
        }
    }
}
