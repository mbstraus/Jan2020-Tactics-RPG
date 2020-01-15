using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BattlePhaseState
{
    protected BattleManager battlePhaseManager;

    public abstract void Tick();

    public virtual void OnStateEnter() { }
    public virtual void OnStateExit() { }

    public abstract void UnitMoved(Unit unit);

    public BattlePhaseState()
    {

    }

    public BattlePhaseState(BattleManager battlePhaseManager)
    {
        this.battlePhaseManager = battlePhaseManager;
    }
}
