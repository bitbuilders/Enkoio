using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public List<EnemyAI> m_enemies = new List<EnemyAI>();

    [SerializeField] GameObject[] m_enemyPrefabs = default;

    [SerializeField] LMckamey_Player m_Player = default;
    [SerializeField] float m_enemyRaduis = 2.0f;

    private int numOfEnemies = 0;


    private void Awake()
    {
        StartCoroutine(FindAllEnemiesInScene());
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
            print("" + distance + "  " + i);
            if (distance <= (m_enemyRaduis * m_enemyRaduis))
            {
                Health enemyHealth = m_enemies[i].GetComponent<Health>();
                if (enemyHealth)
                {
                    int damamge = m_Player.GetDamage();
                    eElementType playerElement = m_Player.GetElementType();
                    eElementType enemyElement = m_enemies[i].GetElementType();
                    enemyHealth.TakeDamage(damamge, playerElement, enemyElement);
                }
                if (!enemyHealth.IsAlive())
                    m_enemies.Remove(m_enemies[i]);
                return true;
            }
        }
        return false;
    }

    IEnumerator FindAllEnemiesInScene()
    {
        yield return new WaitForSeconds(3);
        var enemies = FindObjectsOfType<EnemyAI>();
        for (int i = 0; i < enemies.Length; i++)
        {
            print(enemies[i]);
            m_enemies.Add(enemies[i]);
        }
    }

}
