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
        StartCoroutine(Hop());
    }

    void Update()
    {
        transform.parent.position = Vector2.MoveTowards(transform.position, endPosition, Time.deltaTime * speed);
        Vector2 position2D = new Vector2(transform.position.x, transform.position.y);
        if((position2D - endPosition).magnitude < .01f)
        {
            Destroy(this.gameObject);
        }
    }

    float angle = 0.0f;
    IEnumerator Hop()
    {
        Vector3 offset = Vector3.zero;
        while(true)
        {
            angle += Time.deltaTime;
            angle %= 360.0f;
            offset.y = Mathf.Sin(angle);
            transform.position += offset;
            yield return null;
        }
    }

    public void Collect()
    {
        Destroy(this.gameObject);
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
