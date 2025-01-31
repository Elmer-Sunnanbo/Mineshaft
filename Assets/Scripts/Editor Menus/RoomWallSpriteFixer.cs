using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class RoomWallSpriteFixer : MonoBehaviour
{
    [SerializeField] Tile topTile;
    [SerializeField] Tile frontTile;
    [SerializeField] Tile floorTile;
    [ContextMenu("Tilemap Pass")]
    void TileMapFixing()
    {
        List<GameObject> rooms = new List<GameObject>();
        foreach (RoomManager room in FindObjectsByType<RoomManager>(FindObjectsSortMode.None))
        {
            rooms.Add(room.gameObject);
        }
        
        Tilemap walls = null;
        foreach (GameObject room in rooms)
        {
            Grid grid = room.GetComponentInChildren<Grid>();
            if (grid.GetComponentInChildren<TilemapCollider2D>() != null)
            {
                walls = grid.GetComponentInChildren<TilemapCollider2D>().GetComponent<Tilemap>();
            }
            if (walls != null)
            {
                GameObject newFloorObj = Instantiate(new GameObject());
                GameObject newTopWallObj = Instantiate(new GameObject());
                GameObject newSideWallObj = Instantiate(new GameObject());
                newFloorObj.transform.parent = grid.transform;
                newTopWallObj.transform.parent = grid.transform;
                newSideWallObj.transform.parent = grid.transform;
                newFloorObj.transform.localPosition = Vector2.zero;
                newTopWallObj.transform.localPosition = Vector2.zero;
                newSideWallObj.transform.localPosition = Vector2.zero;
                Tilemap newFloor = newFloorObj.AddComponent<Tilemap>();
                Tilemap newTopWall = newTopWallObj.AddComponent<Tilemap>();
                Tilemap newSideWall = newSideWallObj.AddComponent<Tilemap>();
                if (grid.GetComponentInChildren<TilemapCollider2D>() != null)
                {
                    DestroyImmediate(grid.GetComponentInChildren<TilemapRenderer>().gameObject);
                }
                newFloorObj.AddComponent<TilemapRenderer>().sortingLayerName = "Floor";
                newTopWallObj.AddComponent<TilemapRenderer>().sortingLayerName = "TopWalls";
                newSideWallObj.AddComponent<TilemapRenderer>().sortingLayerName = "FrontWalls";
                newFloor.transform.localPosition = new Vector2(0.5f, 0.5f);
                newTopWall.transform.localPosition = new Vector2(0.5f, 0.5f);
                newSideWall.transform.localPosition = new Vector2(0.5f, 0.5f);
                DestroyImmediate(walls.GetComponent<TilemapRenderer>());
                
                for (int x = -5; x <= 3; x++)
                {
                    for (int y = -5; y <= 3; y++)
                    {
                        if(walls.GetTile(new Vector3Int(x, y)))
                        {
                            newSideWall.SetTile(new Vector3Int(x, y), frontTile);
                            newTopWall.SetTile(new Vector3Int(x, y + 1), topTile);
                        }
                        else
                        {
                            newFloor.SetTile(new Vector3Int(x, y), floorTile);
                        }
                    }
                }
            }
            else
            {

            }
        }


        if (rooms.Count > 0)
        {
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }
    }
}