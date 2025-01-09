using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeBuilder : MonoBehaviour
{
    [SerializeField] int roomSize;
    [Header("Rooms")]
    [SerializeField] List<GameObject> Rooms0;
    [SerializeField] List<GameObject> Roomsi;
    [SerializeField] List<GameObject> RoomsI;
    [SerializeField] List<GameObject> RoomsL;
    [SerializeField] List<GameObject> RoomsT;
    [SerializeField] List<GameObject> RoomsPlus;
    [SerializeField] List<GameObject> RoomsStart;
    [SerializeField] List<GameObject> RoomsTreasure;
    [SerializeField] List<GameObject> RoomsEnd;

    MazeGenerator mazeGen;
    void Start()
    {
        mazeGen = GetComponent<MazeGenerator>();
        Maze maze = mazeGen.GetMaze();
        int minX = -maze.negativeWidth + 1;
        int maxX = maze.positiveWidth - 1;
        int minY = -maze.negativeHeight + 1;
        int maxY = maze.positiveHeight - 1;
        for (int x = minX; x <= maxX; x++)
        {
            Debug.Log("X = " + x);
            for (int y = minY; y <= maxY; y++)
            {
                Instantiate(GetTile(maze[x][y].tileType), GetWorldPosition(new Vector2(x,y)), Quaternion.identity);
            }
        }
    }

    Vector2 GetWorldPosition(Vector2 tilePosition)
    {
        return tilePosition * roomSize;
    }

    GameObject GetTile(TileTypes tileType)
    {
        switch(tileType)
        {
            case TileTypes.Empty:
                return GetRandomFromlist(Rooms0);
            case TileTypes.Passage:
                return GetRandomFromlist(RoomsPlus);
            case TileTypes.Treasure:
                return GetRandomFromlist(RoomsTreasure);
            case TileTypes.Start:
                return GetRandomFromlist(RoomsStart);
            case TileTypes.End:
                return GetRandomFromlist(RoomsEnd);
        }
        return GetComponent<GameObject>();
    }
    Type GetRandomFromlist<Type>(List<Type> List)
    {
        return List[Random.Range(0, List.Count)];
    }
}
