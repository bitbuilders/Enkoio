using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMap : Singleton<TileMap>
{
    [SerializeField] UnityEngine.Tilemaps.Tile m_TileTemplate = null;

    public struct TileInfo
    {
        public Vector2Int CellPosition;
        public TileAttribute Attribute;
        public TileMap TileMap;
    }

    Tile[] m_Tiles;
    Tilemap m_Tilemap;

    void Start()
    {
        m_Tilemap = GetComponent<Tilemap>();
        Init(8);
        m_Tilemap.SetTile(Vector3Int.zero, m_TileTemplate);
    }

    public void Init(int viewDist)
    {
        m_Tilemap.ClearAllTiles();
        for (int x = 0; x < viewDist; x++)
        {
            for (int y = 0; y < viewDist; y++)
            {
                
            }
        }
    }

    private void Update()
    {
        return;
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                float t = Time.time + (i * 8 * .02f + j * .1f);
                float y = Mathf.PingPong(t, 1.0f) - 0.5f;
                Vector2Int tile = new Vector2Int(i - 4, j - 4);
                Vector2 position = Vector2.up * y;
                SetTilePosition(tile, position);
            }
        }
    }

    /// <summary>
    /// Sets a specified tile's position
    /// </summary>
    /// <param name="tile">The tile to modify</param>
    /// <param name="position">The local space position</param>
    public void SetTilePosition(Vector2Int tile, Vector2 position)
    {
        Matrix4x4 transform = Matrix4x4.TRS(position, Quaternion.identity, Vector3.one);
        m_Tilemap.SetTransformMatrix(new Vector3Int(tile.x, tile.y, 0), transform);
    }

    /// <summary>
    /// Returns a specified tile's position
    /// </summary>
    /// <param name="tile">The tile to get</param>
    /// <returns></returns>
    public Vector2 GetTilePosition(Vector2Int tile)
    {
        // Column 3 is apparently position
        return m_Tilemap.GetTransformMatrix(new Vector3Int(tile.x, tile.y, 0)).GetColumn(3);
    }

    Tile CreateTile(TileInfo info)
    {
        Tile tile = null;

        switch (info.Attribute)
        {
            case TileAttribute.SHOP:
                tile = new Shop();
                break;
            case TileAttribute.MYSTERY:
                tile = new Mystery();
                break;
            case TileAttribute.BAD:
                tile = new Bad();
                break;
            case TileAttribute.CASTLE:
                tile = new Castle();
                break;
            case TileAttribute.HOME:
                tile = new Home();
                break;
            default:
                tile = new Tile();
                break;
        }

        tile.Init(info.CellPosition, info.Attribute, info.TileMap);

        return tile;
    }
}
