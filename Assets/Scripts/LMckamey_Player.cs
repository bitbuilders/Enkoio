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
    private eElementType m_currentElementType = eElementType.AIR;
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
            case eElementType.FIRE:
                m_spriteRenderer.color = colors.fireColor;
                break;
            case eElementType.WATER:
                m_spriteRenderer.color = colors.waterColor;
                break;
            case eElementType.EARTH:
                m_spriteRenderer.color = colors.earthColor;
                break;
            case eElementType.AIR:
                m_spriteRenderer.color = colors.airColor;
                break;
        }
        Debug.Log(m_currentElementType);
    }

    public eElementType GetNextEnum(bool isIncrementing)
    {
        eElementType nextElement = default;
        var enums = System.Enum.GetValues(typeof(eElementType));
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
        nextElement = (eElementType)enums.GetValue(elementIndex);
        return nextElement;
    }

    public eElementType GetElementType()
    {
        return m_currentElementType;
    }

    public int GetDamage()
    {
        return m_damage;
    }
}
