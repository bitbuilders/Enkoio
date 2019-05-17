using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct ElementalColors
{
    public Color airColor;
    public Color fireColor;
    public Color waterColor;
    public Color earthColor;
}
public class LMckamey_Player : MonoBehaviour
{

    [SerializeField] int m_damage = 10;
    private eElemntType m_currentElementType = eElemntType.AIR;
    private int elementIndex = 0;
    private ElementalColors colors = default;
    private SpriteRenderer m_spriteRenderer = default;

    private void Start()
    {
        colors.airColor = new Color(0.5f, 1.0f, 1.0f);
        colors.fireColor = new Color(255f, 0f, 0f);
        colors.waterColor = new Color(0f, 0f, 255f);
        colors.earthColor = new Color(0.5f, 0.2f, 0f);
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        m_spriteRenderer.color = colors.airColor;
    }

    public void SwitchElement(bool swipedLeft)
    {
        m_currentElementType = GetNextEnum(swipedLeft);
        switch (m_currentElementType)
        {
            case eElemntType.FIRE:
                m_spriteRenderer.color = colors.fireColor;
                break;
            case eElemntType.WATER:
                m_spriteRenderer.color = colors.waterColor;
                break;
            case eElemntType.EARTH:
                m_spriteRenderer.color = colors.earthColor;
                break;
            case eElemntType.AIR:
                m_spriteRenderer.color = colors.airColor;
                break;
        }
        Debug.Log(m_currentElementType);
    }

    public eElemntType GetNextEnum(bool isIncrementing)
    {
        eElemntType nextElement = default;
        var enums = System.Enum.GetValues(typeof(eElemntType));
        if (isIncrementing)
        {
            elementIndex++;
            if (elementIndex >= enums.Length)
                elementIndex = 0;
        }
        else
        {
            elementIndex--;
            if (elementIndex < 0)
                elementIndex = enums.Length-1;
        }
        nextElement = (eElemntType)enums.GetValue(elementIndex);
        return nextElement;
    }

    public eElemntType GetElementType()
    {
        return m_currentElementType;
    }

    public int GetDamage()
    {
        return m_damage;
    }
}
