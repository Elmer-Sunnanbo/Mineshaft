using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    static Direction startDirection = new Direction(0); //The direction the very first path will go in

    public Maze GetMaze()
    {
        return GenerateMaze();
    }

    /// <summary>
    /// Generates a random maze
    /// </summary>
    /// <returns></returns>
    static Maze GenerateMaze()
    {
        Maze finishedMaze = new Maze();
        Vector2 currentPos = new Vector2();

        //Generate pathways
        finishedMaze[currentPos].tileType = TileTypes.Start;
        RandomWalkDig(finishedMaze, currentPos, startDirection, 8, TileTypes.End, 0); //Main passage
        CreateCorridorSet(finishedMaze, 3, 2, TileTypes.Passage, 0); //Dead ends
        CreateCorridorSet(finishedMaze, 3, 3, TileTypes.Treasure, 0); //Paths to treasure
        CreateCorridorSet(finishedMaze, 2, 2, TileTypes.Treasure, 1); //Distant paths to treasure

        //Add borders
        finishedMaze.Extend(new Direction(0));
        finishedMaze.Extend(new Direction(0));
        finishedMaze.Extend(new Direction(1));
        finishedMaze.Extend(new Direction(1));
        finishedMaze.Extend(new Direction(2));
        finishedMaze.Extend(new Direction(2));
        finishedMaze.Extend(new Direction(3));
        finishedMaze.Extend(new Direction(3));

        FinishMaze(finishedMaze);

        return finishedMaze;
    }

    /// <summary>
    /// Randomly carves corridors into a maze
    /// </summary>
    /// <param name="maze"></param>
    /// <param name="count"></param>
    /// <param name="length"></param>
    /// <param name="endType"></param>
    /// <param name="proximity"></param>
    static void CreateCorridorSet(Maze maze, int count, int length, TileTypes endType, int proximity)
    {
        for (int i = 0; i < count; i++)
        {

            List<Vector2> expansionPoints = GetViableExpansionPoints(maze, proximity); //Get possible points for expansion
            if (expansionPoints.Count == 0) //Make sure there is at least one avaliable
            {
                return;
            }
            Vector2 expansionPoint = expansionPoints[UnityEngine.Random.Range(0, expansionPoints.Count)]; //Select a random one

            //Determine which directions are avaliable for digging
            int[] avaliableDirections = new int[4];
            if (maze.AttemptDig(expansionPoint, new Direction(0))) { avaliableDirections[0] = 1; }
            if (maze.AttemptDig(expansionPoint, new Direction(1))) { avaliableDirections[1] = 1; }
            if (maze.AttemptDig(expansionPoint, new Direction(2))) { avaliableDirections[2] = 1; }
            if (maze.AttemptDig(expansionPoint, new Direction(3))) { avaliableDirections[3] = 1; }

            //Randomly select one of the avaliable directions and dig a path starting in that direction
            RandomWalkDig(maze, expansionPoint, new Direction(GetWeightedRandomOption(avaliableDirections)), length, endType, proximity + 1);
        }
    }

    /// <summary>
    /// Carve a corridor into a maze using a random walk
    /// </summary>
    /// <param name="maze"></param>
    /// <param name="startPosition"></param>
    /// <param name="startDirection"></param>
    /// <param name="length"></param>
    /// <param name="finalTile"></param>
    /// <param name="proximity"></param>
    static void RandomWalkDig(Maze maze, Vector2 startPosition, Direction startDirection, int length, TileTypes finalTile, int proximity)
    {
        //Odds of going straight vs turning
        const int LeftWeight = 1;
        const int CenterWeight = 2;
        const int RightWeight = 1;

        Vector2 CurrentPosition = startPosition;
        Direction Direction = startDirection;

        CurrentPosition = maze.DigPath(CurrentPosition, Direction, proximity);//Dig the first path in the start direction (without checks)
        for (int i = 0; i < length - 1; i++)
        {
            //Weights of going different directions which become 0 when the path that way is blocked
            int LeftWeightChecked = 0;
            int CenterWeightChecked = 0;
            int RightWeightChecked = 0;
            //Check which directions are blocked
            if (maze.AttemptDig(CurrentPosition, Direction.Rotated(-1))) { LeftWeightChecked = LeftWeight; }
            if (maze.AttemptDig(CurrentPosition, Direction)) { CenterWeightChecked = CenterWeight; }
            if (maze.AttemptDig(CurrentPosition, Direction.Rotated(1))) { RightWeightChecked = RightWeight; }

            TileTypes NextTile = TileTypes.Passage; //Place a passage tile
            if (i == length - 2) { NextTile = finalTile; } //On final iteration, place the end tile instead

            //Move in a randomly selected direction
            int Option = GetWeightedRandomOption(new int[] { LeftWeightChecked, CenterWeightChecked, RightWeightChecked });
            switch (Option)
            {
                case -1://If all paths were blocked, place the end tile here and finish
                    maze[CurrentPosition].tileType = finalTile;
                    return;
                case 0: //Turning left
                    Direction.Rotate(-1);
                    break;
                case 1: //Moving ahead
                    break;
                case 2: //Turning right
                    Direction.Rotate(1);
                    break;
            }
            CurrentPosition = maze.DigPath(CurrentPosition, Direction, NextTile, proximity);
        }
    }

    /// <summary>
    /// Selects a random option from a set of provided weights
    /// </summary>
    /// <param name="weights"></param>
    /// <returns>
    /// The index of the selected option, -1 if all weights are 0
    /// </returns>
    static int GetWeightedRandomOption(int[] weights)
    {
        //Get the total weight
        int weightSum = 0;
        foreach (int weight in weights)
        {
            weightSum += weight;
        }

        //Get an option
        float randomNumber = UnityEngine.Random.Range(0, weightSum) + 1;
        for (int index = 0; index < weights.Length; index++)
        {
            if (randomNumber <= weights[index])
            {
                return index;
            }
            else
            {
                randomNumber -= weights[index];
            }
        }

        //Only happens if all weights are 0
        return -1;
    }

    /// <summary>
    /// Gets all tiles in a maze which can be expanded from
    /// </summary>
    /// <param name="maze"></param>
    /// <returns></returns>
    static List<Vector2> GetViableExpansionPoints(Maze maze)
    {
        List<Vector2> points = new List<Vector2>();
        for (int X = -maze.negativeWidth + 1; X < maze.positiveWidth; X++)
        {
            for (int Y = -maze.negativeHeight + 1; Y < maze.positiveHeight; Y++)
            {
                if (CheckTileExpandability(maze, new Vector2(X, Y)) && maze.mazeTiles[X][Y].tileType == TileTypes.Passage)
                {
                    points.Add(new Vector2(X, Y));
                }
            }
        }
        return points;
    }

    /// <summary>
    /// Gets all tiles in a maze which can be expanded from
    /// </summary>
    /// <param name="maze"></param>
    /// <returns></returns>
    static List<Vector2> GetViableExpansionPoints(Maze maze, int proximity)
    {
        List<Vector2> points = new List<Vector2>();
        for (int X = -maze.negativeWidth + 1; X < maze.positiveWidth; X++)
        {
            for (int Y = -maze.negativeHeight + 1; Y < maze.positiveHeight; Y++)
            {
                //Add the tile to the list if it clears all criteria (Has)
                if (CheckTileExpandability(maze, new Vector2(X, Y)) && maze.mazeTiles[X][Y].tileType == TileTypes.Passage && ((MazeTileGen)maze.mazeTiles[X][Y]).startPathProximity == proximity)
                {
                    points.Add(new Vector2(X, Y));
                }
            }
        }
        return points;
    }

    /// <summary>
    /// Checks if a tile can be expanded in any direction
    /// </summary>
    /// <param name="maze"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    static bool CheckTileExpandability(Maze maze, Vector2 position)
    {
        if (maze.AttemptDig(position, new Direction(0)) || maze.AttemptDig(position, new Direction(1)) || maze.AttemptDig(position, new Direction(2)) || maze.AttemptDig(position, new Direction(3)))
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Transforms all generation tiles in a maze into non-gen ones
    /// </summary>
    /// <param name="maze"></param>
    static void FinishMaze(Maze maze)
    {
        for (int i = 0; i < maze.width; i++)
        {
            for (int j = 0; j < maze.height; j++)
            {
                if (maze.mazeTiles.baseArray[i].baseArray[j] is MazeTileGen)
                {
                    maze.mazeTiles.baseArray[i].baseArray[j] = ((MazeTileGen)maze.mazeTiles.baseArray[i].baseArray[j]).GetNonGenTile();
                }
            }
        }
    }
}



