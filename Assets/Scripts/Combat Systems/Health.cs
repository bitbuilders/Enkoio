using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] int m_maxHealthPoints = 100;
    [SerializeField] int m_currentHealthPoints = 100;


    public void TakeDamage(int damage)
    {
        Debug.Log(gameObject.tag + "has taken " + damage + " damage!");
        m_currentHealthPoints -= damage;
    }

    public void Heal(int healPoints)
    {
        if((m_currentHealthPoints + healPoints) > m_maxHealthPoints)
        {
            ResetHealthToMax();
        }
        else
        {
            m_currentHealthPoints += healPoints;
        }
    }

    public void ResetHealthToMax()
    {
        m_currentHealthPoints = m_maxHealthPoints;
    }

    public void DetermineMaxHealthPoints()
    {

    }
}
