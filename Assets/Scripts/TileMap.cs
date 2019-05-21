using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class TileType
{
    public TileAttribute Type;
    public UnityEngine.Tilemaps.Tile Tile;
    [Range(0.0f, 1.0f)] public float Occurrence;
    [HideInInspector] public float Current;
}

public class MovingTile
{
    public Tile Tile;
    public float Time;
    public Vector2 Start;
}

public class TileMap : Singleton<TileMap>
{
    [SerializeField] AnimationCurve m_SpawnCurve = null;
    [SerializeField] [Range(0.0f, 2.0f)] float m_SpawnRate = 0.03f;
    [SerializeField] [Range(0.0f, 10.0f)] float m_SpawnHeight = 5.0f;
    [SerializeField] [Range(0.0f, 10.0f)] float m_FallSpeed = 1.25f;
    [SerializeField] List<TileType> m_TileTypes = null;

    List<Tile> m_Tiles;
    List<MovingTile> m_MovingTiles;
    Tilemap m_Tilemap;
    int m_ViewSize;

    void Start()
    {
        m_Tiles = new List<Tile>();
        m_MovingTiles = new List<MovingTile>();
        m_Tilemap = GetComponent<Tilemap>();
        Init(8); // Temp
    }

    public void Init(int viewSize)
    {
        m_ViewSize = viewSize;

        m_Tilemap.ClearAllTiles();
        int size = m_ViewSize / 2;
        StartCoroutine(CreateTiles(new Vector2Int(-size, -size), new Vector2Int(size, size)));
    }

    IEnumerator CreateTiles(Vector2Int p1, Vector2Int p2)
    {
        for (int x = p1.x; x < p2.x; x++)
        {
            for (int y = p2.y - 1; y >= p1.y; y--)
            {
                AddTile(new Vector2Int(x, y));
                yield return new WaitForSeconds(m_SpawnRate);
            }
        }
    }

    void AddTile(Vector2Int pos)
    {
        for (int i = 0; i < m_TileTypes.Count; i++)
        {
            m_TileTypes[i].Current = Random.value * m_TileTypes[i].Occurrence;
        }

        TileType tt = m_TileTypes.OrderByDescending(t => t.Current).First();
        m_Tilemap.SetTile((Vector3Int)pos, tt.Tile);

        Tile tile = new Tile();
        tile.Init(new TileInfo(pos, tt.Type, m_SpawnCurve, m_FallSpeed, this));
        tile.OnCreate();
        m_Tiles.Add(tile);

        Vector2 p = Vector2.up * m_SpawnHeight;
        m_MovingTiles.Add(new MovingTile() { Tile = tile, Start = p });
        SetTilePosition(pos, p);
    }

    private void Update()
    {
        for (int i = 0; i < m_MovingTiles.Count; i++)
        {
            MovingTile mt = m_MovingTiles[i];

            mt.Time += Time.deltaTime * mt.Tile.FallSpeed;
            if (mt.Time >= 1.0f)
            {
                SetTilePosition(mt.Tile.CellPosition, Vector2.zero);
                m_MovingTiles.Remove(mt);
                i--;
                continue;
            }

            float t = mt.Tile.SpawnCurve.Evaluate(mt.Time);
            Vector2 p = Vector2.LerpUnclamped(mt.Start, Vector2.zero, t);
            SetTilePosition(mt.Tile.CellPosition, p);
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
}
