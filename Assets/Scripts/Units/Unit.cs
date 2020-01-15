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
}
