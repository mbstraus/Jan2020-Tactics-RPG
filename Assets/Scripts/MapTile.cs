using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapTile
{
    public string TileType;
    public Vector2Int GridPosition;
    public TileBase BaseTile;

    public MapTile()
    {

    }

    public MapTile(TileBase baseTile, int x, int y)
    {
        BaseTile = baseTile;
        TileType = baseTile.name;
        GridPosition = new Vector2Int(x, y);
    }

    public int GetMoveCost()
    {
        switch (TileType)
        {
            case "Dirt":
            case "Grass":
            case "Bridge_E_W":
            case "Bridge_N_S":
                return 1;
            case "Forest":
                return 2;
            case "Water":
                return 99;
            default:
                return 99;
        }
    }
}
