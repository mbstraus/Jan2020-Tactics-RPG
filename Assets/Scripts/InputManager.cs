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
            OnTileHover(mapTile, lastMapTile, isTileAccessible, isTileAttackable);
            if (isTileAccessible)
            {
                lastMapTile = mapTile;
            }
        }

        if (Input.GetMouseButtonDown(0) && mapTile != null)
        {
            OnTileSelected(mapTile, lastMapTile, mouseLocation, isTileAccessible, isTileAttackable);
        }
    }

    private bool IsSelectedTileAccessible(Vector3 mouseLocation)
    {
        RaycastHit2D hit = Physics2D.Raycast(mouseLocation, Vector2.zero);
        if (hit.collider != null && hit.collider.gameObject.GetComponent<MoveRangeIndicator>() != null)
        {
            return true;
        }
        return false;
    }

    private bool IsSelectedTileAttackable(Vector3 mouseLocation)
    {
        RaycastHit2D hit = Physics2D.Raycast(mouseLocation, Vector2.zero);
        if (hit.collider != null && hit.collider.gameObject.GetComponent<AttackRangeIndicator>() != null)
        {
            return true;
        }
        return false;
    }
}
