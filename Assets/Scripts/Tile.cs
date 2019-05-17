using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileAttribute
{
    SHOP,
    MYSTERY,
    BAD,
    CASTLE,
    HOME
}

public class Tile
{
    public Vector2Int CellPosition { get; private set; }
    public TileAttribute Attribute { get; private set; }

    protected TileMap m_TileMap;

    public void Init(Vector2Int cellPosition, TileAttribute attribute, TileMap tileMap)
    {
        CellPosition = cellPosition;
        Attribute = attribute;
        m_TileMap = tileMap;
    }

    virtual public void OnEnter()
    {
        Debug.Log("Entered the " + Attribute.ToString() + " Tile!");
    }

    virtual public void Update()
    {

    }

    virtual public void OnExit()
    {
        Debug.Log("Left the " + Attribute.ToString() + " Tile!");
    }
}
