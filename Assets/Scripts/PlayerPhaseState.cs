using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPhaseState : BattlePhaseState
{
    public PlayerPhaseState() : base()
    {

    }

    public PlayerPhaseState(BattlePhaseManager battlePhaseManager) : base(battlePhaseManager)
    {

    }

    public override void Tick()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            battlePhaseManager.SetState(new EnemyPhaseState(battlePhaseManager));
        }

        /*
        Unit selectedUnit = battlePhaseManager.SelectedUnit;
        if (selectedUnit != null)
        {
            List<MapTile> accessableTiles = CalculateMoveRange(selectedUnit);
            UIManager.Instance.ShowMoveRange(accessableTiles);
        }
        */
    }

    public override void OnStateEnter()
    {
        UIManager.Instance.ShowPlayerPhase();
    }
}
