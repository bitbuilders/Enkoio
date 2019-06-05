using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sheep : MonoBehaviour
{
    public Vector2 endPosition = Vector2.zero;
    [Range(1.0f, 10.0f)]
    public float speed = 1.0f;

    void Start()
    {
    }

    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, endPosition, Time.deltaTime * speed);
    }

    float angle = 0.0f;
    IEnumerator Hop()
    {
        while(true)
        {
            angle += Time.deltaTime;
            angle %= 360.0f;

            yield return null;
        }
    }

    public void Collect()
    {
        StopAllCoroutines();
        Destroy(this.gameObject);
    }


}
