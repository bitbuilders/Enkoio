using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] int m_maxHealthPoints = 100;
    [SerializeField] int m_currentHealthPoints = 100;

    public bool IsAlive()
    {
        return m_currentHealthPoints > 0;
    }

    public void TakeDamage(int damage, eElementType attackingElement, eElementType defendingElement)
    {
        int modDamage = DetermindDamageDone(damage, attackingElement, defendingElement);
        m_currentHealthPoints -= modDamage;
        if (m_currentHealthPoints <= 0) Die();
    }

    public void Heal(int healPoints)
    {
        if ((m_currentHealthPoints + healPoints) > m_maxHealthPoints)
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

    private void Die()
    {
        Destroy(this.gameObject);
    }

    private int DetermindDamageDone(int baseDamage, eElementType attackingElement, eElementType defendingElement)
    {
        int modifiedDamage = baseDamage;
        switch (attackingElement)
        {
            case eElementType.FIRE:
                switch (defendingElement)
                {
                    case eElementType.WATER:
                        modifiedDamage = modifiedDamage / 2;
                        break;
                    case eElementType.EARTH:
                        modifiedDamage = modifiedDamage + (modifiedDamage / 2);
                        break;
                }
                break;
            case eElementType.WATER:
                switch (defendingElement)
                {
                    case eElementType.FIRE:
                        modifiedDamage = modifiedDamage / 2;
                        break;
                    case eElementType.AIR:
                        modifiedDamage = modifiedDamage + (modifiedDamage / 2);
                        break;
                }
                break;
            case eElementType.EARTH:
                switch (defendingElement)
                {
                    case eElementType.FIRE:
                        modifiedDamage = modifiedDamage + (modifiedDamage / 2);
                        break;
                    case eElementType.AIR:
                        modifiedDamage = modifiedDamage / 2;
                        break;
                }
                break;
            case eElementType.AIR:
                switch (defendingElement)
                {
                    case eElementType.WATER:
                        modifiedDamage = modifiedDamage + (modifiedDamage / 2);
                        break;
                    case eElementType.EARTH:
                        modifiedDamage = modifiedDamage / 2;
                        break;
                }
                break;
        }
        return modifiedDamage;
    }
}
