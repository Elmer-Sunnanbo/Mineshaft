using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinecartMovement : MonoBehaviour
{
    [SerializeField] RailTile startTile;
    [SerializeField] Directions startDir;
    [SerializeField] float speed;
    float currentTileProgress = 0;
    int currentFuel;
    RailTile currentTile;
    Direction currentDirection;
    bool moving;

    private void Start()
    {
        currentTile = startTile;
        currentTileProgress = 0.5f;
        transform.position = currentTile.GetPosition(currentTileProgress, currentDirection);
        currentDirection = new Direction((int) startDir);
    }

    void AddFuel(int fuel)
    {
        if(fuel < 0) //If we're going backwards, turn around (and go forwards)
        {
            currentDirection.Rotate(2);
        }
        currentFuel += Mathf.Abs(fuel);
        if(fuel != 0)
        {
            moving = true;
        }
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.M))
        {
            AddFuel(1);
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            AddFuel(-1);
        }

        if (moving)
        {
            currentTileProgress += Time.deltaTime * speed;
            if (currentTileProgress > 1)
            {
                currentTileProgress -= 1;
                currentDirection = currentTile.GetDirectionAfterTravel(currentDirection);
                currentTile = currentTile.GetNextTile(currentDirection);
                if (currentTile.isStop)
                {
                    currentFuel--;
                }
            }
            if(currentTileProgress > 0.5f && currentTile.isStop && currentFuel <= 0)
            {
                currentTileProgress = 0.5f;
                moving = false;
            }
            transform.position = currentTile.GetPosition(currentTileProgress, currentDirection);
        }
    }
}
