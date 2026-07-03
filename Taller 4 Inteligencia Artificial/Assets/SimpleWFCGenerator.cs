using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class SimpleWFCGenerator : MonoBehaviour
{
    public GameObject roomPrefab;

    [Header("Mapa")]
    public int totalRooms = 15;
    public float roomSpacing = 1.2f;

    [Header("Límites")]
    public int minX = -5;
    public int maxX = 5;
    public int minY = -5;
    public int maxY = 5;

    private Dictionary<Vector2Int, WFCTile> rooms = new Dictionary<Vector2Int, WFCTile>();

    void Start()
    {
        Stopwatch stopwatch = Stopwatch.StartNew();

        GenerateShape();
        CollapseRooms();
        SpawnRooms();

        stopwatch.Stop();
        UnityEngine.Debug.Log($"Mapa WFC generado en {stopwatch.ElapsedMilliseconds} ms");
    }

    void GenerateShape()
    {
        Vector2Int current = Vector2Int.zero;

        rooms[current] = new WFCTile();

        while (rooms.Count < totalRooms)
        {
            int dir = Random.Range(0, 4);

            Vector2Int next = current;

            switch (dir)
            {
                case 0: next += Vector2Int.up; break;
                case 1: next += Vector2Int.down; break;
                case 2: next += Vector2Int.left; break;
                case 3: next += Vector2Int.right; break;
            }

            if (next.x < minX || next.x > maxX ||
                next.y < minY || next.y > maxY)
                continue;

            current = next;

            if (!rooms.ContainsKey(current))
                rooms.Add(current, new WFCTile());
        }
    }

    void CollapseRooms()
    {
        List<Vector2Int> positions = new List<Vector2Int>(rooms.Keys);

        Vector2Int startPos = positions[Random.Range(0, positions.Count)];
        positions.Remove(startPos);

        rooms[startPos].possibleTiles.Clear();
        rooms[startPos].possibleTiles.Add(TileType.Start);
        rooms[startPos].collapsed = true;

        Vector2Int bossPos = positions[0];
        float maxDistance = -1f;

        foreach (Vector2Int pos in positions)
        {
            float dist = Vector2Int.Distance(startPos, pos);

            if (dist > maxDistance)
            {
                maxDistance = dist;
                bossPos = pos;
            }
        }

        positions.Remove(bossPos);

        rooms[bossPos].possibleTiles.Clear();
        rooms[bossPos].possibleTiles.Add(TileType.Boss);
        rooms[bossPos].collapsed = true;

        Vector2Int treasurePos = positions[Random.Range(0, positions.Count)];
        positions.Remove(treasurePos);

        rooms[treasurePos].possibleTiles.Clear();
        rooms[treasurePos].possibleTiles.Add(TileType.Treasure);
        rooms[treasurePos].collapsed = true;

        foreach (var room in rooms)
        {
            if (room.Value.collapsed)
                continue;

            room.Value.possibleTiles.Remove(TileType.Start);
            room.Value.possibleTiles.Remove(TileType.Boss);
            room.Value.possibleTiles.Remove(TileType.Treasure);
        }

        Propagate(startPos);
        Propagate(bossPos);
        Propagate(treasurePos);

        while (true)
        {
            Vector2Int cell = FindLowestEntropy();

            if (!rooms.ContainsKey(cell))
                break;

            CollapseCell(cell);

            Propagate(cell);
        }
    }

    Vector2Int FindLowestEntropy()
    {
        int bestEntropy = int.MaxValue;

        Vector2Int best = new Vector2Int(999, 999);

        foreach (var room in rooms)
        {
            if (room.Value.collapsed)
                continue;

            int entropy = room.Value.possibleTiles.Count;

            if (entropy < bestEntropy)
            {
                bestEntropy = entropy;
                best = room.Key;
            }
        }

        return best;
    }

    void CollapseCell(Vector2Int pos)
    {
        WFCTile tile = rooms[pos];

        int random = Random.Range(0, tile.possibleTiles.Count);

        TileType chosen = tile.possibleTiles[random];

        tile.possibleTiles.Clear();
        tile.possibleTiles.Add(chosen);

        tile.collapsed = true;
    }

    void Propagate(Vector2Int pos)
    {
        TileType current = rooms[pos].possibleTiles[0];

        Vector2Int[] dirs =
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        foreach (Vector2Int dir in dirs)
        {
            Vector2Int neighborPos = pos + dir;

            if (!rooms.ContainsKey(neighborPos))
                continue;

            WFCTile neighbor = rooms[neighborPos];

            if (neighbor.collapsed)
                continue;

            if (current == TileType.Boss)
            {
                neighbor.possibleTiles.Remove(TileType.Boss);
            }

            if (current == TileType.Treasure)
            {
                neighbor.possibleTiles.Remove(TileType.Treasure);
            }

            if (neighbor.possibleTiles.Count == 0)
            {
                neighbor.possibleTiles.Add(TileType.Normal);
            }
        }
    }
    void SpawnRooms()
    {
        foreach (var room in rooms)
        {
            Vector3 worldPos = new Vector3(
                room.Key.x * roomSpacing,
                room.Key.y * roomSpacing,
                0);

            GameObject obj = Instantiate(roomPrefab, worldPos, Quaternion.identity, transform);

            SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();

            switch (room.Value.possibleTiles[0])
            {
                case TileType.Start:
                    sr.color = Color.green;
                    break;

                case TileType.Normal:
                    sr.color = Color.gray;
                    break;

                case TileType.Treasure:
                    sr.color = Color.yellow;
                    break;

                case TileType.Boss:
                    sr.color = Color.red;
                    break;
            }
        }
    }
}