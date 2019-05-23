using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public List<EnemyAI> m_enemies = default;

    [SerializeField] GameObject[] m_enemyPrefabs = default;

    [SerializeField] LMckamey_Player m_Player = default;
    [SerializeField] float m_enemyRaduis = 2.0f;
    [SerializeField] int m_numOfPotentialEnemies = 6;
    [SerializeField] int m_numOfTilesPerSide = 5;
    [SerializeField] TileMap m_tileMap = default;

    private int numOfEnemies = 0;
    private List<int> usedTiles = default;


    private void Awake()
    {
        GenerateEnemies();
    }

    private void GenerateEnemies()
    {
        numOfEnemies = Random.Range(0, m_numOfPotentialEnemies);
        for (int i = 0; i < numOfEnemies; i++)
        {
            int randomEnemyType = Random.Range(0, m_enemyPrefabs.Length);
            GameObject randomEnemy = Instantiate(m_enemyPrefabs[randomEnemyType]);
            DetermineEnemyPlacement(randomEnemy);
            m_enemies.Add(randomEnemy.GetComponent<EnemyAI>());
        }
    }

    private void DetermineEnemyPlacement(GameObject enemy)
    {
        var tileTypes = m_tileMap.GetTileTypes();
        int numOfTotalTiles = m_numOfTilesPerSide * m_numOfTilesPerSide;
        int enkoTilePlacement = (numOfTotalTiles / 2) + 1;
        int randomPlacement = Random.Range(0, numOfTotalTiles - 1);
        if(randomPlacement != enkoTilePlacement)
        {
            tileTypes[randomPlacement].Child = enemy;
        }
    }


    public bool CheckIfEnemyTapped(Vector3 tapPosition)
    {
        print("CheckIfEnemyTapped");
        Vector3 tapPosInWorldSpace = Camera.main.ScreenToWorldPoint(tapPosition);
        print("Tap Position in World Space" + tapPosInWorldSpace);
        tapPosInWorldSpace.z = 0.0f;  
        for (int i = 0; i < m_enemies.Count; i++)
        {
            float distance = Vector3.SqrMagnitude(m_enemies[i].transform.position - tapPosInWorldSpace);
            print(""+distance +"  "+ i);
            if(distance <= (m_enemyRaduis * m_enemyRaduis))
            {
                Health enemyHealth = m_enemies[i].GetComponent<Health>();
                enemyHealth.TakeDamage(m_Player.GetDamage(), m_Player.GetElementType(), m_enemies[i].GetElementType());
                if (!enemyHealth.IsAlive())
                    m_enemies.Remove(m_enemies[i]);
                return true;
            }
        }
        return false;
    }

}
