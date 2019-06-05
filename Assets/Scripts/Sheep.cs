using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sheep : MonoBehaviour
{
    public Vector2 startPosition = Vector2.zero;
    public Vector2 endPosition = Vector2.zero;
    public float speed = 0.0f;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    IEnumerator Hop()
    {
        yield return null;
    }

    public void Collect()
    {
        StopAllCoroutines();
        Destroy(this.gameObject);
    }
}
