using System.Collections;
using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    [SerializeField] private Material mtrlOrg;
    [SerializeField] private float first = -1.1f;
    [SerializeField] private float last = 1.1f;
    private Renderer _renderer;
    // Start is called before the first frame update
    void Start()
    {
        _renderer = GetComponent<SkinnedMeshRenderer>();
        StartCoroutine(Spawn());
    }

    IEnumerator Spawn()
    {
        while(first < last)
        {
            yield return null;
            first += 0.005f;
            _renderer.material.SetFloat("_Split_Value", first);
        }
        _renderer.material = mtrlOrg;
        enabled = false;
        yield break;
    }
}