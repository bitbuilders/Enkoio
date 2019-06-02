using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TileManager : Singleton<TileManager>
{
    [SerializeField] TileMap m_Overworld = null;
    [SerializeField] TextMeshProUGUI m_FromCenterText = null;

    Vector2 m_LastTilePos;

    private void Update()
    {
        Vector2 posToMove = LocationManager.Instance.position - m_LastTilePos;
        string debug = "From Center: {0}";
        if (m_FromCenterText) m_FromCenterText.text = string.Format(debug, LocationManager.Instance.position);

        if (m_Overworld.MoveTo(posToMove * 5.0f))
        {
            m_LastTilePos = LocationManager.Instance.position;
            // JUMP ENKO JUMP
            print("Move");
        }
    }
}
