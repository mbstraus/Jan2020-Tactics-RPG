using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BattlePhaseManager : MonoBehaviour
{
    public static BattlePhaseManager Instance;

    private BattlePhaseState CurrentState;
    private Tilemap tilemap;
    private MapTile[,] Map;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        tilemap = FindObjectOfType<Tilemap>();

        BoundsInt bounds = tilemap.cellBounds;
        TileBase[] tiles = tilemap.GetTilesBlock(bounds);
        Map = new MapTile[bounds.size.y, bounds.size.x];

        for (int x = 0; x < bounds.size.x; x++)
        {
            for (int y = 0; y < bounds.size.y; y++)
            {
                TileBase tile = tiles[x + y * bounds.size.x];
                if (tile != null)
                {
                    MapTile mapTile = new MapTile(tile, x, y);
                    Map[x, y] = mapTile;
                }
            }
        }

        SetState(new PlayerPhaseState(this));
    }

    void Update()
    {
        CurrentState?.Tick();
    }

    public void SetState(BattlePhaseState newState)
    {
        CurrentState?.OnStateExit();
        CurrentState = newState;
        CurrentState.OnStateEnter();
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
            return Map[x, y];
        }
        catch (IndexOutOfRangeException ex)
        {
            Debug.LogWarning("Attempted to click outside of grid bounds. - " + ex.Message);
            return null;
        }
    }
}
