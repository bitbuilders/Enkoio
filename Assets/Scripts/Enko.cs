using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Enko : Singleton<Enko>
{
    [SerializeField] TextMeshProUGUI m_TouchInfo = null;
    [SerializeField] TextMeshProUGUI m_SwipeInfo = null;
    [SerializeField] [Range(0.0f, 2.0f)] float m_SwipeTime = 1.0f;
    [SerializeField] [Range(0.0f, 50.0f)] float m_SwipeDistance = 10.0f;
    [SerializeField] [Range(0.0f, 1.0f)] float m_VerticalLeeway = 0.25f;
    [SerializeField] [Range(0.0f, 1.0f)] float m_HorizontalLeeway = 0.25f;

    public bool RightSwipe { get; private set; }
    public bool LeftSwipe { get; private set; }
    public bool UpSwipe { get; private set; }
    public bool DownSwipe { get; private set; }

    Vector2 m_TouchPosition;
    Vector2 m_TouchDir;
    float m_TouchTime;

    private void Update()
    {
        UpdateSwipe();


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
            m_TouchTime = 0.0f;
        }

        m_TouchTime += Time.deltaTime;
        if (m_TouchTime >= m_SwipeTime) return;

        Vector2 dir = touch.position - m_TouchPosition;
        m_TouchDir = dir.normalized;
        if (dir.sqrMagnitude < m_SwipeDistance * m_SwipeDistance) return;

        if (Mathf.Abs(m_TouchDir.y) < m_VerticalLeeway)
        {
            float dot = Vector2.Dot(m_TouchDir, Vector2.right);

            // Right
            if (dot > 0.0f)
            {
                RightSwipe = true;
            }

            // Left
            if (dot < 0.0f)
            {
                LeftSwipe = true;
            }
        }
        if (Mathf.Abs(m_TouchDir.x) < m_HorizontalLeeway)
        {
            float dot = Vector2.Dot(m_TouchDir, Vector2.up);

            // Up
            if (dot > 0.0f)
            {
                UpSwipe = true;
            }

            // Down
            if (dot < 0.0f)
            {
                DownSwipe = true;
            }
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
            string debug = "T-Start: {0} | T-Dir: {1} | T-Time: {2}";
            m_TouchInfo.text = string.Format(debug, m_TouchPosition, m_TouchDir, m_TouchTime);
        }
        else m_TouchInfo.text = "Touch Info Text Not Set";
        
        if (m_SwipeInfo)
        {
            string swipe = "S-Right: {0} | S-Left: {1} | S-Up: {2} | S-Down: {3}";
            m_SwipeInfo.text = string.Format(swipe, RightSwipe, LeftSwipe, UpSwipe, DownSwipe);
        }
        else m_TouchInfo.text = "Swipe Info Text Not Set";
    }
}
