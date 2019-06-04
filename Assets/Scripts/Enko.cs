using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public struct ElementData
{
    public eElementType Element;
    public Color Color;
}

public class Enko : Singleton<Enko>
{
    [SerializeField] TextMeshProUGUI m_TouchInfo = null;
    [SerializeField] TextMeshProUGUI m_SwipeInfo = null;
    [SerializeField] AnimationCurve m_SwapCurve = null;
    [SerializeField] [Range(0, 100)] int m_Damage = 20;
    [SerializeField] [Range(0, 100)] int m_Health = 100;
    [SerializeField] [Range(0.0f, 10.0f)] float m_ElementSwapSpeed = 1.5f;
    [SerializeField] [Range(0.0f, 2.0f)] float m_SwipeTime = 1.0f;
    [SerializeField] [Range(0.0f, 50.0f)] float m_SwipeDistance = 10.0f;
    [SerializeField] [Range(0.0f, 1000.0f)] float m_TargetDistance = 150.0f;
    [SerializeField] [Range(0.0f, 1.0f)] float m_VerticalLeeway = 0.25f;
    [SerializeField] [Range(0.0f, 1.0f)] float m_HorizontalLeeway = 0.25f;
    [SerializeField] ElementData[] m_ElementData = null;

    public bool InCombat
    {
        get
        {
            return m_InCombat;
        }
        set
        {
            m_InCombat = value;
            if (!m_InCombat) m_Inventory.Close();
        }
    }

    public Vector2 SpritePosition { get { return m_SpriteRenderer.transform.position; } }
    public int Damage { get { return m_Damage; } }
    public int Health { get { return m_Health; } }
    public eElementType Element { get; private set; }
    public bool RightSwipe { get; private set; }
    public bool LeftSwipe { get; private set; }
    public bool UpSwipe { get; private set; }
    public bool DownSwipe { get; private set; }

    Dictionary<eElementType, Color> m_ElementColors;
    Inventory m_Inventory;
    SpriteRenderer m_SpriteRenderer;
    Vector2 m_TouchPosition;
    Vector2 m_TouchDir;
    float m_TouchTime;
    int m_CurrentElement;
    int m_ElementCount;
    int m_MaxHealth;
    bool m_InCombat;
    bool m_TargetTooFar;

    private void Start()
    {
        m_Inventory = GetComponentInChildren<Inventory>();
        m_Inventory.Init(gameObject);

        m_SpriteRenderer = GetComponentInChildren<SpriteRenderer>();

        m_ElementCount = Enum.GetNames(typeof(eElementType)).Length;

        m_ElementColors = new Dictionary<eElementType, Color>();
        foreach (ElementData data in m_ElementData)
        {
            m_ElementColors[data.Element] = data.Color;
        }

        m_CurrentElement = 0;
        Element = (eElementType)m_CurrentElement;
        m_MaxHealth = m_Health;
    }

    private void Update()
    {
        UpdateSwipe();

        UpdateElement();

        UpdateInventory();
    }

    public void Jump()
    {
        // TODO: Animation
    }

    void UpdateInventory()
    {
        if (!InCombat) return;

        if (UpSwipe)
        {
            m_Inventory.Open();
        }
        else if (DownSwipe)
        {
            m_Inventory.Close();
        }
    }

    void UpdateElement()
    {
        if (!RightSwipe && !LeftSwipe) return;

        Color prevColor = m_ElementColors[Element];
        if (RightSwipe)
        {
            m_CurrentElement--;
            if (m_CurrentElement < 0) m_CurrentElement = m_ElementCount - 1;
            Element = (eElementType)m_CurrentElement;
        }
        else if (LeftSwipe)
        {
            m_CurrentElement++;
            m_CurrentElement %= m_ElementCount;
            Element = (eElementType)m_CurrentElement;
        }

        StartCoroutine(ElementLerp(prevColor, m_ElementColors[Element], m_ElementSwapSpeed));
    }

    IEnumerator ElementLerp(Color start, Color end, float speed)
    {
        for (float i = 0.0f; i <= 1.0f; i += Time.deltaTime * speed)
        {
            float t = m_SwapCurve.Evaluate(i);
            Color c = Color.LerpUnclamped(start, end, t);
            m_SpriteRenderer.color = c;
            yield return null;
        }

        m_SpriteRenderer.color = end;
    }

    void UpdateSwipe()
    {
        SetDebugText();
        ResetSwipes();
        if (Input.touchCount == 0) return;

        Touch touch = Input.GetTouch(0);

        if (touch.phase == TouchPhase.Began)
        {
            m_TouchPosition = touch.position;
            Vector2 dirToSelf = Camera.main.WorldToScreenPoint(SpritePosition) - 
                (Vector3)m_TouchPosition;
            m_TargetTooFar = dirToSelf.sqrMagnitude > m_TargetDistance * m_TargetDistance;
            m_TouchTime = 0.0f;
        }

        m_TouchTime += Time.deltaTime;
        if (m_TouchTime >= m_SwipeTime || m_TargetTooFar) return;

        Vector2 dir = touch.position - m_TouchPosition;
        if (dir.sqrMagnitude < m_SwipeDistance * m_SwipeDistance) return;
        m_TouchDir = dir.normalized;

        if (Mathf.Abs(m_TouchDir.y) < m_VerticalLeeway)
        {
            float dot = Vector2.Dot(m_TouchDir, Vector2.right);
            
            if (dot > 0.0f) // Right
            {
                RightSwipe = true;
            }
            else if (dot < 0.0f) // Left
            {
                LeftSwipe = true;
            }
            m_TouchTime = m_SwipeTime;
        }
        if (Mathf.Abs(m_TouchDir.x) < m_HorizontalLeeway)
        {
            float dot = Vector2.Dot(m_TouchDir, Vector2.up);
            
            if (dot > 0.0f) // Up
            {
                UpSwipe = true;
            }
            else if (dot < 0.0f) // Down
            {
                DownSwipe = true;
            }
            m_TouchTime = m_SwipeTime;
        }
    }

    void ResetSwipes()
    {
        RightSwipe = false;
        LeftSwipe = false;
        UpSwipe = false;
        DownSwipe = false;
    }

    void SetDebugText()
    {
        if (m_TouchInfo)
        {
            string debug = "T-Start: {0} | T-Dir: {1} | Too Far: {2}";
            m_TouchInfo.text = string.Format(debug, m_TouchPosition, m_TouchDir, m_TargetTooFar);
        }
        
        if (m_SwipeInfo)
        {
            string swipe = "S-Right: {0} | S-Left: {1} | S-Up: {2} | S-Down: {3}";
            m_SwipeInfo.text = string.Format(swipe, RightSwipe, LeftSwipe, UpSwipe, DownSwipe);
        }
    }
}
