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

    // For all calculations below, assuming currently equipped item is an Iron Sword:
    // Range 1, 5 Weight, 5 Might, 90 Hit, 0 Crit

    // Formula: Weapon Might + Unit Strength
    public int WeaponAttack => 5 + Strength;
    // Formula: Weapon Hit + Unit Dexterity
    public int WeaponHit => 90 + Dexterity;
    // Formula: Weapon Crit + (Unit Dexterity + Unit Luck) / 2
    public int WeaponCrit => 0 + (Dexterity + Luck) / 2;
    // Formula: Unit Speed - Total Weight
    public int Avoid => Speed - 5;

    public List<MapTile> CalculateMoveRange()
    {
        int unitPositionX = (int)transform.position.x;
        int unitPositionY = (int)transform.position.y;
        MapTile currentTile = BattleManager.Instance.GetTileAt(unitPositionX, unitPositionY);

        int moveCost = 0;
        List<MapTile> accessableTiles = new List<MapTile>();

        ProcessTile(currentTile, accessableTiles, moveCost, 0);

        return accessableTiles;
    }

    private void ProcessTile(MapTile tileToProcess, List<MapTile> accessableTiles, int moveCost, int moveCostModifier)
    {
        int localMoveCost = moveCost + tileToProcess.GetMoveCost();
        if (localMoveCost > Movement + moveCostModifier)
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
            ProcessTile(adjacentTile, accessableTiles, localMoveCost, moveCostModifier);
        }
    }

    public List<MapTile> CalculateAttackRange()
    {
        int unitPositionX = (int)transform.position.x;
        int unitPositionY = (int)transform.position.y;
        MapTile currentTile = BattleManager.Instance.GetTileAt(unitPositionX, unitPositionY);

        int moveCost = 0;
        List<MapTile> accessableTiles = new List<MapTile>();

        // TODO: Move cost modifier here should be the weapon attack range.
        ProcessTile(currentTile, accessableTiles, moveCost, 1);

        return accessableTiles;
    }

    public List<MapTile> DetermineMovePath(MapTile mapTile)
    {
        int unitPositionX = (int)transform.position.x;
        int unitPositionY = (int)transform.position.y;


        MoveCostMapTile currentTile = new MoveCostMapTile(BattleManager.Instance.GetTileAt(unitPositionX, unitPositionY), 0, null);
        if (mapTile.Equals(currentTile.MapTile))
        {
            return new List<MapTile>();
        }
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
