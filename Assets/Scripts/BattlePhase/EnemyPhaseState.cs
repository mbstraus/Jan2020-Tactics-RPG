using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPhaseState : BattlePhaseState
{
    private List<Unit> RemainingUnits;

    public EnemyPhaseState() : base()
    {

    }

    public EnemyPhaseState(BattleManager battlePhaseManager) : base(battlePhaseManager)
    {
        RemainingUnits = new List<Unit>(battlePhaseManager.EnemyUnits);
    }

    public override void Tick()
    {
        if (IsPhaseStarted)
        {
            battlePhaseManager.SetState(new PlayerPhaseState(battlePhaseManager));
        }
    }

    public override void OnStateEnter()
    {
        UIManager.Instance.ShowEnemyPhase(PhaseStart);
    }

    public void EnemyPhaseStart()
    {
        // There is no AI right now, so just immediately end the phase.
        battlePhaseManager.SetState(new PlayerPhaseState(battlePhaseManager));
    }

    public override void UnitMoved(Unit unit)
    {
        RemainingUnits.Remove(unit);
        if (RemainingUnits.Count == 0)
        {
            battlePhaseManager.SetState(new EnemyPhaseState(battlePhaseManager));
        }
    }
}
