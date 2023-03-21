using System.Collections;
using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    [SerializeField] private Material mtrlOrg;
    [SerializeField] private float first = -1.1f;
    [SerializeField] private float last = 1.1f;
    private SkinnedMeshRenderer _renderer;

    void Start()
    {
        TryGetComponent(out _renderer);
        StartCoroutine(Spawn());
    }

    IEnumerator Spawn()
    {
        while(first < last)
        {
            yield return null;
            first += Time.deltaTime;
            _renderer.material.SetFloat("_Split_Value", first);
        }
        _renderer.material = mtrlOrg;
        enabled = false;
        yield break;
    }
}