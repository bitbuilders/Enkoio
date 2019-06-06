using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orb : MonoBehaviour
{
    [SerializeField] AnimationCurve m_SpeedRamp = null;
    [SerializeField] [Range(0.0f, 10.0f)] float m_RandomSizeMin = 1.0f;
    [SerializeField] [Range(0.0f, 10.0f)] float m_RandomSizeMax = 2.0f;
    [SerializeField] [Range(0.0f, 100.0f)] float m_PotencyBase = 10.0f;
    [SerializeField] [Range(0.0f, 10.0f)] float m_RampTimeMin = 4.0f;
    [SerializeField] [Range(0.0f, 10.0f)] float m_RampTimeMax = 7.0f;
    [SerializeField] [Range(0.0f, 10.0f)] float m_MaxSpeed = 3.0f;

    float m_Speed = 0.0f;
    float m_Ramp = 0.0f;
    float m_Time = 0.0f;
    float m_Potency = 0.0f;
    float m_Size = 0.0f;

    private void Start()
    {
        m_Size = Random.Range(m_RandomSizeMin, m_RandomSizeMax);
        m_Ramp = Random.Range(m_RampTimeMin, m_RampTimeMax);
        m_Potency = m_Size * m_PotencyBase;
    }

    void Update()
    {
        Vector2 dir = Enko.Instance.transform.position - transform.position;

        m_Time += Time.deltaTime / m_Ramp;
        m_Speed = (m_SpeedRamp.Evaluate(Mathf.Clamp01(m_Time)) * m_MaxSpeed);
        Vector2 velocity = dir.normalized * m_Speed;

        transform.position += (Vector3)velocity * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enko enko = collision.GetComponentInParent<Enko>();
        if (enko)
        {
            enko.Health.Heal((int)m_Potency);
            Destroy(gameObject);

            // TODO: Particle effect?

        }
    }
}
