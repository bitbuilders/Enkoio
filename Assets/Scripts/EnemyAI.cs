using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] float m_attackTimer = 4.0f;
    [SerializeField] float m_missChance = 0.0f;
    [SerializeField] int m_damage = 10;
    [SerializeField] GameObject m_target;
    [SerializeField] eElementType m_elemntType = default;

    private float m_attackTimeElapsed = 0.0f;

    public eElementType GetElementType()
    {
        return m_elemntType;
    }
    private void Awake()
    {
        m_attackTimer = m_attackTimer + Random.Range(0f, m_attackTimer / 2);
    }

    private void Update()
    {
        m_attackTimeElapsed += Time.deltaTime;
        if (m_attackTimeElapsed >= m_attackTimer)
        {
            //Attack();
        }
    }

    private void Attack()
    {
        LMckamey_Player player = m_target.GetComponent<LMckamey_Player>();
        if (player)
        {
            Health health = m_target.GetComponent<Health>();
            if (health)
            {
                health.TakeDamage(m_damage, m_elemntType, player.GetElementType());
                m_attackTimeElapsed = 0.0f;
            }
        }
    }


}
