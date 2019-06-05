using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] [Range(0.0f, 5.0f)] float m_Lifetime = 0.5f;
    [SerializeField] [Range(-5.0f, 5.0f)] float m_YKill = -0.8f;
    [SerializeField] [Range(-10.0f, 10.0f)] float m_Gravity = 1.0f;

    protected Rigidbody2D m_RigidBody;
    float m_Time;
    bool m_Thrown;

    private void Awake()
    {
        m_RigidBody = GetComponent<Rigidbody2D>();
    }

    virtual public void Use(Vector2 force)
    {
        m_RigidBody.gravityScale = m_Gravity;
        m_RigidBody.AddForce(force);
        m_Thrown = true;
    }

    private void Update()
    {
        if (!m_Thrown) return;

        m_Time += Time.deltaTime;
        if (m_Time > m_Lifetime || transform.position.y < m_YKill)
        {
            Break();
        }
    }

    virtual public void Break()
    {
        Destroy(gameObject);
    }
}
