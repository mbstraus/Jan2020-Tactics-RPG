using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private Camera activeCamera;
    [SerializeField] private GameObject ActiveTilePrefab;
    private GameObject activeTileInstance;
    private MapTile lastMapTile;

    public delegate void TileHoverEvent(MapTile tile, MapTile previousTile, bool isTileAccessible, bool isTileAttackable);
    private TileHoverEvent OnTileHover;

    public delegate void TileSelectedEvent(MapTile selectedTile, MapTile previousTile, Vector3 mousePosition, bool isTileAccessible, bool isTileAttackable);
    private TileSelectedEvent OnTileSelected;

    void Start()
    {
        if (activeCamera == null)
        {
            activeCamera = Camera.main;
        }
        activeTileInstance = Instantiate(ActiveTilePrefab);
        activeTileInstance.SetActive(false);
    }

    public void RegisterMouseOverTileEvent(TileHoverEvent onTileHover)
    {
        OnTileHover += onTileHover;
    }
    public void UnregisterMouseOverTileEvent(TileHoverEvent onTileHover)
    {
        OnTileHover -= onTileHover;
    }

    public void RegisterTileSelectedEvent(TileSelectedEvent onTileSelected)
    {
        OnTileSelected += onTileSelected;
    }
    public void UnregisterTileSelectedEvent(TileSelectedEvent onTileSelected)
    {
        OnTileSelected -= onTileSelected;
    }

    private void Update()
    {
        Vector3 mouseLocation = activeCamera.ScreenToWorldPoint(Input.mousePosition);
        MapTile mapTile = BattleManager.Instance.GetTileAt((int)mouseLocation.x, (int)mouseLocation.y);
        bool isTileAccessible = IsSelectedTileAccessible(mouseLocation);
        bool isTileAttackable = IsSelectedTileAttackable(mouseLocation);
        if (mouseLocation.x <= 0 || mouseLocation.y <= 0)
        {
            mapTile = null;
        }
        if (lastMapTile == null || lastMapTile != mapTile)
        {
            if (isTileAccessible)
            {
                lastMapTile = mapTile;
            }
        }
        OnTileHover(mapTile, lastMapTile, isTileAccessible, isTileAttackable);

        if (Input.GetMouseButtonDown(0) && mapTile != null)
        {
            OnTileSelected(mapTile, lastMapTile, mouseLocation, isTileAccessible, isTileAttackable);
        }
    }

    private bool IsSelectedTileAccessible(Vector3 mouseLocation)
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(mouseLocation, Vector2.zero);
        bool hasUnit = false;
        bool isAccessible = false;
        foreach (var hit in hits)
        {
            if (hit.collider != null && hit.collider.gameObject.GetComponent<MoveRangeIndicator>() != null)
            {
                isAccessible = true;
            }
            if (hit.collider != null && hit.collider.gameObject.GetComponent<Unit>() != null)
            {
                Unit tileUnit = hit.collider.gameObject.GetComponent<Unit>();
                if (tileUnit.CurrentHealthPoints <= 0)
                {
                    continue;
                }
                hasUnit = true;
            }
        }
        return isAccessible && !hasUnit;
    }

    private bool IsSelectedTileAttackable(Vector3 mouseLocation)
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(mouseLocation, Vector2.zero);
        bool hasEnemy = false;
        bool isAttackable = false;
        foreach (var hit in hits)
        {
            if (hit.collider != null)
            {
                if (hit.collider.gameObject.GetComponent<AttackRangeIndicator>() != null || hit.collider.gameObject.GetComponent<MoveRangeIndicator>() != null)
                {
                    isAttackable = true;
                }
                if (hit.collider.gameObject.GetComponent<Unit>() != null)
                {
                    Unit unit = hit.collider.gameObject.GetComponent<Unit>();
                    if (unit.CurrentHealthPoints > 0 && unit.Team == Unit.UnitTeam.ENEMY)
                    {
                        hasEnemy = true;
                    }
                }
            }
        }
        bool isSelectedTileAttackable = isAttackable && hasEnemy;
        return isSelectedTileAttackable;
    }
}
