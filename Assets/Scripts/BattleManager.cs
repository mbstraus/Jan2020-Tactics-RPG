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

    public delegate void TileAttackedEvent(Unit attackingUnit, MapTile attackedTile);
    private TileAttackedEvent OnTileAttacked;

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
    public void RegisterOnTileAttackedEvent(TileAttackedEvent onTileAttacked)
    {
        OnTileAttacked += onTileAttacked;
    }
    public void UnregisterOnTileAttackedEvent(TileAttackedEvent onTileAttacked)
    {
        OnTileAttacked -= onTileAttacked;
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
                if (unit.CurrentHealthPoints <= 0)
                {
                    continue;
                }
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
            if (tileUnit != null && tileUnit.Team == Unit.UnitTeam.PLAYER)
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
                    AttackTile(selectedTile);
                }
                SetSelectedUnit(null);
                OnUnitSelected(null);
            }
        }
    }

    public void AttackTile(MapTile selectedTile)
    {
        Unit attackingUnit = SelectedUnit;
        Unit defendingUnit = GetUnitAtTile(selectedTile);

        if (attackingUnit == null || defendingUnit == null || attackingUnit == defendingUnit)
        {
            Debug.LogError("Something went wrong attacking the tile... " + attackingUnit + " - " + defendingUnit);
            return;
        }

        bool attackerIsHit = UnityEngine.Random.Range(0, 100) <= CalculateHitChance(attackingUnit, defendingUnit);
        bool attackerIsCrit = UnityEngine.Random.Range(0, 100) <= CalculateCritChance(attackingUnit, defendingUnit);
        if (attackerIsHit)
        {
            int damage = CalculateDamage(attackingUnit, defendingUnit) * (attackerIsCrit ? 3 : 1);
            defendingUnit.CurrentHealthPoints -= damage;
            Debug.Log("Attacking unit dealt " + damage + " to enemy!");
        }
        else
        {
            Debug.Log("Attacking unit missed!");
        }

        if (defendingUnit.CurrentHealthPoints <= 0)
        {
            Debug.Log("Defending unit died!");
            defendingUnit.GetComponent<SpriteRenderer>().enabled = false;
            return;
        }

        bool defenderIsHit = UnityEngine.Random.Range(0, 100) <= CalculateHitChance(defendingUnit, attackingUnit);
        bool defenderIsCrit = UnityEngine.Random.Range(0, 100) <= CalculateCritChance(defendingUnit, attackingUnit);
        if (defenderIsHit)
        {
            int damage = CalculateDamage(defendingUnit, attackingUnit) * (defenderIsCrit ? 3 : 1);
            attackingUnit.CurrentHealthPoints -= damage;
            Debug.Log("Defending unit dealt " + damage + " to attacker!");
        }
        else
        {
            Debug.Log("Defending unit missed!");
        }

        if (attackingUnit.CurrentHealthPoints <= 0)
        {
            Debug.Log("Attacking unit died!");
            attackingUnit.GetComponent<SpriteRenderer>().enabled = false;
            return;
        }
    }

    public static int CalculateDamage(Unit attackingUnit, Unit defendingUnit)
    {
        // Formula: Attacker [ATK] Weapon/Spell Might + Strength
        return Mathf.Max(attackingUnit.WeaponAttack - defendingUnit.Defense, 0);
    }

    public static int CalculateHitChance(Unit attackingUnit, Unit defendingUnit)
    {
        // Simulating attacking with an iron sword for simplicity (hit rate of 90, weight of 5).
        return Mathf.Clamp(attackingUnit.WeaponHit - defendingUnit.Avoid, 0, 100);
    }

    public static int CalculateCritChance(Unit attackingUnit, Unit defendingUnit)
    {
        return Mathf.Clamp(attackingUnit.WeaponCrit - defendingUnit.Luck, 0, 100);
    }
}
