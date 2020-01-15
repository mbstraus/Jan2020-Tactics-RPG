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

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        InputManager inputManager = FindObjectOfType<InputManager>();
        inputManager.RegisterMouseOverTileEvent(ShowTileHover);

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
            ShowMoveRange(selectedUnit.CalculateMoveRange());
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

    public void ShowTileHover(MapTile mapTile)
    {
        if (mapTile != null)
        {
            ActiveTileInstance.transform.position = new Vector3(mapTile.GridPosition.x + 0.5f, mapTile.GridPosition.y + 0.5f, 0f);
            ActiveTileInstance.SetActive(true);
        }
        else
        {
            ActiveTileInstance.SetActive(false);
        }
    }
}
