using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    [SerializeField] private RectTransform mapImage;
    private Transform player;
    public Vector2 playerPos;
    private Vector2 prevPos;
    private Vector2 offset;

    void OnEnable()
    {
        if (ReferenceEquals(player, null))
        {
            player = GameObject.FindWithTag("Player").transform;
            playerPos = new Vector2(player.position.x, player.position.z);
            prevPos = playerPos;
        }
        mapImage.anchoredPosition = new Vector2(-730 + (155 - playerPos.x) * 6, 880 + (45 - playerPos.y) * 6);
        StartCoroutine(EnableMap());
    }

    IEnumerator EnableMap()
    {
        var wfs = new WaitForSeconds(0.3f);
        while (true)
        {
            playerPos.x = player.position.x;
            playerPos.y = player.position.z;
            offset = playerPos - prevPos;
            mapImage.anchoredPosition -= offset * 6f;
            prevPos = playerPos;
            yield return wfs;
        }
    }

    void OnDisable()
    {
        StopCoroutine(EnableMap());
    }
}