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

public struct TileInfo
{
    public TileInfo(Vector2Int pos, TileAttribute type, AnimationCurve spawnCurve, float fallSpeed, 
        TileMap tmap, UnityEngine.Tilemaps.Tile tileSprite)
    {
        CellPosition = pos;
        Attribute = type;
        SpawnCurve = spawnCurve;
        FallSpeed = fallSpeed;
        TileMap = tmap;
        TileSprite = tileSprite;
    }

    public Vector2Int CellPosition;
    public TileAttribute Attribute;
    public AnimationCurve SpawnCurve;
    public float FallSpeed;
    public TileMap TileMap;
    public UnityEngine.Tilemaps.Tile TileSprite;
}

public class Tile
{
    public Vector2Int CellPosition { get; set; }
    public TileAttribute Attribute { get; private set; }
    public AnimationCurve SpawnCurve { get; private set; }
    public UnityEngine.Tilemaps.Tile TileSprite { get; private set; }
    public float FallSpeed { get; private set; }

    protected TileMap m_TileMap;

    public void Init(TileInfo tileInfo)
    {
        CellPosition = tileInfo.CellPosition;
        Attribute = tileInfo.Attribute;
        SpawnCurve = tileInfo.SpawnCurve;
        TileSprite = tileInfo.TileSprite;
        FallSpeed = tileInfo.FallSpeed;
        m_TileMap = tileInfo.TileMap;
    }

    virtual public void OnCreate()
    {
        
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
