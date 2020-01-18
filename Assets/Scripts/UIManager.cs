using UnityEngine;
using TMPro;
using System;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] private PlayerPhaseStartHUD PlayerPhase;
    [SerializeField] private EnemyPhaseStartHUD EnemyPhase;
    [SerializeField] private SelectedUnitHUD SelectedUnitHUD;

    [SerializeField] private TextMeshProUGUI LevelField;
    [SerializeField] private TextMeshProUGUI CurrentHPField;
    [SerializeField] private TextMeshProUGUI MaxHPField;
    [SerializeField] private TextMeshProUGUI SelectedUnitName;

    [SerializeField] private GameObject ActiveTileInstance;
    [SerializeField] private GameObject MoveRangeIndicatorsContainer;
    [SerializeField] private GameObject MoveRangeIndicatorPrefab;
    [SerializeField] private GameObject AttackRangeIndicatorsContainer;
    [SerializeField] private GameObject AttackRangeIndicatorPrefab;

    [SerializeField] private List<GameObject> MovementArrowPrefabs;
    [SerializeField] private GameObject MovementPathContainer;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        InputManager inputManager = FindObjectOfType<InputManager>();
        inputManager.RegisterMouseOverTileEvent(OnTileHover);

        BattleManager.Instance.RegisterOnUnitSelectedEvent(OnUnitSelected);
    }

    private void Update()
    {
        Unit selectedUnit = BattleManager.Instance.SelectedUnit;
        if (selectedUnit != null)
        {
            SelectedUnitHUD.gameObject.SetActive(true);
            LevelField.text = selectedUnit.Level.ToString();
            CurrentHPField.text = selectedUnit.CurrentHealthPoints.ToString();
            MaxHPField.text = selectedUnit.MaxHealthPoints.ToString();
            SelectedUnitName.text = selectedUnit.Name;
        }
        else if (selectedUnit == null)
        {
            SelectedUnitHUD.gameObject.SetActive(false);
        }
    }

    public void OnUnitSelected(Unit selectedUnit)
    {
        if (selectedUnit != null)
        {
            List<MapTile> moveRange = selectedUnit.CalculateMoveRange();
            List<MapTile> attackRange = selectedUnit.CalculateAttackRange();
            ShowMoveRange(moveRange);
            ShowAttackRange(attackRange, moveRange);
        }
        else
        {
            ClearMoveRange();
        }
    }

    public void ShowMoveRange(List<MapTile> accessableTiles)
    {
        ClearMoveRange();
        foreach (var tile in accessableTiles)
        {
            Vector3 position = new Vector3(tile.GridPosition.x + 0.5f, tile.GridPosition.y + 0.5f, 0f);
            Instantiate(MoveRangeIndicatorPrefab, position, Quaternion.identity, MoveRangeIndicatorsContainer.transform);
        }
    }

    public void ClearMoveRange()
    {
        foreach (Transform child in MoveRangeIndicatorsContainer.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void ShowAttackRange(List<MapTile> attackRange, List<MapTile> moveRange)
    {
        ClearAttackRange();
        foreach (var tile in attackRange)
        {
            // Don't show the attack indicator if the unit can already move to the tile.
            if (moveRange.Contains(tile))
            {
                continue;
            }
            Vector3 position = new Vector3(tile.GridPosition.x + 0.5f, tile.GridPosition.y + 0.5f, 0f);
            Instantiate(AttackRangeIndicatorPrefab, position, Quaternion.identity, AttackRangeIndicatorsContainer.transform);
        }
    }

    public void ClearAttackRange()
    {
        foreach (Transform child in AttackRangeIndicatorsContainer.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void ShowPlayerPhase()
    {
        Animation anim = PlayerPhase.GetComponent<Animation>();
        anim.Play("PhaseHUDAnimation");
    }

    public void ShowEnemyPhase()
    {
        Animation anim = EnemyPhase.GetComponent<Animation>();
        anim.Play("PhaseHUDAnimation");
    }

    public void OnTileHover(MapTile mapTile, MapTile previousTile, bool isTileAccessible, bool isTileAttackable)
    {
        if (mapTile != null)
        {
            ActiveTileInstance.transform.position = new Vector3(mapTile.GridPosition.x + 0.5f, mapTile.GridPosition.y + 0.5f, 0f);
            ActiveTileInstance.SetActive(true);

            if (isTileAccessible)
            {
                List<MapTile> movePath = BattleManager.Instance.SelectedUnit.DetermineMovePath(mapTile);
                DrawMovementPath(movePath);
            }
            else if (isTileAttackable)
            {
                List<MapTile> movePath = BattleManager.Instance.SelectedUnit.DetermineMovePath(previousTile);
                DrawMovementPath(movePath);
            }
            else
            {
                ClearMovementPath();
            }
        }
        else
        {
            ActiveTileInstance.SetActive(false);
            ClearMovementPath();
        }
    }

    public enum Direction
    {
        UP, DOWN, LEFT, RIGHT, UNKNOWN
    };

    public void ClearMovementPath()
    {
        foreach (Transform child in MovementPathContainer.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void DrawMovementPath(List<MapTile> path)
    {
        ClearMovementPath();
        if (path.Count == 0)
        {
            return;
        }
        for (int index = 0; index < path.Count; index++)
        {
            MapTile currentTile = path[index];
            MapTile previousTile = null;
            MapTile nextTile = null;
            Direction nextDirection;
            Direction previousDirection;
            if (index != 0)
            {
                previousTile = path[index - 1];
            }
            if (index < path.Count - 1)
            {
                nextTile = path[index + 1];
            }
            previousDirection = DetermineTileDirection(currentTile, previousTile);
            nextDirection = DetermineTileDirection(currentTile, nextTile);
            GameObject arrowForTile = DetermineMovementArrowType(previousDirection, nextDirection);
            Vector3 instantiatePosition = new Vector3(currentTile.GridPosition.x, currentTile.GridPosition.y, 0f);
            Instantiate(arrowForTile, instantiatePosition, Quaternion.identity, MovementPathContainer.transform);
        }
    }

    private Direction DetermineTileDirection(MapTile currentTile, MapTile otherTile)
    {
        if (otherTile == null)
        {
            return Direction.UNKNOWN;
        }
        if (currentTile.GridPosition.x > otherTile.GridPosition.x)
        {
            return Direction.LEFT;
        }
        if (currentTile.GridPosition.x < otherTile.GridPosition.x)
        {
            return Direction.RIGHT;
        }
        if (currentTile.GridPosition.y > otherTile.GridPosition.y)
        {
            return Direction.DOWN;
        }
        if (currentTile.GridPosition.y < otherTile.GridPosition.y)
        {
            return Direction.UP;
        }
        return Direction.UNKNOWN;
    }

    private GameObject DetermineMovementArrowType(Direction previousDirection, Direction nextDirection)
    {
        if (previousDirection == Direction.UNKNOWN)
        {
            if (nextDirection == Direction.LEFT || nextDirection == Direction.RIGHT) return MovementArrowPrefabs[5];
            else return MovementArrowPrefabs[4];
        }
        if (nextDirection == Direction.UNKNOWN)
        {
            if (previousDirection == Direction.DOWN) return MovementArrowPrefabs[9];
            if (previousDirection == Direction.UP) return MovementArrowPrefabs[6];
            if (previousDirection == Direction.LEFT) return MovementArrowPrefabs[8];
            else return MovementArrowPrefabs[7];
        }
        if ((previousDirection == Direction.UP || previousDirection == Direction.DOWN)
            && (nextDirection == Direction.UP || nextDirection == Direction.DOWN))
        {
            return MovementArrowPrefabs[4];
        }
        if ((previousDirection == Direction.LEFT || previousDirection == Direction.RIGHT)
            && (nextDirection == Direction.LEFT || nextDirection == Direction.RIGHT))
        {
            return MovementArrowPrefabs[5];
        }
        if ((previousDirection == Direction.DOWN || previousDirection == Direction.RIGHT)
            && (nextDirection == Direction.DOWN || nextDirection == Direction.RIGHT))
        {
            return MovementArrowPrefabs[1];
        }
        if ((previousDirection == Direction.DOWN || previousDirection == Direction.LEFT)
            && (nextDirection == Direction.DOWN || nextDirection == Direction.LEFT))
        {
            return MovementArrowPrefabs[0];
        }
        if ((previousDirection == Direction.UP || previousDirection == Direction.RIGHT)
            && (nextDirection == Direction.UP || nextDirection == Direction.RIGHT))
        {
            return MovementArrowPrefabs[3];
        }
        if ((previousDirection == Direction.UP || previousDirection == Direction.LEFT)
            && (nextDirection == Direction.UP || nextDirection == Direction.LEFT))
        {
            return MovementArrowPrefabs[2];
        }
        Debug.Log("Unable to determine movement arrow position, defaulting to straight up down.");
        return MovementArrowPrefabs[4];
    }
}
