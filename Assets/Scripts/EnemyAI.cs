using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] float m_attackTimer = 4.0f;
    [SerializeField] int m_damage = 10;
    [SerializeField] eElementType m_elemntType = default;

    public bool Active { get; set; }

    private float m_attackTimeElapsed = 0.0f;

    public eElementType GetElementType()
    {
        return m_elemntType;
    }
    private void Awake()
    {
        m_attackTimer = m_attackTimer + Random.Range(0f, m_attackTimer / 2);
        Active = true;
    }

    private void Update()
    {
        if (!Active) return;

        m_attackTimeElapsed += Time.deltaTime;
        if (m_attackTimeElapsed >= m_attackTimer)
        {
            Attack();
            Health playerHealth = Enko.Instance.Health;
            if (playerHealth && !playerHealth.IsAlive())
            {
                CombatManager.Instance.EndBattle();
                playerHealth.ResetHealth();
            }
        }
    }

    private void Attack()
    {
        Health health = Enko.Instance.Health;
        if (health)
        {
            health.TakeDamage(m_damage, m_elemntType, Enko.Instance.Element);
            m_attackTimeElapsed = 0.0f;
        }
    }


}
