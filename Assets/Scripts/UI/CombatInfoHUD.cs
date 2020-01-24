﻿using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombatInfoHUD : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI PlayerUnitNameText;
    [SerializeField] private TextMeshProUGUI PlayerCurrentHPText;
    [SerializeField] private TextMeshProUGUI PlayerAttackText;
    [SerializeField] private TextMeshProUGUI PlayerHitText;
    [SerializeField] private TextMeshProUGUI PlayerCritText;
    [SerializeField] private TextMeshProUGUI EnemyUnitNameText;
    [SerializeField] private TextMeshProUGUI EnemyAttackText;
    [SerializeField] private TextMeshProUGUI EnemyHitText;
    [SerializeField] private TextMeshProUGUI EnemyCritText;
    [SerializeField] private TextMeshProUGUI EnemyCurrentHPText;

    // Start is called before the first frame update
    void Start()
    {
        InputManager inputManager = FindObjectOfType<InputManager>();
        inputManager.RegisterMouseOverTileEvent(OnTileHover);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTileHover(MapTile mapTile, MapTile previousTile, bool isTileAccessible, bool isTileAttackable)
    {
        Unit selectedUnit = BattleManager.Instance.SelectedUnit;
        if (selectedUnit != null && isTileAttackable)
        {
            Unit targetUnit = BattleManager.Instance.GetUnitAtTile(mapTile);
            if (targetUnit == null || targetUnit == selectedUnit)
            {
                return;
            }
            PlayerUnitNameText.text = selectedUnit.Name;
            PlayerCurrentHPText.text = selectedUnit.CurrentHealthPoints.ToString();
            PlayerAttackText.text = BattleManager.CalculateDamage(selectedUnit, targetUnit).ToString();
            PlayerHitText.text = BattleManager.CalculateHitChance(selectedUnit, targetUnit).ToString() + "%";
            PlayerCritText.text = BattleManager.CalculateCritChance(selectedUnit, targetUnit).ToString() + "%";
            EnemyUnitNameText.text = targetUnit.Name;
            EnemyCurrentHPText.text = targetUnit.CurrentHealthPoints.ToString();
            EnemyAttackText.text = BattleManager.CalculateDamage(targetUnit, selectedUnit).ToString();
            EnemyHitText.text = BattleManager.CalculateHitChance(targetUnit, selectedUnit).ToString() + "%";
            EnemyCritText.text = BattleManager.CalculateCritChance(targetUnit, selectedUnit).ToString() + "%";
        }
    }
}
