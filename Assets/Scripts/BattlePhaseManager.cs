using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class BattlePhaseManager : MonoBehaviour
{
    public static BattlePhaseManager Instance;

    [SerializeField] private GameObject PlayerUnitsContainer;
    private BattlePhaseState CurrentState;
    private Tilemap tilemap;
    private MapTile[,] Map;
    private Unit[] Units;
    public Unit SelectedUnit { get; private set; }

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

        Units = PlayerUnitsContainer.GetComponentsInChildren<Unit>();
        SetSelectedUnit(null);

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
        catch (IndexOutOfRangeException)
        {
            return null;
        }
    }

    public Unit GetUnitAtTile(MapTile mapTile)
    {
        foreach (var unit in Units)
        {
            if ((int) unit.transform.position.x == mapTile.GridPosition.x && (int) unit.transform.position.y == mapTile.GridPosition.y)
            {
                return unit.GetComponent<Unit>();
            }
        }
        return null;
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

    public void SetSelectedUnit(Unit selectedUnit)
    {
        SelectedUnit = selectedUnit;
        UIManager.Instance.ClearMoveRange();

        if (selectedUnit != null)
        {
            List<MapTile> accessableTiles = CalculateMoveRange(selectedUnit);
            UIManager.Instance.ShowMoveRange(accessableTiles);
        }
    }

    private List<MapTile> CalculateMoveRange(Unit selectedUnit)
    {
        int unitPositionX = (int)selectedUnit.gameObject.transform.position.x;
        int unitPositionY = (int)selectedUnit.gameObject.transform.position.y;
        MapTile currentTile = GetTileAt(unitPositionX, unitPositionY);

        int moveCost = 0;
        List<MapTile> accessableTiles = new List<MapTile>();

        ProcessTile(selectedUnit, currentTile, accessableTiles, moveCost);

        return accessableTiles;
    }

    private void ProcessTile(Unit selectedUnit, MapTile tileToProcess, List<MapTile> accessableTiles, int moveCost)
    {
        int localMoveCost = moveCost + tileToProcess.GetMoveCost();
        Debug.Log("Processing Tile: " + tileToProcess.GridPosition.ToString() + " - " + localMoveCost);
        if (localMoveCost > selectedUnit.Movement)
        {
            return;
        }
        if (!accessableTiles.Contains(tileToProcess))
        {
            accessableTiles.Add(tileToProcess);
        }
        List<MapTile> adjacentTiles = GetAdjacentTiles(tileToProcess.GridPosition.x, tileToProcess.GridPosition.y);
        foreach (var adjacentTile in adjacentTiles)
        {
            Debug.Log("Adjacent Tile for " + tileToProcess.GridPosition.ToString() + ": " + adjacentTile.GridPosition.ToString() + " - " + adjacentTile.GetMoveCost());
            ProcessTile(selectedUnit, adjacentTile, accessableTiles, localMoveCost);
        }
    }
}
