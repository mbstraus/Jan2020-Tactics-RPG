using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPhaseState : BattlePhaseState
{
    public EnemyPhaseState() : base()
    {

    }

    public EnemyPhaseState(BattlePhaseManager battlePhaseManager) : base(battlePhaseManager)
    {

    }

    public override void Tick()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            battlePhaseManager.SetState(new PlayerPhaseState(battlePhaseManager));
        }
    }

    public override void OnStateEnter()
    {
        UIManager.Instance.ShowEnemyPhase();
    }
}
