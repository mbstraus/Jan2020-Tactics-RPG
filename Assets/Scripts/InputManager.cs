using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private Camera activeCamera;
    [SerializeField] private GameObject ActiveTilePrefab;
    private GameObject activeTileInstance;
    private MapTile lastMapTile;

    public delegate void TileHoverEvent(MapTile tile);
    private TileHoverEvent OnTileHover;

    public delegate void TileSelectedEvent(MapTile selectedTile, Vector3 mousePosition);
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

    void Update()
    {
        Vector3 mouseLocation = activeCamera.ScreenToWorldPoint(Input.mousePosition);
        MapTile mapTile = BattleManager.Instance.GetTileAt((int)mouseLocation.x, (int)mouseLocation.y);
        if (mouseLocation.x <= 0 || mouseLocation.y <= 0)
        {
            mapTile = null;
        }
        if (lastMapTile == null || lastMapTile != mapTile)
        {
            OnTileHover(mapTile);
            lastMapTile = mapTile;
        }

        if (Input.GetMouseButtonDown(0) && mapTile != null)
        {
            OnTileSelected(mapTile, mouseLocation);
        }
    }
}
