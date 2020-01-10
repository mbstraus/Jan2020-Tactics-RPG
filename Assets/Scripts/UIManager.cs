using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    private Unit SelectedUnit;
    [SerializeField] private PlayerPhaseStartHUD PlayerPhase;
    [SerializeField] private EnemyPhaseStartHUD EnemyPhase;
    [SerializeField] private SelectedUnitHUD SelectedUnitHUD;
    [SerializeField] private TextMeshProUGUI LevelField;
    [SerializeField] private TextMeshProUGUI CurrentHPField;
    [SerializeField] private TextMeshProUGUI MaxHPField;
    [SerializeField] private TextMeshProUGUI SelectedUnitName;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SetSelectedUnit(null);
    }

    private void Update()
    {
        if (SelectedUnit != null)
        {
            LevelField.text = SelectedUnit.Level.ToString();
            CurrentHPField.text = SelectedUnit.CurrentHealthPoints.ToString();
            MaxHPField.text = SelectedUnit.MaxHealthPoints.ToString();
            SelectedUnitName.text = SelectedUnit.Name;
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

    public void SetSelectedUnit(Unit selectedUnit)
    {
        SelectedUnit = selectedUnit;
        if (selectedUnit != null)
        {
            SelectedUnitHUD.gameObject.SetActive(true);
        }
        else
        {
            SelectedUnitHUD.gameObject.SetActive(false);
        }
    }
}
