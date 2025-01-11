using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using UnityEditor.SceneManagement;

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
        List<RailTile> tiles = FindObjectsByType<RailTile>(FindObjectsSortMode.None).ToList();
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
            if (count == 3) { Debug.LogWarning("SnapAndConnectRails found T rail"); }

            serializedRailTile.ApplyModifiedProperties();
        }


        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
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
