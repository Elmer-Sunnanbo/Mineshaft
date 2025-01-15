using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RailTile : MonoBehaviour
{
    UnityEvent turnTurntable = new UnityEvent();

    public RailTile[] neighbours;
    public bool isStop;
    public bool isTurntable;
    [SerializeField] SpriteRenderer sprite;
    [SerializeField] Sprite straightSprite;
    [SerializeField] Sprite straightStopSprite;
    [SerializeField] Sprite curveSprite;
    [SerializeField] Sprite tSpriteError;
    [SerializeField] Sprite orphanSpriteError;
    [SerializeField] Sprite crossSprite;
    [SerializeField] Sprite turntableSprite;
    [SerializeField] Sprite turntableSpriteRotated;

    public void EnterTurntable(MinecartMovement minecart)
    {
        turnTurntable.AddListener(minecart.TurnTableRotation);
    }
    public void ExitTurntable(MinecartMovement minecart)
    {
        turnTurntable.RemoveListener(minecart.TurnTableRotation);
    }

    public Direction GetDirectionAfterTravel(Direction startDir)
    {
        if (isTurntable)
        {
            if (neighbours[startDir.direction] != null)
            {
                return startDir;
            }
            else
            {
                return startDir.Rotated(2);
            }
        }
        if (neighbours[0] != null && neighbours[1] != null && neighbours[2] != null && neighbours[3] != null) //If this is a cross rail
        {
            return startDir;
        }
        for (int dir = 0; dir < 4; dir++)
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
        if (progress < 0)
        {
            Debug.LogError("A rail tile was tasked with finding a position for a progress value below 0");
            return GetPosition(0, startDirection);
        }
        if (progress > 1)
        {
            Debug.LogError("A rail tile was tasked with finding a position for a progress value above 1");
            return GetPosition(1, startDirection);
        }
        if (isTurntable && neighbours[startDirection.direction] == null)
        {
            return GetPositionBounceStraight(progress, startDirection);
        }
        if (neighbours[startDirection.direction] != null || isTurntable)
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
        if (progress <= 0.5f)
        {
            return GetPositionStraight(progress, startDirection);
        }
        else
        {
            return GetPositionStraight(1 - progress, startDirection);
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
        for (int i = 0; i < turns; i++)
        {
            vector = new Vector2(vector.y, vector.x * -1);
        }
        return vector;
    }

    public void RotateTurntable()
    {
        turnTurntable.Invoke();
    }

    public void AutoSetSprite()
    {
        Sprite selectedSprite = null;
        int turns = 0;

        //Count number of neighbours
        int count = 0;
        if (neighbours[0] != null) { count++; }
        if (neighbours[1] != null) { count++; }
        if (neighbours[2] != null) { count++; }
        if (neighbours[3] != null) { count++; }

        if(isTurntable)
        {
            selectedSprite = turntableSprite;
        }
        else
        {
            switch (count)
            {
                case 0:
                    selectedSprite = orphanSpriteError;
                    break;
                case 1:
                    if(isStop)
                    {
                        selectedSprite = straightStopSprite;
                    }
                    else
                    {
                        selectedSprite = straightSprite;
                    }
                    
                    if (neighbours[0] == null && neighbours[2] == null) { turns = 1; } //Rotate 90 degrees if the rail should go left to right
                    break;
                case 2:
                    if (neighbours[0] != null)
                    {
                        if (neighbours[2] != null)
                        {
                            if (isStop)
                            {
                                selectedSprite = straightStopSprite;
                            }
                            else
                            {
                                selectedSprite = straightSprite;
                            }
                        }
                        if (neighbours[1] != null)
                        {
                            selectedSprite = curveSprite;
                            turns = 1;
                        }
                        if (neighbours[3] != null)
                        {
                            selectedSprite = curveSprite;
                            turns = 2;
                        }
                    }
                    else if (neighbours[1] != null)
                    {
                        if (neighbours[3] != null)
                        {
                            if (isStop)
                            {
                                selectedSprite = straightStopSprite;
                            }
                            else
                            {
                                selectedSprite = straightSprite;
                            }
                            turns = 1;
                        }
                        if (neighbours[2] != null)
                        {
                            selectedSprite = curveSprite;
                        }
                    }
                    else
                    {
                        selectedSprite = curveSprite;
                        turns = 3;
                    }
                    break;
                case 3:
                    selectedSprite = tSpriteError;
                    if (neighbours[0] == null)
                    {
                        turns = 0;
                    }
                    else if (neighbours[1] == null)
                    {
                        turns = 3;
                    }
                    else if (neighbours[2] == null)
                    {
                        turns = 2;
                    }
                    else if (neighbours[3] == null)
                    {
                        turns = 1;
                    }
                    else
                    {
                        Debug.LogError("A railtile's AutoSetSprite failed to find an empty connection on a rail with 3 connections (somehow)");
                    }
                    break;
                case 4:
                    selectedSprite = crossSprite;
                    break;
            }
        }
        if(selectedSprite == null)
        {
            Debug.LogError("A railtile's AutoSetSprite failed to find a sprite");
        }
        sprite.sprite = selectedSprite;
        sprite.transform.rotation = Quaternion.Euler(0, 0, turns * 90);
    }
}
