using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : Singleton<TileManager>
{
    [SerializeField] TileMap m_Overworld = null;

    Vector2 m_LastTilePos;

    private void Update()
    {
        Vector2 posToMove = LocationManager.Instance.position - m_LastTilePos;
        if (m_Overworld.MoveTo(posToMove))
        {
            m_LastTilePos = LocationManager.Instance.position;
            // JUMP ENKO JUMP

        }
    }
}
