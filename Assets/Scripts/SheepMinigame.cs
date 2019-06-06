using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepMinigame : MonoBehaviour
{

    [SerializeField]
    BoxCollider2D spawnArea = null;

    [SerializeField]
    BoxCollider2D endArea = null;

    [SerializeField]
    GameObject sheepPrefab = null;

    int numCollected = 0;

    void Start()
    {
        GameObject sheep = GameObject.Instantiate(sheepPrefab, RandomPointInBounds2D(spawnArea.bounds), Quaternion.identity);
        sheep.GetComponentInChildren<Sheep>().endPosition = RandomPointInBounds2D(endArea.bounds);
    }

    void Update()
    {
        
    }

    Vector2 RandomPointInBounds2D(Bounds bounds)
    {
        return new Vector2(Random.Range(bounds.min.x, bounds.max.x), Random.Range(bounds.min.y, bounds.max.y));
    }
}
