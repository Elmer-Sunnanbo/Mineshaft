using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailTile : MonoBehaviour
{
    [SerializeField] RailTile[] neighbours;
    public bool isStop;
    public bool isDeadEnd;
    public Direction GetDirectionAfterTravel(Direction startDir)
    {
        for(int dir = 0; dir < 4; dir++)
        {
            if (neighbours[dir] == null || dir == startDir.Rotated(2).direction)
            {
                continue;
            }
            return new Direction(dir);
        }
        //Must be a dead end, turn around
        return startDir.Rotated(2);
    }

    public RailTile GetNextTile(Direction direction)
    {
        return neighbours[direction.direction];
    }

    public Vector2 GetPosition(float progress, Direction startDirection)
    {
        if(progress < 0)
        {
            Debug.LogError("A rail tile was tasked with finding a position for a progress value below 0");
            return GetPosition(0, startDirection);
        }
        if (progress > 1)
        {
            Debug.LogError("A rail tile was tasked with finding a position for a progress value above 1");
            return GetPosition(1, startDirection);
        }
        if (neighbours[startDirection.direction] != null)
        {
            return GetPositionStraight(progress, startDirection);
        }
        else if (neighbours[startDirection.Rotated(1).direction] != null)
        {
            return GetPositionRightTurn(progress, startDirection);
        }
        else if (neighbours[startDirection.Rotated(-1).direction] != null)
        {
            return GetPositionLeftTurn(progress, startDirection);
        }
        else //This rail tile must be a dead end
        {
            return GetPositionBounceStraight(progress, startDirection);
        }
    }

    Vector2 GetPositionStraight(float progress, Direction startDirection)
    {
        switch (startDirection.direction)
        {
            case 0:
                return (Vector2)transform.position + new Vector2(0, progress - 0.5f);
            case 1:
                return (Vector2)transform.position + new Vector2(progress - 0.5f, 0);
            case 2:
                return (Vector2)transform.position + new Vector2(0, -progress + 0.5f);
            case 3:
                return (Vector2)transform.position + new Vector2(-progress + 0.5f, 0);
            default:
                Debug.LogError("A rail tile was tasked with finding a position with an invalid direction");
                return transform.position;
        }
    }
    Vector2 GetPositionBounceStraight(float progress, Direction startDirection)
    {
        if(progress <= 0.5f)
        {
            return GetPositionStraight(progress, startDirection);
        }
        else
        {
            return GetPositionStraight(1- progress, startDirection);
        }
    }

    Vector2 GetPositionRightTurn(float progress, Direction startDirection)
    {
        Vector2 position = GetCurvePosition(progress);
        //Move generated curve to match rail
        position /= 2;
        position.x *= -1;
        position.x += 0.5f;
        position.y -= 0.5f;
        

        position = TurnVector(position, startDirection.direction);

        return position + (Vector2)transform.position;
    }
    Vector2 GetPositionLeftTurn(float progress, Direction startDirection)
    {
        Vector2 position = GetCurvePosition(progress);
        //Move generated curve to match rail
        position /= 2;
        position.x -= 0.5f;
        position.y -= 0.5f;

        position = TurnVector(position, startDirection.direction);

        return position + (Vector2)transform.position;
    }

    Vector2 GetCurvePosition(float progress)
    {
        float angle = progress * 90;
        float x = Mathf.Cos(angle * Mathf.Deg2Rad);
        float y = Mathf.Sin(angle * Mathf.Deg2Rad);
        return new Vector2(x, y);
    }

    Vector2 TurnVector(Vector2 vector, int turns)
    {
        for(int i = 0; i < turns; i++)
        {
            vector = new Vector2(vector.y, vector.x * -1);
        }
        return vector;
    }
}
