using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sheep : MonoBehaviour
{
    
    public Vector2 endPosition = Vector2.zero;
    public float speed = 1.0f;
    private Vector2 position = Vector2.zero;
    private float angle = 0.0f;
    private Vector2 offset = Vector2.zero;

    void Start()
    {
        position = transform.position;
    }

    void Update()
    {
        float dt = Time.deltaTime;
        position = Vector2.MoveTowards(position, endPosition, dt * speed);

        angle += dt * 1080.0f;
        angle %= 360.0f;
        offset.y = (Mathf.Acos(Mathf.Cos(angle * Mathf.Deg2Rad)) / 10.0f);

        transform.position = position + offset;

        if((position - endPosition).magnitude < .01f)
        {
            Destroy(transform.parent.gameObject);
        }
    }


    public void Collect()
    {
        Destroy(transform.parent.gameObject);
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
