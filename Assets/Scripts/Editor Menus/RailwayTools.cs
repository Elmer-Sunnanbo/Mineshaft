using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class RailwayTools : MonoBehaviour
{
    [MenuItem("Building Tools/Snap and connect")]
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

    [MenuItem("Building Tools/Auto Sprite")]
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

    [MenuItem("Building Tools/Auto Roommanager")]
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
                if (-3.5f > tile.transform.position.x)
                {
                    leftMostTile = tile;
                }

                if (3.5f < tile.transform.position.x)
                {
                    rightMostTile = tile;
                }

                if (3.5f < tile.transform.position.y)
                {
                    topMostTile = tile;
                }

                if (-3.5f > tile.transform.position.y)
                {
                    bottomMostTile = tile;
                }
            }
            SerializedObject serManager = new SerializedObject(latestManager);
            SerializedProperty serNorth = serManager.FindProperty("borderRailNorth");
            SerializedProperty serSouth = serManager.FindProperty("borderRailSouth");
            SerializedProperty serEast = serManager.FindProperty("borderRailEast");
            SerializedProperty serWest = serManager.FindProperty("borderRailWest");
            serNorth.objectReferenceValue = topMostTile;
            serSouth.objectReferenceValue = bottomMostTile;
            serEast.objectReferenceValue = rightMostTile;
            serWest.objectReferenceValue = leftMostTile;
            serManager.ApplyModifiedProperties();
            /*
            latestManager.borderRailNorth = topMostTile;
            latestManager.borderRailSouth = bottomMostTile;
            latestManager.borderRailEast = rightMostTile;
            latestManager.borderRailWest = leftMostTile;
            */

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

    [MenuItem("Building Tools/Align Room")]
    static void AlignRoom()
    {
        List<GameObject> rooms = Selection.gameObjects.ToList();
        foreach (GameObject room in rooms)
        {
            if(!room.TryGetComponent<RoomManager>(out _)) //If it's not a room
            {
                continue;
            }

            room.transform.position = Vector2.zero;
            for(int roomChildIndex = 0; roomChildIndex < room.transform.childCount; roomChildIndex++)
            {
                GameObject currentChild = room.transform.GetChild(roomChildIndex).gameObject;
                if(currentChild.TryGetComponent(out Grid foundGrid))
                {
                    foundGrid.transform.position = Vector2.zero;
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
