using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepMinigame : Singleton<SheepMinigame>
{

    [SerializeField]
    BoxCollider2D spawnArea = null;

    [SerializeField]
    BoxCollider2D endArea = null;

    [SerializeField]
    GameObject sheepPrefab = null;

    [SerializeField]
    [Range(10.0f, 20.0f)]
    float minigameTime = 10.0f;


    public int numCollected = 0;
    const int maxSheep = 6;
    private int numSheep = 0;

    float timeLeft = 0.0f;

    bool inGame = false;

    void Start()
    {
        StartGame();
    }

    void Update()
    {
        if (inGame)
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    Collider2D collider = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(touch.position));
                    if (collider != null && collider.CompareTag("Sheep"))
                    {
                        Sheep s = collider.GetComponent<Sheep>();
                        s.Collect();
                        numCollected++;
                    }
                }
            }
            timeLeft -= Time.deltaTime;
            if (timeLeft <= 0.0f)
            {
                EndGame();
            }
        }
    }

    IEnumerator SpawnSheep()
    {
        int numToSpawn = 0;
        Sheep s = null;
        while (true)
        {
            if (numSheep <= maxSheep)
            {
                numToSpawn = Random.Range(1, 3);
                for (int i = 0; i < numToSpawn; i++)
                {
                    s = GameObject.Instantiate(sheepPrefab).GetComponentInChildren<Sheep>();
                    s.transform.position = RandomPointInBounds2D(spawnArea.bounds);
                    s.endPosition = RandomPointInBounds2D(endArea.bounds);
                    s.speed = Random.Range(2.0f, 4.0f);
                }
                yield return new WaitForSeconds(1.0f);
            }
            else
            {
                yield return null;
            }
        }
    }

    public void StartGame()
    {
        timeLeft = minigameTime;
        numCollected = 0;
        numSheep = 0;
        StartCoroutine(SpawnSheep());
        inGame = true;
    }


    public void EndGame()
    {
        inGame = false;
        StopAllCoroutines();
    }

    Vector2 RandomPointInBounds2D(Bounds bounds)
    {
        return new Vector2(Random.Range(bounds.min.x, bounds.max.x), Random.Range(bounds.min.y, bounds.max.y));
    }
}
