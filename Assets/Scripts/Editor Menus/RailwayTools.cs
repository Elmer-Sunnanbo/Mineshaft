using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class RailwayTools : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [MenuItem("Railway Tools/Snap and connect")]
    static void SnapAndConnectRails()
    {
        List<RailTile> tiles = new List<RailTile>();
        if (Selection.gameObjects.Length > 0)
        {
            foreach (GameObject tile in Selection.gameObjects)
            {
                if(tile.TryGetComponent(out RailTile foundRailTile))
                {
                    tiles.Add(foundRailTile);
                }
            }
        }
        else
        {
            tiles = FindObjectsByType<RailTile>(FindObjectsSortMode.None).ToList();
        }
        List<Vector2> positions = new List<Vector2>();
        foreach (RailTile tile in tiles)
        {
            Vector2 roundedPosition = new Vector2(Mathf.Round(tile.transform.position.x), Mathf.Round(tile.transform.position.y));
            positions.Add(roundedPosition);
            SerializedObject serializedTransform = new SerializedObject(tile.transform);
            serializedTransform.FindProperty("m_LocalPosition").vector3Value = roundedPosition;
            serializedTransform.ApplyModifiedProperties();
        }
        if(tiles.Count != positions.Count)
        {
            Debug.LogError("SnapAndConnectRails amount of positions and tiles did not match");
            return;
        }
        for (int i = 0; i < tiles.Count; i++)
        {
            SerializedObject serializedRailTile = new SerializedObject(tiles[i]);
            serializedRailTile.FindProperty("neighbours").GetArrayElementAtIndex(0).objectReferenceValue = CheckForTileAtVector(positions[i] + Vector2.up, tiles, positions);
            serializedRailTile.FindProperty("neighbours").GetArrayElementAtIndex(1).objectReferenceValue = CheckForTileAtVector(positions[i] + Vector2.right, tiles, positions);
            serializedRailTile.FindProperty("neighbours").GetArrayElementAtIndex(2).objectReferenceValue = CheckForTileAtVector(positions[i] + Vector2.down, tiles, positions);
            serializedRailTile.FindProperty("neighbours").GetArrayElementAtIndex(3).objectReferenceValue = CheckForTileAtVector(positions[i] + Vector2.left, tiles, positions);

            //Count number of neighbours
            int count = 0;
            if (serializedRailTile.FindProperty("neighbours").GetArrayElementAtIndex(0).objectReferenceValue != null) { count++; }
            if (serializedRailTile.FindProperty("neighbours").GetArrayElementAtIndex(1).objectReferenceValue != null) { count++; }
            if (serializedRailTile.FindProperty("neighbours").GetArrayElementAtIndex(2).objectReferenceValue != null) { count++; }
            if (serializedRailTile.FindProperty("neighbours").GetArrayElementAtIndex(3).objectReferenceValue != null) { count++; }
            if (count == 0) { Debug.LogWarning("SnapAndConnectRails found orphan rail"); }
            if (count == 3 && !serializedRailTile.FindProperty("isTurntable").boolValue) { Debug.LogWarning("SnapAndConnectRails found T rail"); }

            serializedRailTile.ApplyModifiedProperties();
        }
    }

    [MenuItem("Railway Tools/Auto Sprite")]
    static void AutoSprite()
    {
        List<RailTile> tiles = new List<RailTile>();
        if (Selection.gameObjects.Length > 0)
        {
            foreach (GameObject tile in Selection.gameObjects)
            {
                if (tile.TryGetComponent(out RailTile foundRailTile))
                {
                    tiles.Add(foundRailTile);
                }
            }
        }
        else
        {
            tiles = FindObjectsByType<RailTile>(FindObjectsSortMode.None).ToList();
        }
        foreach (RailTile tile in tiles)
        {
            tile.AutoSetSprite();
        }
        if(tiles.Count > 0)
        {
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }
    }

    [MenuItem("Railway Tools/Auto Roommanager")]
    static void AutoManager()
    {
        List<GameObject> rooms = Selection.gameObjects.ToList();
        foreach (GameObject room in rooms)
        {
            RoomManager latestManager;
            if (!room.TryGetComponent(out latestManager))
            {
                latestManager = room.AddComponent<RoomManager>();
            }
            
            List<RailTile> railTiles = room.GetComponentsInChildren<RailTile>().ToList();

            RailTile leftMostTile = null;
            RailTile rightMostTile = null;
            RailTile topMostTile = null;
            RailTile bottomMostTile = null;
            foreach (RailTile tile in railTiles)
            {
                if(leftMostTile != null)
                {
                    if(leftMostTile.transform.position.x > tile.transform.position.x)
                    {
                        leftMostTile = tile;
                    }
                }
                else
                {
                    leftMostTile = tile;
                }

                if (rightMostTile != null)
                {
                    if (rightMostTile.transform.position.x < tile.transform.position.x)
                    {
                        rightMostTile = tile;
                    }
                }
                else
                {
                    rightMostTile = tile;
                }

                if (topMostTile != null)
                {
                    if (topMostTile.transform.position.y < tile.transform.position.y)
                    {
                        topMostTile = tile;
                    }
                }
                else
                {
                    topMostTile = tile;
                }

                if (bottomMostTile != null)
                {
                    if (bottomMostTile.transform.position.y > tile.transform.position.y)
                    {
                        bottomMostTile = tile;
                    }
                }
                else
                {
                    bottomMostTile = tile;
                }
            }

            latestManager.borderRailNorth = topMostTile;
            latestManager.borderRailSouth = bottomMostTile;
            latestManager.borderRailEast = rightMostTile;
            latestManager.borderRailWest = leftMostTile;

            latestManager.enemiesInRoom = new List<GameObject>();
            for (int i = 0; i < room.transform.childCount; i++)
            {
                if (room.transform.GetChild(i).TryGetComponent<IEnemy>(out _))
                {
                    latestManager.enemiesInRoom.Add(room.transform.GetChild(i).gameObject);
                }
            }
        }


        if (rooms.Count > 0)
        {
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }
    }

    static RailTile CheckForTileAtVector(Vector2 vector, List<RailTile> tiles, List<Vector2> vectors)
    {
        for(int i = 0; i < vectors.Count; i++)
        {
            if(vectors[i] == vector)
            {
                return tiles[i];
            }
        }
        return null;
    }
}
