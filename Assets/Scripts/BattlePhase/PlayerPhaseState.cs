using System.Collections.Generic;
using UnityEngine;

public class PlayerPhaseState : BattlePhaseState
{
    private List<Unit> RemainingUnits;

    public PlayerPhaseState() : base()
    {

    }

    public PlayerPhaseState(BattleManager battlePhaseManager) : base(battlePhaseManager)
    {
        RemainingUnits = new List<Unit>(battlePhaseManager.PlayerUnits);
    }

    public override void Tick()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            battlePhaseManager.SetState(new EnemyPhaseState(battlePhaseManager));
        }
    }

    public override void OnStateEnter()
    {
        UIManager.Instance.ShowPlayerPhase();
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
