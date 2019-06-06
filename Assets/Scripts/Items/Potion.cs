using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : Item
{
    [Space(10)]
    [Header("Potion")]
    [SerializeField] GameObject m_OrbTemplate = null;
    [SerializeField] [Range(0, 10)] int m_OrbCountMin = 4;
    [SerializeField] [Range(0, 10)] int m_OrbCountMax = 6;
    [SerializeField] [Range(0.0f, 2.0f)] float m_OffsetMin = 0.1f;
    [SerializeField] [Range(0.0f, 2.0f)] float m_OffsetMax = 0.3f;

    float m_Offset;
    int m_Orbs;

    private void Start()
    {
        m_Orbs = Random.Range(m_OrbCountMin, m_OrbCountMax + 1);
        m_Offset = Random.Range(m_OffsetMin, m_OffsetMax);
    }

    public override void Use(Vector2 force)
    {
        base.Use(force);
    }

    public override void Break()
    {
        for (int i = 0; i < m_Orbs; i++)
        {
            Vector2 offset = Random.insideUnitCircle.normalized * m_Offset;
            Instantiate(m_OrbTemplate, transform.position + (Vector3)offset, Quaternion.identity);
        }

        base.Break();
    }
}
