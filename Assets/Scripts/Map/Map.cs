using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Map
{
    public MapTile[,] MapTiles { get; private set; }

    public Map(Tilemap tilemap)
    {
        BoundsInt bounds = tilemap.cellBounds;
        TileBase[] tiles = tilemap.GetTilesBlock(bounds);
        MapTiles = new MapTile[bounds.size.y, bounds.size.x];

        for (int x = 0; x < bounds.size.x; x++)
        {
            for (int y = 0; y < bounds.size.y; y++)
            {
                TileBase tile = tiles[x + y * bounds.size.x];
                if (tile != null)
                {
                    MapTile mapTile = new MapTile(tile, x, y);
                    MapTiles[x, y] = mapTile;
                }
            }
        }
    }

    /// <summary>
    /// Gets the tile at position x and y in the tile map.  X reflects the column that the tile is in, from left to right.
    /// Y reflects the row that the tile is in, from bottom to top.  These reflect Unity positional coordinates.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public MapTile GetTileAt(int x, int y)
    {
        try
        {
            return MapTiles[x, y];
        }
        catch (IndexOutOfRangeException)
        {
            return null;
        }
    }

    public List<MapTile> GetAdjacentTiles(int x, int y)
    {
        List<MapTile> returnList = new List<MapTile>();
        MapTile north = GetTileAt(x, y + 1);
        MapTile south = GetTileAt(x, y - 1);
        MapTile east = GetTileAt(x + 1, y);
        MapTile west = GetTileAt(x - 1, y);
        if (north != null) returnList.Add(north);
        if (south != null) returnList.Add(south);
        if (east != null) returnList.Add(east);
        if (west != null) returnList.Add(west);
        return returnList;
    }
}