/// <summary>
/// A maze consisting of tiles with paths in between
/// </summary>
public class Maze
{
    public ExtendedArray<ExtendedArray<MazeTile>> mazeTiles;
    public int width => mazeTiles.length;
    public int positiveWidth => mazeTiles.positiveLength;
    public int negativeWidth => mazeTiles.negativeLength;
    public int height => mazeTiles[0].length;
    public int positiveHeight => mazeTiles[0].positiveLength;
    public int negativeHeight => mazeTiles[0].negativeLength;

    public Maze()
    {
        //MazeTiles = new MazeTile[1][];
        mazeTiles = new ExtendedArray<ExtendedArray<MazeTile>>(1);
        //MazeTiles[0] = new MazeTile[1];
        mazeTiles[0] = new ExtendedArray<MazeTile>(1);
        mazeTiles[0][0] = new MazeTile();
    }
    public Maze(int Size)
    {
        mazeTiles = new ExtendedArray<ExtendedArray<MazeTile>>(Size);
        //MazeTiles = new MazeTile[Size][];
        for (int i = 0; i < Size; i++)
        {
            mazeTiles[i] = new ExtendedArray<MazeTile>(Size);
            //MazeTiles[i] = new MazeTile[Size];
        }
        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                mazeTiles[i][j] = new MazeTile();
            }
        }
    }
    public Maze(int x, int y)
    {
        mazeTiles = new ExtendedArray<ExtendedArray<MazeTile>>(x);
        //MazeTiles = new MazeTile[Size][];
        for (int i = 0; i < x; i++)
        {
            mazeTiles[i] = new ExtendedArray<MazeTile>(y);
            //MazeTiles[i] = new MazeTile[Size];
        }
        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                mazeTiles[i][j] = new MazeTile();
            }
        }
    }

    public List<MazeTile> GetAllMazeTiles()
    {
        List<MazeTile> allTiles = new List<MazeTile>();

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                allTiles.Add(mazeTiles.baseArray[i].baseArray[j]);
            }
        }
        return allTiles;
    }

    /// <summary>
    /// Makes sure all slots of the maze have a tile
    /// </summary>
    public void PopulateMaze()
    {
        int x = mazeTiles.length;
        int y = mazeTiles[0].length;

        //Make sure all arrays are present
        for (int i = 0; i < x; i++)
        {
            if (mazeTiles.baseArray[i] == null)
            {
                mazeTiles.baseArray[i] = new ExtendedArray<MazeTile>(y);
            }
        }
        //Make sure all tiles are present
        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                if (mazeTiles.baseArray[i].baseArray[j] == null)
                {
                    mazeTiles.baseArray[i].baseArray[j] = new MazeTileGen();
                }
            }
        }
    }

    /// <summary>
    /// Digs a path in the maze
    /// </summary>
    /// <param name="startPosition"></param>
    /// <param name="digDirection"></param>
    /// <returns>
    /// The tile the path ends at
    /// </returns>
    public Vector2 DigPath(Vector2 startPosition, Direction digDirection)
    {
        return DigPath(startPosition, digDirection, TileTypes.Passage);
    }
    /// <summary>
    /// Digs a path in the maze
    /// </summary>
    /// <param name="startPosition"></param>
    /// <param name="digDirection"></param>
    /// <param name="tileType"></param>
    /// <returns>
    /// The tile the path ends at
    /// </returns>
    public Vector2 DigPath(Vector2 startPosition, Direction digDirection, TileTypes tileType)
    {
        return DigPath(startPosition, digDirection, tileType, 0);
    }
    /// <summary>
    /// Digs a path in the maze
    /// </summary>
    /// <param name="startPosition"></param>
    /// <param name="digDirection"></param>
    /// <param name="proximity"></param>
    /// <returns>
    /// The tile the path ends at
    /// </returns>
    public Vector2 DigPath(Vector2 startPosition, Direction digDirection, int proximity)
    {
        return DigPath(startPosition, digDirection, TileTypes.Passage, proximity);
    }
    /// <summary>
    /// Digs a path in the maze
    /// </summary>
    /// <param name="startPosition"></param>
    /// <param name="digDirection"></param>
    /// <param name="tileType"></param>
    /// <param name="proximity"></param>
    /// <returns>
    /// The tile the path ends at
    /// </returns>
    public Vector2 DigPath(Vector2 startPosition, Direction digDirection, TileTypes tileType, int proximity)
    {
        Vector2 targetPosition = startPosition + digDirection.ToVector();
        //Expand the maze if nessecary
        if (targetPosition.x >= positiveWidth || targetPosition.x <= -negativeWidth || targetPosition.y >= positiveHeight || targetPosition.y <= -negativeHeight)
        {
            Extend(digDirection);
        }
        //Create the path
        mazeTiles[(int)startPosition.x][(int)startPosition.y].SetPath(digDirection, true);
        mazeTiles[(int)targetPosition.x][(int)targetPosition.y].SetPath(digDirection.Rotated(2), true);

        //Configure the target tile
        mazeTiles[(int) targetPosition.x][(int)targetPosition.y].tileType = tileType;
        ((MazeTileGen)mazeTiles[(int)targetPosition.x][(int)targetPosition.y]).startPathProximity = proximity;

        return targetPosition;
    }

    /// <summary>
    /// Checks if a potential path would hit anything
    /// </summary>
    /// <param name="startPosition"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    public bool AttemptDig(Vector2 startPosition, Direction direction)
    {
        Vector2 targetPosition = startPosition + direction.ToVector();
        if (targetPosition.x >= positiveWidth || targetPosition.x <= -negativeWidth || targetPosition.y >= positiveHeight || targetPosition.y <= -negativeHeight)
        {
            return true; //The dig attempts to expand beyond the border, and thus cannot hit anything
        }
        if (mazeTiles[(int) startPosition.x + (int) direction.ToVector().x][(int)startPosition.y + (int)direction.ToVector().y].tileType != TileTypes.Empty)
        {
            return false; //The dig will hit a non-empty tile
        }
        else
        {
            return true; //The dig will hit an empty tile
        }
    }

    /// <summary>
    /// Expands the maze array in the provided direction
    /// </summary>
    /// <param name="expansionDirection"></param>
    public void Extend(Direction expansionDirection)
    {
        switch (expansionDirection.direction)
        {
            case 0:
                for (int i = 0; i < mazeTiles.length; i++)
                {
                    mazeTiles.baseArray[i].Extend(1);
                }
                break;
            case 1:
                mazeTiles.Extend(1);
                mazeTiles.baseArray[^1] = new ExtendedArray<MazeTile>(mazeTiles[0].length);
                mazeTiles.baseArray[^1].zeroPosition = mazeTiles[0].zeroPosition;
                break;
            case 2:
                for (int i = 0; i < mazeTiles.length; i++)
                {
                    mazeTiles.baseArray[i].Extend(-1);
                }
                break;
            case 3:
                mazeTiles.Extend(-1);
                mazeTiles.baseArray[0] = new ExtendedArray<MazeTile>(mazeTiles[0].length);
                mazeTiles.baseArray[0].zeroPosition = mazeTiles[0].zeroPosition;
                break;
        }
        PopulateMaze(); //Fill the newly created space with things
    }

    public MazeTile this[Vector2 pos]
    {
        get
        {
            return mazeTiles[(int) pos.x][(int)pos.y];
        }
        set
        {
            mazeTiles[(int)pos.x][(int)pos.y] = value;
        }
    }
    public ExtendedArray<MazeTile> this[int index]
    {
        get
        {
            return mazeTiles[index];
        }
        set
        {
            mazeTiles[index] = value;
        }
    }
}

