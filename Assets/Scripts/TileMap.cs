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
    public GameObject Child;
    [Range(0.0f, 1.0f)] public float Occurrence;
    [HideInInspector] public float Current;
}

public class MovingTile
{
    public Tile Tile;
    public GameObject Source;
    public float Time;
    public Vector2 Start;
    public Vector2 End;
    public Vector2Int OriginalTile;
}

public class TileMap : MonoBehaviour
{
    [SerializeField] AnimationCurve m_SpawnCurve = null;
    [SerializeField] AnimationCurve m_ChildCurve = null;
    [SerializeField] [Range(1, 100)] int m_ViewSize = 7;
    [SerializeField] [Range(0.0f, 2.0f)] float m_SpawnRate = 0.03f; // .03 ~~ 1s total time, .04 ~~ 1.4s total time
    [SerializeField] [Range(0.0f, 10.0f)] float m_SpawnHeight = 5.0f;
    [SerializeField] [Range(-3.0f, 3.0f)] float m_ChildHeight = 0.5f;
    [SerializeField] [Range(0.0f, 10.0f)] float m_ChildSpawnDelay = 1.5f;
    [SerializeField] [Range(0.0f, 10.0f)] float m_FallSpeed = 1.25f;
    [SerializeField] List<TileType> m_TileTypes = null;

    List<Tile> m_Tiles;
    List<MovingTile> m_MovingTiles;
    List<MovingTile> m_MovingChildren;
    Tilemap m_Tilemap;
    Vector2Int m_Bounds;

    void Start()
    {
        m_Tiles = new List<Tile>();
        m_MovingTiles = new List<MovingTile>();
        m_MovingChildren = new List<MovingTile>();
        m_Tilemap = GetComponent<Tilemap>();
        Init();
    }

    public void Init()
    {
        m_Tilemap.ClearAllTiles();
        int size = m_ViewSize / 2;
        int offset = m_ViewSize % 2;
        m_Bounds = new Vector2Int(-size - offset, size);
        Vector2Int p1 = new Vector2Int(m_Bounds.x, m_Bounds.x);
        Vector2Int p2 = new Vector2Int(m_Bounds.y, m_Bounds.y);
        StartCoroutine(CreateTiles(p1, p2));
    }

    public void MoveTo(Vector2 worldPos)
    {
        Vector2Int tile = TileFromWorldPos(worldPos);
        Vector2 dir = WorldFromTile(tile);

        print("tile: " + tile + " dir: " + dir);

        for (int i = 0; i < m_MovingTiles.Count; i++)
        {
            int prev = tile.x / m_ViewSize + tile.y % m_ViewSize;
            m_MovingTiles[i].Start = dir;
            m_MovingTiles[i].End = Vector2.zero;
            m_MovingTiles[i].Time = 0.0f;
            m_MovingTiles[i].OriginalTile = m_MovingTiles[i].Tile.CellPosition;
            m_MovingTiles[i].Tile.CellPosition -= tile;
            m_Tilemap.SetTile((Vector3Int)m_MovingTiles[i + prev].Tile.CellPosition, m_MovingTiles[i].Tile.TileSprite);
            m_Tilemap.SetTile((Vector3Int)m_MovingTiles[i].Tile.CellPosition, m_MovingTiles[i + prev].Tile.TileSprite);
            //print("tile: " + m_MovingTiles[i].Tile.CellPosition + " | OOB: " + TileOOB(m_MovingTiles[i].Tile.CellPosition));
        }

        for (int i = 0; i < m_MovingChildren.Count; i++)
        {
            Vector3Int tPos = (Vector3Int)m_MovingChildren[i].Tile.CellPosition;
            Vector3 cHeight = Vector3.up * m_ChildHeight;
            m_MovingChildren[i].Start = m_Tilemap.CellToWorld(tPos) + cHeight;
            m_MovingChildren[i].End = m_Tilemap.CellToWorld(tPos - (Vector3Int)tile) + cHeight;
            m_MovingChildren[i].Time = 0.0f;
            m_MovingChildren[i].OriginalTile = m_MovingChildren[i].Tile.CellPosition;
            m_MovingChildren[i].Tile.CellPosition -= tile;
            //print("tile: " + m_MovingTiles[i].Tile.CellPosition + " | OOB: " + TileOOB(m_MovingTiles[i].Tile.CellPosition));
        }

        StartCoroutine(MoveTiles(dir * -1, tile));
    }

    Vector2Int TileFromWorldPos(Vector2 worldPos)
    {
        return new Vector2Int(Mathf.RoundToInt(worldPos.x), Mathf.RoundToInt(worldPos.y));
    }

    Vector2 WorldFromTile(Vector2Int tile)
    {
        return m_Tilemap.CellToWorld((Vector3Int)tile);
    }

    IEnumerator MoveTiles(Vector2 dir, Vector2Int tile)
    {
        //Vector2Int p1 = new Vector2Int(m_Bounds.x, m_Bounds.x);
        //Vector2Int p2 = new Vector2Int(m_Bounds.x, m_Bounds.x);
        //StartCoroutine(CreateTiles(p1, p2));

        for (float a = 0.0f; a < 1.0f; a += Time.deltaTime)
        {
            for (int i = 0; i < m_MovingTiles.Count; i++)
            {
                Vector2Int pos = m_MovingTiles[i].Tile.CellPosition;
                if (TileOOB(pos))
                {
                    SetTileAplpha(i, a);
                }
            }
            for (int i = 0; i < m_MovingChildren.Count; i++)
            {
                Vector2Int pos = m_MovingChildren[i].Tile.CellPosition;
                if (TileOOB(pos))
                {
                    SetChildAlpha(m_MovingChildren[i].Source, 1.0f - a);
                }
            }
            m_Tilemap.RefreshAllTiles();
            yield return null;
        }

        for (int i = 0; i < m_MovingTiles.Count; i++)
        {
            Vector2Int pos = m_MovingTiles[i].Tile.CellPosition;
            if (TileOOB(pos))
            {
                SetTileAplpha(i, 1.0f);
            }
        }
        for (int i = 0; i < m_MovingChildren.Count; i++)
        {
            Vector2Int pos = m_MovingChildren[i].Tile.CellPosition;
            if (TileOOB(pos))
            {
                SetChildAlpha(m_MovingChildren[i].Source, 0.0f);
            }
        }

        m_Tilemap.RefreshAllTiles();
    }

