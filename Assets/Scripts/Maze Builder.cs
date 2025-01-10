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
            for (int y = minY; y <= maxY; y++)
            {
                int turns = 0;
                GameObject selectedTile = null; //Only assigned because it needs to be something
                //Determine room type and how many times to turn it
                switch(maze[x][y].tileType)
                {
                    case TileTypes.Empty:
                        selectedTile = GetRandomFromlist(Rooms0);
                        break;
                    case TileTypes.Start:
                        selectedTile = GetRandomFromlist(RoomsStart);
                        if (maze[x][y].GetPath(new Direction(0)))
                        {
                            turns = 0;
                        }
                        if (maze[x][y].GetPath(new Direction(1)))
                        {
                            turns = 3;
                        }
                        if (maze[x][y].GetPath(new Direction(2)))
                        {
                            turns = 2;
                        }
                        if (maze[x][y].GetPath(new Direction(3)))
                        {
                            turns = 1;
                        }
                        break;
                    case TileTypes.End:
                        selectedTile = GetRandomFromlist(RoomsEnd);
                        if (maze[x][y].GetPath(new Direction(0)))
                        {
                            turns = 0;
                        }
                        if (maze[x][y].GetPath(new Direction(1)))
                        {
                            turns = 3;
                        }
                        if (maze[x][y].GetPath(new Direction(2)))
                        {
                            turns = 2;
                        }
                        if (maze[x][y].GetPath(new Direction(3)))
                        {
                            turns = 1;
                        }
                        break;
                    case TileTypes.Treasure:
                        selectedTile = GetRandomFromlist(RoomsTreasure);
                        if (maze[x][y].GetPath(new Direction(0)))
                        {
                            turns = 0;
                        }
                        if (maze[x][y].GetPath(new Direction(1)))
                        {
                            turns = 3;
                        }
                        if (maze[x][y].GetPath(new Direction(2)))
                        {
                            turns = 2;
                        }
                        if (maze[x][y].GetPath(new Direction(3)))
                        {
                            turns = 1;
                        }
                        break;
                    case TileTypes.Passage:
                        switch (GetOpenPathsCount(maze[x][y]))
                        {
                            case 0:
                                //It's an empty tile
                                selectedTile = GetRandomFromlist(Rooms0);
                                Debug.LogWarning("Mazebuilder found passage room with 0 paths");
                                break;
                            case 1:
                                //It's dead end tile
                                selectedTile = GetRandomFromlist(Roomsi);
                                if (maze[x][y].GetPath(new Direction(0)))
                                {
                                    turns = 0;
                                }
                                if (maze[x][y].GetPath(new Direction(1)))
                                {
                                    turns = 3;
                                }
                                if (maze[x][y].GetPath(new Direction(2)))
                                {
                                    turns = 2;
                                }
                                if (maze[x][y].GetPath(new Direction(3)))
                                {
                                    turns = 1;
                                }
                                break;
                            case 2:
                                //It's a curve or a straight
                                if (maze[x][y].GetPath(new Direction(0)))
                                {
                                    if (maze[x][y].GetPath(new Direction(2)))
                                    {
                                        //N-S straight
                                        selectedTile = GetRandomFromlist(RoomsI);
                                    }
                                    else if (maze[x][y].GetPath(new Direction(1)))
                                    {
                                        //N-E curve
                                        selectedTile = GetRandomFromlist(RoomsL);
                                    }
                                    else
                                    {
                                        //N-W curve
                                        //Only option left
                                        selectedTile = GetRandomFromlist(RoomsL);
                                        turns = 1;
                                    }
                                }
                                else if (maze[x][y].GetPath(new Direction(1)))
                                {
                                    if (maze[x][y].GetPath(new Direction(3)))
                                    {
                                        //E-W straight
                                        selectedTile = GetRandomFromlist(RoomsI);
                                        turns = 1;
                                    }
                                    else
                                    {
                                        //E-S curve
                                        //Only option left (N-E curve was chaught above)
                                        selectedTile = GetRandomFromlist(RoomsL);
                                        turns = 3;
                                    }
                                }
                                else
                                {
                                    //W-S curve
                                    //Only option left if north and east don't have rail
                                    selectedTile = GetRandomFromlist(RoomsL);
                                    turns = 2;
                                }
                                break;
                            case 3:
                                //It's a T
                                selectedTile = GetRandomFromlist(RoomsT);
                                if (!maze[x][y].GetPath(new Direction(0)))
                                {
                                    turns = 0;
                                }
                                if (!maze[x][y].GetPath(new Direction(1)))
                                {
                                    turns = 3;
                                }
                                if (!maze[x][y].GetPath(new Direction(2)))
                                {
                                    turns = 2;
                                }
                                if (!maze[x][y].GetPath(new Direction(3)))
                                {
                                    turns = 1;
                                }
                                break;
                            case 4:
                                //It's a cross
                                selectedTile = GetRandomFromlist(RoomsPlus);
                                break;
                        }
                        break;
                }
                Quaternion rotation = Quaternion.Euler(0,0,turns * 90);
                if(selectedTile != null)
                {
                    GameObject latestRoom = Instantiate(selectedTile, GetWorldPosition(new Vector2(x, y)), rotation);
                    latestRoom.transform.parent = transform;
                }
                else
                {
                    Debug.LogError("MazeBuilder failed to select a room type for a room");
                }
            }
        }
    }

    Vector2 GetWorldPosition(Vector2 tilePosition)
    {
        return tilePosition * roomSize;
    }

    GameObject GetTileBasic(MazeTile tile)
    {
        switch(tile.tileType)
        {
            case TileTypes.Empty:
                return GetRandomFromlist(Rooms0);
            case TileTypes.Passage:
                Debug.LogError("MazeBuilder's GetTile was fed a passage");
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

    int GetOpenPathsCount(MazeTile tile)
    {
        int openPaths = 0;
        if (tile.GetPath(new Direction(0))) { openPaths++; }
        if (tile.GetPath(new Direction(1))) { openPaths++; }
        if (tile.GetPath(new Direction(2))) { openPaths++; }
        if (tile.GetPath(new Direction(3))) { openPaths++; }
        return openPaths;
    }
}
