using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] [Range(0.0f, 5.0f)] float m_Lifetime = 0.5f;
    [SerializeField] [Range(-5.0f, 5.0f)] float m_YKill = -0.8f;
    [SerializeField] [Range(-10.0f, 10.0f)] float m_Gravity = 1.0f;
    [SerializeField] [Range(0.0f, 50.0f)] float m_Radius = 20.0f;
    [SerializeField] [Range(1.0f, 5.0f)] float m_Bounce = 1.5f;

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

        Vector2 newPos;
        Vector2 outNorm = GameBounds.OutOfBounds(transform.position, m_Radius, out newPos);
        if (outNorm != Vector2.zero)
        {
            m_RigidBody.position = newPos;
            Vector2 v = m_RigidBody.velocity;
            if (Mathf.Abs(outNorm.x) > 0.0f)
            {
                v.x = -outNorm.x * Mathf.Abs(m_RigidBody.velocity.x);
                v.x *= m_Bounce;
            }
            if (Mathf.Abs(outNorm.y) > 0.0f)
            {
                v.y = -outNorm.y * Mathf.Abs(m_RigidBody.velocity.y);
                v.y *= m_Bounce;
            }
            m_RigidBody.velocity = v;
        }

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