/// <summary>
/// A tile in a maze
/// </summary>
public class MazeTile
{
    public TileTypes tileType;
    protected bool[] pathsOpen;
    public bool GetPath(Direction dir)
    {
        return pathsOpen[dir.direction];
    }
    public void SetPath(Direction dir, bool open)
    {
        pathsOpen[dir.direction] = open;
    }
    public MazeTile()
    {
        tileType = TileTypes.Empty;
        pathsOpen = new bool[4];
    }
    public MazeTile(TileTypes type, bool[] pathsOpen)
    {
        tileType = type;
        this.pathsOpen = pathsOpen;
    }

    /// <summary>
    /// Renders the tile at the selected position
    /// </summary>
    /// <param name="pos"></param>
    public void Render(Vector2 pos)
    {
        Vector2 consolePos = GetConsolePosition(pos);
        Console.SetCursorPosition((int)consolePos.x, (int)consolePos.y);
        switch (tileType)
        {
            case TileTypes.Empty:
                Console.Write("X");
                break;
            case TileTypes.Passage:
                Console.Write("0");
                break;
            case TileTypes.Treasure:
                Console.Write("T");
                break;
            case TileTypes.Start:
                Console.Write("S");
                break;
            case TileTypes.End:
                Console.Write("E");
                break;
        }

        if (pathsOpen[0])
        {
            Console.SetCursorPosition((int)consolePos.x, (int)consolePos.y - 1);
            Console.Write("|");
        }
        else
        {
            Console.SetCursorPosition((int)consolePos.x, (int)consolePos.y - 1);
            Console.Write(" ");
        }
        if (pathsOpen[1])
        {
            Console.SetCursorPosition((int)consolePos.x + 1, (int)consolePos.y);
            Console.Write("-");
        }
        else
        {
            Console.SetCursorPosition((int)consolePos.x + 1, (int)consolePos.y);
            Console.Write(" ");
        }
        if (pathsOpen[2])
        {
            Console.SetCursorPosition((int)consolePos.x, (int)consolePos.y + 1);
            Console.Write("|");
        }
        else
        {
            Console.SetCursorPosition((int)consolePos.x, (int)consolePos.y + 1);
            Console.Write(" ");
        }
        if (pathsOpen[3])
        {
            Console.SetCursorPosition((int)consolePos.x - 1, (int)consolePos.y);
            Console.Write("-");
        }
        else
        {
            Console.SetCursorPosition((int) consolePos.x - 1, (int) consolePos.y);
            Console.Write(" ");
        }
    }

