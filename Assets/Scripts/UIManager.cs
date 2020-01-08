using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    private PlayerPhaseStartHUD PlayerPhase;
    private EnemyPhaseStartHUD EnemyPhase;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        PlayerPhase = FindObjectOfType<PlayerPhaseStartHUD>();
        EnemyPhase = FindObjectOfType<EnemyPhaseStartHUD>();
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
}
