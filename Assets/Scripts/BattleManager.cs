using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;

    [SerializeField] private GameObject PlayerUnitsContainer;
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private Unit[] units;

    private Map map;

    public BattlePhaseState CurrentState { get; private set; }
    public List<Unit> PlayerUnits { get; private set; }
    public List<Unit> EnemyUnits { get; private set; }
    public Unit SelectedUnit { get; private set; }

    public delegate void UnitSelectedEvent(Unit selectedUnit);
    private UnitSelectedEvent OnUnitSelected;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        map = new Map(tilemap);

        if (tilemap == null)
        {
            tilemap = FindObjectOfType<Tilemap>();
        }

        if (units == null)
        {
            units = PlayerUnitsContainer.GetComponentsInChildren<Unit>();
        }

        PlayerUnits = new List<Unit>();
        EnemyUnits = new List<Unit>();
        foreach (var unit in units)
        {
            if (unit.Team == Unit.UnitTeam.PLAYER)
            {
                PlayerUnits.Add(unit);
            }
            else
            {
                EnemyUnits.Add(unit);
            }
        }
        SetSelectedUnit(null);

        SetState(new PlayerPhaseState(this));

        InputManager inputManager = FindObjectOfType<InputManager>();
        inputManager.RegisterTileSelectedEvent(OnTileSelected);
    }

    void Update()
    {
        CurrentState.Tick();
    }

    public void SetState(BattlePhaseState newState)
    {
        CurrentState?.OnStateExit();
        CurrentState = newState;
        CurrentState.OnStateEnter();
    }

    public void RegisterOnUnitSelectedEvent(UnitSelectedEvent onUnitSelected)
    {
        OnUnitSelected += onUnitSelected;
    }
    public void UnregisterMouseOverTileEvent(UnitSelectedEvent onUnitSelected)
    {
        OnUnitSelected -= onUnitSelected;
    }

    public MapTile GetTileAt(int x, int y)
    {
        return map.GetTileAt(x, y);
    }

    public Unit GetUnitAtTile(MapTile mapTile)
    {
        foreach (var unit in units)
        {
            if ((int) unit.transform.position.x == mapTile.GridPosition.x && (int) unit.transform.position.y == mapTile.GridPosition.y)
            {
                return unit;
            }
        }
        return null;
    }

    public List<MapTile> GetAdjacentTiles(int x, int y)
    {
        return map.GetAdjacentTiles(x, y);
    }

    public void SetSelectedUnit(Unit selectedUnit)
    {
        SelectedUnit = selectedUnit;
        UIManager.Instance.ClearMoveRange();

        if (selectedUnit != null)
        {
            List<MapTile> accessableTiles = selectedUnit.CalculateMoveRange();
            UIManager.Instance.ShowMoveRange(accessableTiles);
        }
    }

    public void MovePlayerTo(MapTile targetTile)
    {
        SelectedUnit.gameObject.transform.position = new Vector3(targetTile.GridPosition.x + 0.5f, targetTile.GridPosition.y + 0.5f, 0f);
        CurrentState.UnitMoved(SelectedUnit);
    }

    public void OnTileSelected(MapTile selectedTile, MapTile previousTile, Vector3 mouseLocation, bool isTileAccessible, bool isTileAttackable)
    {
        if (CurrentState is PlayerPhaseState && selectedTile != null)
        {
            Unit tileUnit = GetUnitAtTile(selectedTile);
            if (tileUnit != null)
            {
                SetSelectedUnit(tileUnit);
                OnUnitSelected(tileUnit);
            }
            else
            {
                if (isTileAccessible)
                {
                    MovePlayerTo(selectedTile);
                }
                else if (isTileAttackable)
                {
                    MovePlayerTo(previousTile);
                }
                SetSelectedUnit(null);
                OnUnitSelected(tileUnit);
            }
        }
    }
}
