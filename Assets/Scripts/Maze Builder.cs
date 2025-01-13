using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeBuilder : MonoBehaviour
{
    [SerializeField] int roomSize;
    [Header("Rooms")]
    [SerializeField] List<GameObject> Rooms0;
    [SerializeField] List<GameObject> Roomsi;
    [SerializeField] List<GameObject> Roomsi1;
    [SerializeField] List<GameObject> Roomsi2;
    [SerializeField] List<GameObject> Roomsi3;
    [SerializeField] List<GameObject> RoomsI;
    [SerializeField] List<GameObject> RoomsI1;
    [SerializeField] List<GameObject> RoomsL;
    [SerializeField] List<GameObject> RoomsL1;
    [SerializeField] List<GameObject> RoomsL2;
    [SerializeField] List<GameObject> RoomsL3;
    [SerializeField] List<GameObject> RoomsT;
    [SerializeField] List<GameObject> RoomsT1;
    [SerializeField] List<GameObject> RoomsT2;
    [SerializeField] List<GameObject> RoomsT3;
    [SerializeField] List<GameObject> RoomsPlus;
    [SerializeField] List<GameObject> RoomsStart;
    [SerializeField] List<GameObject> RoomsTreasure;
    [SerializeField] List<GameObject> RoomsTreasure1;
    [SerializeField] List<GameObject> RoomsTreasure2;
    [SerializeField] List<GameObject> RoomsTreasure3;
    [SerializeField] List<GameObject> RoomsEnd;
    [SerializeField] List<GameObject> RoomsEnd1;
    [SerializeField] List<GameObject> RoomsEnd2;
    [SerializeField] List<GameObject> RoomsEnd3;

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
                GameObject selectedTile = null; //Only assigned because it needs to be something
                //Determine room type and how many times to turn it
                switch(maze[x][y].tileType)
                {
                    case TileTypes.Empty:
                        selectedTile = GetRandomFromlist(Rooms0);
                        break;
                    case TileTypes.Start:
                        selectedTile = GetRandomFromlist(RoomsStart);
                        break;
                    case TileTypes.End:
                        if (maze[x][y].GetPath(new Direction(0)))
                        {
                            selectedTile = GetRandomFromlist(RoomsEnd);
                        }
                        if (maze[x][y].GetPath(new Direction(1)))
                        {
                            selectedTile = GetRandomFromlist(RoomsEnd1);
                        }
                        if (maze[x][y].GetPath(new Direction(2)))
                        {
                            selectedTile = GetRandomFromlist(RoomsEnd2);
                        }
                        if (maze[x][y].GetPath(new Direction(3)))
                        {
                            selectedTile = GetRandomFromlist(RoomsEnd3);
                        }
                        break;
                    case TileTypes.Treasure:
                        
                        if (maze[x][y].GetPath(new Direction(0)))
                        {
                            selectedTile = GetRandomFromlist(RoomsTreasure);
                        }
                        if (maze[x][y].GetPath(new Direction(1)))
                        {
                            selectedTile = GetRandomFromlist(RoomsTreasure1);
                        }
                        if (maze[x][y].GetPath(new Direction(2)))
                        {
                            selectedTile = GetRandomFromlist(RoomsTreasure2);
                        }
                        if (maze[x][y].GetPath(new Direction(3)))
                        {
                            selectedTile = GetRandomFromlist(RoomsTreasure3);
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
                                if (maze[x][y].GetPath(new Direction(0)))
                                {
                                    selectedTile = GetRandomFromlist(Roomsi);
                                }
                                if (maze[x][y].GetPath(new Direction(1)))
                                {
                                    selectedTile = GetRandomFromlist(Roomsi1);
                                }
                                if (maze[x][y].GetPath(new Direction(2)))
                                {
                                    selectedTile = GetRandomFromlist(Roomsi2);
                                }
                                if (maze[x][y].GetPath(new Direction(3)))
                                {
                                    selectedTile = GetRandomFromlist(Roomsi3);
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
                                        selectedTile = GetRandomFromlist(RoomsL3);
                                    }
                                }
                                else if (maze[x][y].GetPath(new Direction(1)))
                                {
                                    if (maze[x][y].GetPath(new Direction(3)))
                                    {
                                        //E-W straight
                                        selectedTile = GetRandomFromlist(RoomsI1);
                                    }
                                    else
                                    {
                                        //E-S curve
                                        //Only option left (N-E curve was chaught above)
                                        selectedTile = GetRandomFromlist(RoomsL1);
                                    }
                                }
                                else
                                {
                                    //W-S curve
                                    //Only option left if north and east don't have rail
                                    selectedTile = GetRandomFromlist(RoomsL2);
                                }
                                break;
                            case 3:
                                //It's a T
                                if (!maze[x][y].GetPath(new Direction(0)))
                                {
                                    selectedTile = GetRandomFromlist(RoomsT);
                                }
                                if (!maze[x][y].GetPath(new Direction(1)))
                                {
                                    selectedTile = GetRandomFromlist(RoomsT1);
                                }
                                if (!maze[x][y].GetPath(new Direction(2)))
                                {
                                    selectedTile = GetRandomFromlist(RoomsT2);
                                }
                                if (!maze[x][y].GetPath(new Direction(3)))
                                {
                                    selectedTile = GetRandomFromlist(RoomsT3);
                                }
                                break;
                            case 4:
                                //It's a cross
                                selectedTile = GetRandomFromlist(RoomsPlus);
                                break;
                        }
                        break;
                }
                if(selectedTile != null)
                {
                    GameObject latestRoom = Instantiate(selectedTile, GetWorldPosition(new Vector2(x, y)), Quaternion.identity);
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
