using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepMinigame : MonoBehaviour
{

    [SerializeField]
    BoxCollider2D spawnArea;

    [SerializeField]
    Sheep sheepPrefab;

    void Start()
    {
        Sheep sheep = GameObject.Instantiate(sheepPrefab, RandomPointInBounds2D(spawnArea.bounds), Quaternion.identity);

    }

    void Update()
    {
        
    }

    Vector2 RandomPointInBounds2D(Bounds bounds)
    {
        return new Vector2(Random.Range(bounds.min.x, bounds.max.x), Random.Range(bounds.min.y, bounds.max.y));
    }
}