    void SetTileAplpha(int index, float alpha)
    {
        Color c = m_MovingTiles[index].Tile.TileSprite.color;
        c.a = 1.0f - alpha;
        m_MovingTiles[index].Tile.TileSprite.color = c;
    }

    void SetChildAlpha(GameObject child, float alpha)
    {
        SpriteRenderer sr = child.GetComponent<SpriteRenderer>();
        Color c = sr.color;
        c.a = alpha;
        sr.color = c;
    }

    bool TileOOB(Vector2Int tp)
    {
        return ((tp.x < m_Bounds.x || tp.x > m_Bounds.y) ||
                (tp.y < m_Bounds.x || tp.y > m_Bounds.y));
    }

    IEnumerator CreateTiles(Vector2Int p1, Vector2Int p2)
    {
        float start = Time.time;

        float time = 0.0f;
        int size = Mathf.Abs(p2.x - p1.x) * Mathf.Abs(p2.y - p1.y);
        int x = p1.x;
        int y = p2.y - 1;
        int count = 0;
        while (count < size)
        {
            time += Time.deltaTime;
            if (time >= m_SpawnRate)
            {
                time -= m_SpawnRate;
                AddTile(new Vector2Int(x, y));
                if (y >= p1.y)
                {
                    y--;
                    if (y < p1.y)
                    {
                        y = p2.y - 1;
                        x++;
                    }
                }
                count++;
            }

            if (count < size) yield return null;
        }

        float end = Time.time;
        print("Took: " + (end - start) + " seconds to spawn tiles");
        yield return new WaitForSeconds(2f);
        Vector2 target = new Vector2(0.6f, 0.0f);
        MoveTo(target);
    }

    void AddTile(Vector2Int pos)
    {
        for (int i = 0; i < m_TileTypes.Count; i++)
        {
            m_TileTypes[i].Current = Random.value * m_TileTypes[i].Occurrence;
        }

        TileType tt = m_TileTypes.OrderByDescending(t => t.Current).First();
        UnityEngine.Tilemaps.Tile tileSprite = Instantiate(tt.Tile);
        m_Tilemap.SetTile((Vector3Int)pos, tileSprite);

        Tile tile = new Tile();
        tile.Init(new TileInfo(pos, tt.Type, m_SpawnCurve, m_FallSpeed, this, tileSprite));
        tile.OnCreate();
        m_Tiles.Add(tile);

        Vector2 p = Vector2.up * m_SpawnHeight;
        m_MovingTiles.Add(new MovingTile() { Tile = tile, Start = p, End = Vector2.zero, OriginalTile = pos });
        SetTilePosition(pos, p);

        StartCoroutine(AddChildTile(tt, tile, (Vector3Int)pos));
    }

    IEnumerator AddChildTile(TileType tt, Tile tile, Vector3Int pos)
    {
        yield return new WaitForSeconds(m_ChildSpawnDelay);
        if (tt.Child)
        {
            GameObject child = Instantiate(tt.Child);
            Vector3 offset = Vector3.up * m_ChildHeight;
            child.transform.position = m_Tilemap.CellToWorld(pos) + offset;

            Vector3 worldPos = m_Tilemap.CellToWorld(pos);
            Vector3 startPos = worldPos + Vector3.up * m_SpawnHeight;
            m_MovingChildren.Add(new MovingTile()
            {
                Tile = tile,
                Source = child,
                Start = startPos,
                End = worldPos + offset,
                OriginalTile = (Vector2Int)pos
            });
            child.transform.position = startPos;
        }
    }

    private void Update()
    {
        UpdateSpawningTiles();
        UpdateChildTiles();
    }

    void UpdateSpawningTiles()
    {
        UpdateTiles(m_MovingTiles, m_SpawnCurve, false);
    }

    void UpdateChildTiles()
    {
        UpdateTiles(m_MovingChildren, m_ChildCurve, true);
    }

    void UpdateTiles(List<MovingTile> tiles, AnimationCurve curve, bool child)
    {
        for (int i = 0; i < tiles.Count; i++)
        {
            MovingTile mt = tiles[i];

            float last = mt.Time;
            mt.Time += Time.deltaTime * mt.Tile.FallSpeed;
            if (mt.Time >= 1.0f)
            {
                if (last < 1.0f)
                {
                    if (child) mt.Source.transform.position = mt.End;
                    else SetTilePosition(mt.OriginalTile, mt.End);
                }
                continue;
            }

            float t = curve.Evaluate(mt.Time);
            Vector2 p = Vector2.LerpUnclamped(mt.Start, mt.End, t);
            if (child) mt.Source.transform.position = p;
            else SetTilePosition(mt.OriginalTile, p);
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
        m_Tilemap.SetTransformMatrix((Vector3Int)tile, transform);
    }

    /// <summary>
    /// Returns a specified tile's position
    /// </summary>
    /// <param name="tile">The tile to get</param>
    /// <returns></returns>
    public Vector2 GetTilePosition(Vector2Int tile)
    {
        // Column 3 is apparently position
        return m_Tilemap.GetTransformMatrix((Vector3Int)tile).GetColumn(3);
    }
}