    /// <summary>
    /// Gets a postion in the console from a position
    /// </summary>
    /// <param name="truePos"></param>
    /// <returns></returns>
    static Vector2 GetConsolePosition(Vector2 truePos)
    {
        Vector2 consolePosition = new Vector2();
        consolePosition.x = ((truePos.x + 1) * 3) - 1;
        consolePosition.y = ((truePos.y + 1) * 3) - 1;
        return consolePosition;
    }
}

/// <summary>
/// A MazeTile with extra generation data
/// </summary>
class MazeTileGen : MazeTile
{
    public int startPathProximity = 0; //How far the path is from the main path. Used only during generation.

    public MazeTile GetNonGenTile()
    {
        return new MazeTile(tileType, pathsOpen);
    }
}

public enum TileTypes
{
    Empty,
    Passage,
    Start,
    Treasure,
    End,
}



/// <summary>
/// One of the 4 cardinal directions represented as 0, 1, 2, 3
/// </summary>
public struct Direction
{
    public int direction;


    public Direction(int dir)
    {
        this.direction = dir;
    }
    /// <summary>
    /// Rotates the direction in 90 degree increments clockwise
    /// </summary>
    /// <param name="turns"></param>
    public void Rotate(int turns)
    {
        direction += turns;

        while (direction < 0)
        {
            direction += 4;
        }
        while (direction > 3)
        {
            direction -= 4;
        }
    }
    /// <summary>
    /// Gets a verision of the direction that is rotated in 90 degree increments clockwise
    /// </summary>
    /// <param name="turns"></param>
    /// <returns></returns>
    public Direction Rotated(int turns)
    {
        int newDir = direction + turns;

        while (newDir < 0)
        {
            newDir += 4;
        }
        while (newDir > 3)
        {
            newDir -= 4;
        }

        return new Direction(newDir);
    }

