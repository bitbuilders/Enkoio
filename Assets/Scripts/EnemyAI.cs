using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] float m_attackTimer = 4.0f;
    [SerializeField] float m_missChance = 0.0f;
    [SerializeField] int m_damage = 10;
    [SerializeField] GameObject m_target;

    private float m_attackTimeElapsed = 0.0f;

    private void Start()
    {
        m_attackTimer = m_attackTimer + Random.Range(0f, m_attackTimer/2);
    }

    private void Update()
    {
        m_attackTimeElapsed += Time.deltaTime;
        if(m_attackTimeElapsed >= m_attackTimer)
        {
            //Attack();
        }
    }

    private void Attack()
    {
        m_target.GetComponent<Health>().TakeDamage(m_damage);
        m_attackTimeElapsed = 0.0f;
    }

}
