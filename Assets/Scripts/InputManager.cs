using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class InputManager : MonoBehaviour
{
    private Camera camera;
    private Tilemap tilemap;

    // Start is called before the first frame update
    void Start()
    {
        camera = Camera.main;
        tilemap = FindObjectOfType<Tilemap>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 clickedLocation = camera.ScreenToWorldPoint(Input.mousePosition);
            MapTile mapTile = BattlePhaseManager.Instance.GetTileAt((int)clickedLocation.x, (int)clickedLocation.y);
            if (clickedLocation.x < 0 || clickedLocation.y < 0)
            {
                Debug.Log("Tile clicked out of bounds, Original mouse click: " + clickedLocation);
            } else if (mapTile != null)
            {
                Debug.Log("Tile clicked at " + mapTile.GridPosition + " - Tile Type: " + mapTile.TileType + ", Original mouse click: " + clickedLocation);
            }
        }
    }
}