    /// <summary>
    /// Turns the direction into a vector pointing in the direction
    /// </summary>
    /// <returns></returns>
    public Vector2 ToVector()
    {
        switch (direction)
        {
            case 0:
                return new Vector2(0, 1);
            case 1:
                return new Vector2(1, 0);
            case 2:
                return new Vector2(0, -1);
            case 3:
                return new Vector2(-1, 0);
            default:
                return new Vector2(0, 0);
        }
    }
}

/// <summary>
/// An array that can handle negative indexes
/// </summary>
/// <typeparam name="Type"></typeparam>
public class ExtendedArray<Type>
{
    public Type[] baseArray;
    public int zeroPosition = 0;

    public ExtendedArray(int length)
    {
        baseArray = new Type[length];
    }
    public ExtendedArray(int positiveLength, int negativeLength)
    {
        baseArray = new Type[positiveLength + negativeLength - 1];
        zeroPosition = negativeLength - 1;
    }

    public int length => baseArray.Length;
    public int positiveLength => baseArray.Length - zeroPosition;
    public int negativeLength => zeroPosition + 1;

    public Type this[int Index]
    {
        get
        {
            return baseArray[Index + zeroPosition];
        }
        set
        {
            baseArray[Index + zeroPosition] = value;
        }
    }

    /// <summary>
    /// Extends the array. Positive values extend the positive side, negative values extend the negative side.
    /// </summary>
    /// <param name="amount"></param>
    public void Extend(int amount)
    {
        if (amount > 0)
        {
            Array.Resize(ref baseArray, baseArray.Length + amount);
        }
        else if (amount < 0)
        {
            Array.Resize(ref baseArray, baseArray.Length + -amount);
            zeroPosition += -amount;
            for (int i = 0; i < -amount; i++)
            {
                for (int j = baseArray.Length - 2; j >= 0; j--)
                {
                    baseArray[j + 1] = baseArray[j];
                }
                baseArray[0] = default(Type);
            }
        }
    }
}
