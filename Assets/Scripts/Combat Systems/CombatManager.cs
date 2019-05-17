using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public List<EnemyAI> m_enemies = default;

    [SerializeField] LMckamey_Player m_Player = default;
    [SerializeField] float m_enemyRaduis = 2.0f;

    private void Start()
    {
        
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
                enemyHealth.TakeDamage(m_Player.GetDamage());
                if (!enemyHealth.IsAlive())
                    m_enemies.Remove(m_enemies[i]);
                return true;
            }
        }
        return false;
    }

}
