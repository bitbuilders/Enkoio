using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TileManager : Singleton<TileManager>
{
    [SerializeField] TileMap m_Overworld = null;
    [SerializeField] [Range(0.0f, 10.0f)] float m_DebugIterationDistance = 1.0f;
    [SerializeField] [Range(0.0f, 10.0f)] float m_DebugIterationTime = 3.0f;
    [SerializeField] bool m_Debug = false;
    [SerializeField] TextMeshProUGUI m_FromCenterText = null;

    Vector2 m_LastTilePos;
    Vector2 m_Distance;

    private void Start()
    {
        if (m_Debug) Invoke("DebugInit", m_DebugIterationTime);
    }

    void DebugInit()
    {
        StartCoroutine(AddDistance());
    }

    public void HideTileMap()
    {
        m_Overworld.Hide();
    }

    public void ShowTileMap()
    {
        m_Overworld.Spawn();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift)) ShowTileMap();

        if (!m_Overworld.gameObject.activeSelf)
        {
            m_LastTilePos = m_Debug ? m_Distance : LocationManager.Instance.position;
            return;
        }

        Vector2 p = m_Debug ? m_Distance : LocationManager.Instance.position;
        Vector2 posToMove = p - m_LastTilePos;
        string debug = "From Center: {0}";
        if (m_FromCenterText) m_FromCenterText.text = string.Format(debug, posToMove);

        if (m_Overworld.MoveTo(posToMove))
        {
            m_LastTilePos = m_Debug ? m_Distance : LocationManager.Instance.position;
            // JUMP ENKO JUMP
        }
    }
    

    IEnumerator AddDistance()
    {
        m_Distance += Random.insideUnitCircle.normalized * m_DebugIterationDistance;

        yield return new WaitForSeconds(m_DebugIterationTime);
        StartCoroutine(AddDistance());
    }
}
