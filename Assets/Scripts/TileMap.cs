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

public class Line
{
    public Vector2 P1;
    public Vector2 P2;
}

public class MovingTile
{
    public Tile Tile;
    public GameObject Child;
    public Line TilePath;
    public Line ChildPath;
    public float SpawnTime;
    public float ChildTime;
    public bool Dead;
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
    Tilemap m_Tilemap;
    Vector2Int m_Bounds;

    void Start()
    {
        m_Tiles = new List<Tile>();
        m_MovingTiles = new List<MovingTile>();
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

    public bool MoveTo(Vector2 worldPos)
    {
        Vector2Int tile = TileFromWorldPos(worldPos);
        Vector2 dir = WorldFromTile(tile);
        
        if (tile == Vector2Int.zero) return false;

        print("tile: " + tile + " dir: " + dir);

        for (int i = 0; i < m_MovingTiles.Count; i++)
        {
            m_MovingTiles[i].Tile.CellPosition -= tile;

            m_MovingTiles[i].TilePath = new Line() { P1 = dir, P2 = Vector2.zero };
            Vector2 cH = Vector2.up * m_ChildHeight;
            Vector2 cPos = m_Tilemap.CellToWorld((Vector3Int)m_MovingTiles[i].Tile.CellPosition) + (Vector3)cH;
            m_MovingTiles[i].ChildPath = new Line() { P1 = cPos + dir, P2 = cPos };
            m_MovingTiles[i].SpawnTime = 0.0f;
            m_MovingTiles[i].ChildTime = 0.0f;

            m_Tilemap.SetTile((Vector3Int)m_MovingTiles[i].Tile.CellPosition, m_MovingTiles[i].Tile.TileSprite);
            m_Tilemap.SetTile((Vector3Int)(m_MovingTiles[i].Tile.CellPosition + tile), null);

            SetTilePosition(m_MovingTiles[i].Tile.CellPosition, m_MovingTiles[i].TilePath.P1);
            if (m_MovingTiles[i].Child) m_MovingTiles[i].Child.transform.position = m_MovingTiles[i].ChildPath.P1;
        }

        StartCoroutine(MoveTiles(dir * -1, tile));

        return true;
    }

    Vector2Int TileFromWorldPos(Vector2 worldPos)
    {
        return new Vector2Int(Mathf.RoundToInt(worldPos.x), Mathf.RoundToInt(worldPos.y));
    }

    Vector2 WorldFromTile(Vector2Int tile)
    {
        return m_Tilemap.CellToWorld((Vector3Int)tile);
    }

    int IndexFromTile(Vector2Int tile)
    {
        int x = tile.x + -m_Bounds.x;
        int y = tile.y + -m_Bounds.x;
        return x * m_ViewSize + (m_ViewSize - y - 1);
    }

    IEnumerator MoveTiles(Vector2 dir, Vector2Int tile)
    {
        for (float a = 0.0f; a < 1.0f; a += Time.deltaTime)
        {
            float alpha = 1.0f - a;
            for (int i = 0; i < m_MovingTiles.Count; i++)
            {
                Vector2Int pos = m_MovingTiles[i].Tile.CellPosition;
                if (TileOOB(pos))
                {
                    SetTileAplpha(i, alpha);
                    if (m_MovingTiles[i].Child) SetChildAlpha(m_MovingTiles[i].Child, alpha);
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
                SetTileAplpha(i, 0.0f);
                if (m_MovingTiles[i].Child) SetChildAlpha(m_MovingTiles[i].Child, 0.0f);
                m_MovingTiles[i].Dead = true;
            }
        }

        m_Tilemap.RefreshAllTiles();
        yield return null;
        Vector2Int p1 = new Vector2Int(m_Bounds.x, m_Bounds.x);
        Vector2Int p2 = new Vector2Int(m_Bounds.y, m_Bounds.y);
        StartCoroutine(CreateTiles(p1, p2));
    }

    void SetTileAplpha(int index, float alpha)
    {
        Color c = m_MovingTiles[index].Tile.TileSprite.color;
        c.a = alpha;
        m_MovingTiles[index].Tile.TileSprite.color = c;
    }

    void SetChildAlpha(GameObject child, float alpha)
    {
        SpriteRenderer sr = child.GetComponentInChildren<SpriteRenderer>();
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
            bool canPlace = false;
            if (time >= m_SpawnRate)
            {
                time -= m_SpawnRate;
                IEnumerable<MovingTile> t = null;
                if (m_MovingTiles.Count > 0)
                    t = m_MovingTiles.Where(mt => mt.Tile.CellPosition == new Vector2Int(x, y));
                canPlace = (t == null || t.Count() == 0);
                if (canPlace) AddTile(new Vector2Int(x, y));
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
            if (!canPlace) continue;

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
        tile.Init(new TileInfo(pos, tt.Type, m_SpawnCurve, m_ChildCurve, m_FallSpeed, this, tileSprite));
        tile.OnCreate();
        m_Tiles.Add(tile);

        Vector2 p = Vector2.up * m_SpawnHeight;
        MovingTile mt = new MovingTile()
        {
            Tile = tile,
            Child = null,
            TilePath = new Line()
            {
                P1 = Vector2.zero + Vector2.up * m_SpawnHeight,
                P2 = Vector2.zero
            }
        };
        m_MovingTiles.Add(mt);
        SetTilePosition(pos, p);

        StartCoroutine(AddChildTile(tt, mt));
    }

    IEnumerator AddChildTile(TileType tt, MovingTile mt)
    {
        yield return new WaitForSeconds(m_ChildSpawnDelay);
        if (tt.Child)
        {
            GameObject child = Instantiate(tt.Child);

            Vector3 worldPos = m_Tilemap.CellToWorld((Vector3Int)mt.Tile.CellPosition);
            Vector3 startPos = worldPos + Vector3.up * m_SpawnHeight;
            mt.Child = child;
            mt.ChildPath = new Line() { P1 = startPos, P2 = (Vector2)worldPos + Vector2.up * m_ChildHeight };
            child.transform.position = startPos;
        }
    }

    private void Update()
    {
        for (int i = 0; i < m_MovingTiles.Count; i++)
        {
            MovingTile mt = m_MovingTiles[i];

            if (m_MovingTiles[i].Dead)
            {
                m_Tilemap.SetTile((Vector3Int)mt.Tile.CellPosition, null);
                m_MovingTiles.RemoveAt(i);
                if (mt.Child) Destroy(mt.Child);
                i--;
                continue;
            }

            // Child
            if (mt.Child)
            {
                float lastC = mt.ChildTime;
                mt.ChildTime += Time.deltaTime;
                if (mt.ChildTime >= 1.0f)
                {
                    if (lastC < 1.0f)
                    {

                    }
                }
                else
                {
                    float cT = mt.Tile.ChildCurve.Evaluate(mt.ChildTime);
                    Vector2 cP = Vector2.LerpUnclamped(mt.ChildPath.P1, mt.ChildPath.P2, cT);
                    mt.Child.transform.position = cP;
                }
            }

            // Tile
            float lastS = mt.SpawnTime;
            mt.SpawnTime += Time.deltaTime;
            if (mt.SpawnTime >= 1.0f)
            {
                if (lastS < 1.0f)
                {

                }
                continue;
            }
            else
            {
                float sT = mt.Tile.SpawnCurve.Evaluate(mt.SpawnTime);
                Vector2 sP = Vector2.LerpUnclamped(mt.TilePath.P1, mt.TilePath.P2, sT);
                SetTilePosition(mt.Tile.CellPosition, sP);
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
