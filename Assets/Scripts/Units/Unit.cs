using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Unit : MonoBehaviour
{
    public enum UnitTeam
    {
        PLAYER, ENEMY
    }
    [SerializeField] public string Name;
    [SerializeField] public int CurrentHealthPoints;
    [SerializeField] public int MaxHealthPoints;
    [SerializeField] public int Level;
    [SerializeField] public int Strength;
    [SerializeField] public int Magic;
    [SerializeField] public int Dexterity;
    [SerializeField] public int Speed;
    [SerializeField] public int Luck;
    [SerializeField] public int Defense;
    [SerializeField] public int Resistance;
    [SerializeField] public int Movement;
    [SerializeField] public UnitTeam Team;

    public List<MapTile> CalculateMoveRange()
    {
        int unitPositionX = (int)transform.position.x;
        int unitPositionY = (int)transform.position.y;
        MapTile currentTile = BattleManager.Instance.GetTileAt(unitPositionX, unitPositionY);

        int moveCost = 0;
        List<MapTile> accessableTiles = new List<MapTile>();

        ProcessTile(currentTile, accessableTiles, moveCost);

        return accessableTiles;
    }

    private void ProcessTile(MapTile tileToProcess, List<MapTile> accessableTiles, int moveCost)
    {
        int localMoveCost = moveCost + tileToProcess.GetMoveCost();
        if (localMoveCost > Movement)
        {
            return;
        }
        if (!accessableTiles.Contains(tileToProcess))
        {
            accessableTiles.Add(tileToProcess);
        }
        List<MapTile> adjacentTiles = BattleManager.Instance.GetAdjacentTiles(tileToProcess.GridPosition.x, tileToProcess.GridPosition.y);
        foreach (var adjacentTile in adjacentTiles)
        {
            ProcessTile(adjacentTile, accessableTiles, localMoveCost);
        }
    }

    public List<MapTile> DetermineMovePath(MapTile mapTile)
    {
        int unitPositionX = (int)transform.position.x;
        int unitPositionY = (int)transform.position.y;

        MoveCostMapTile currentTile = new MoveCostMapTile(BattleManager.Instance.GetTileAt(unitPositionX, unitPositionY), 0, null);
        Queue<MoveCostMapTile> processingQueue = new Queue<MoveCostMapTile>();
        List<MapTile> processedTiles = new List<MapTile>();
        List<MoveCostMapTile> adjacentTiles = GetAdjacentMoveCostMapTiles(currentTile, processedTiles);
        processedTiles.Add(currentTile.MapTile);
        EnqueueAdjacentTiles(processingQueue, adjacentTiles);

        while (processingQueue.Count > 0)
        {
            var adjacentTile = processingQueue.Dequeue();
            processedTiles.Add(adjacentTile.MapTile);
            if (adjacentTile.MapTile == mapTile)
            {
                return BuildPath(adjacentTile);
            }
            adjacentTiles = GetAdjacentMoveCostMapTiles(adjacentTile, processedTiles);
            EnqueueAdjacentTiles(processingQueue, adjacentTiles);
        }
        Debug.LogError("Failed to build path to tile!");
        return new List<MapTile>();
    }

    private void EnqueueAdjacentTiles(Queue<MoveCostMapTile> processingQueue, List<MoveCostMapTile> adjacentTiles)
    {
        foreach (var adjacentTile in adjacentTiles)
        {
            processingQueue.Enqueue(adjacentTile);
        }
    }

    private List<MoveCostMapTile> GetAdjacentMoveCostMapTiles(MoveCostMapTile currentTile, List<MapTile> processedTiles)
    {
        List<MapTile> adjacentTiles = BattleManager.Instance.GetAdjacentTiles(currentTile.MapTile.GridPosition.x, currentTile.MapTile.GridPosition.y);
        List<MoveCostMapTile> returnList = new List<MoveCostMapTile>();
        foreach (var adjacentTile in adjacentTiles)
        {
            if (processedTiles.Contains(adjacentTile))
            {
                continue;
            }
            MoveCostMapTile tile = new MoveCostMapTile(adjacentTile, currentTile.Cost + adjacentTile.GetMoveCost(), currentTile);
            returnList.Add(tile);
        }
        return returnList;
    }

    private List<MapTile> BuildPath(MoveCostMapTile moveCostAdjacentTile)
    {
        List<MapTile> moveList = new List<MapTile>();
        MoveCostMapTile current = moveCostAdjacentTile;
        moveList.Add(current.MapTile);
        while (current.PreviousMapTile != null)
        {
            current = current.PreviousMapTile;
            moveList.Add(current.MapTile);
        }

        moveList.Reverse();
        return moveList;
    }
}
