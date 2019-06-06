using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : Singleton<CombatManager>
{
    public List<EnemyAI> m_enemies = new List<EnemyAI>();

    [SerializeField] Enko m_Player = default;
    [SerializeField] float m_enemyRaduis = 2.0f;
    [SerializeField] GameObject m_combatTileMap = default;
    [SerializeField] Transform m_GridTransform = default;

    private int numOfEnemies = 0;
    private TileMap m_tileMap = default;


    private void Awake()
    {
        StartCoroutine(FindAllEnemiesInScene());
    }


    public void NewBattle()
    {
        gameObject.SetActive(true);
        GameObject go = Instantiate(m_combatTileMap, m_GridTransform);
        m_tileMap = go.GetComponent<TileMap>();
        TileManager.Instance.HideTileMap();
    }

    public bool CheckIfEnemyTapped(Vector3 tapPosition)
    {
        Vector3 tapPosInWorldSpace = Camera.main.ScreenToWorldPoint(tapPosition);
        tapPosInWorldSpace.z = 0.0f;
        for (int i = 0; i < m_enemies.Count; i++)
        {
            float distance = Vector3.SqrMagnitude(m_enemies[i].transform.position - tapPosInWorldSpace);
            if (distance <= (m_enemyRaduis * m_enemyRaduis))
            {
                Health enemyHealth = m_enemies[i].GetComponent<Health>();
                if (enemyHealth)
                {
                    int damage = Enko.Instance.Damage;
                    eElementType playerElement = Enko.Instance.Element;
                    eElementType enemyElement = m_enemies[i].GetElementType();
                    enemyHealth.TakeDamage(damage, playerElement, enemyElement);
                }
                if (!enemyHealth.IsAlive())
                {
                    m_enemies.Remove(m_enemies[i]);
                    if(m_enemies.Count == 0)
                    {
                        EndBattle();
                    }
                }
                return true;
            }
        }
        return false;
    }

    public void EndBattle()
    {
        gameObject.SetActive(false);
        //Show the world map
        TileManager.Instance.ShowTileMap();
        //hide the combat map and set it to null
        m_tileMap.Hide();
        m_tileMap = null;
    }

    IEnumerator FindAllEnemiesInScene()
    {
        yield return new WaitForSeconds(3);
        var enemies = FindObjectsOfType<EnemyAI>();
        for (int i = 0; i < enemies.Length; i++)
        {
            m_enemies.Add(enemies[i]);
        }
    }

}
