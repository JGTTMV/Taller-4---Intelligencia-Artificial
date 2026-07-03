using System.Collections.Generic;

public class WFCTile
{
    public bool collapsed = false;

    public List<TileType> possibleTiles = new List<TileType>()
    {
        TileType.Start,
        TileType.Normal,
        TileType.Treasure,
        TileType.Boss
    };
}