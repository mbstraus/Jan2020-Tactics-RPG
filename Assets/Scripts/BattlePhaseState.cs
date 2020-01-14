using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BattlePhaseState
{
    protected BattlePhaseManager battlePhaseManager;

    public abstract void Tick();

    public virtual void OnStateEnter() { }
    public virtual void OnStateExit() { }

    public abstract void UnitMoved(Unit unit);

    public BattlePhaseState()
    {

    }

    public BattlePhaseState(BattlePhaseManager battlePhaseManager)
    {
        this.battlePhaseManager = battlePhaseManager;
    }
}
