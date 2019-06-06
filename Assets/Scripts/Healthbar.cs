using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    [SerializeField] [Range(0.0f, 10.0f)] float m_Speed = 3.0f;

    Slider m_Slider;
    float m_Target;
    float m_Current;

    private void Start()
    {
        m_Slider = GetComponentInChildren<Slider>();
    }

    private void LateUpdate()
    {
        m_Target = (float)Enko.Instance.Health.Current / (float)Enko.Instance.Health.GetMaxHealth();
        m_Current = Mathf.Lerp(m_Current, m_Target, Time.deltaTime * m_Speed);

        m_Slider.value = m_Current;
    }
}
