using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class InputManager : MonoBehaviour
{
    private Camera activeCamera;
    private Tilemap tilemap;
    [SerializeField] private GameObject ActiveTilePrefab;
    private GameObject activeTileInstance;

    void Start()
    {
        activeCamera = Camera.main;
        tilemap = FindObjectOfType<Tilemap>();
        activeTileInstance = Instantiate(ActiveTilePrefab);
        activeTileInstance.SetActive(false);
    }

    void Update()
    {
        Vector3 mouseLocation = activeCamera.ScreenToWorldPoint(Input.mousePosition);
        MapTile mapTile = BattlePhaseManager.Instance.GetTileAt((int)mouseLocation.x, (int)mouseLocation.y);
        if (mouseLocation.x >= 0 && mouseLocation.y >= 0 && mapTile != null)
        {
            activeTileInstance.transform.position = new Vector3(mapTile.GridPosition.x + 0.5f, mapTile.GridPosition.y + 0.5f, 0f);
            activeTileInstance.SetActive(true);
        }
        else
        {
            activeTileInstance.SetActive(false);
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (mouseLocation.x < 0 || mouseLocation.y < 0)
            {
                Debug.Log("Tile clicked out of bounds, Original mouse click: " + mouseLocation);
            }
            else if (mapTile != null)
            {
                Debug.Log("Tile clicked at " + mapTile.GridPosition + " - Tile Type: " + mapTile.TileType + ", Original mouse click: " + mouseLocation);
                Unit tileUnit = BattlePhaseManager.Instance.GetUnitAtTile(mapTile);
                if (tileUnit != null)
                {
                    UIManager.Instance.SetSelectedUnit(tileUnit);
                }
                else
                {
                    UIManager.Instance.SetSelectedUnit(null);
                }
            }
        }
    }
}
