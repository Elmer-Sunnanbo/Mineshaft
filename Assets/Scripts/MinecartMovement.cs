using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinecartMovement : MonoBehaviour
{
    [SerializeField] RailTile startTile;
    [SerializeField] Directions startDir;
    [SerializeField] float speed;
    float currentTileProgress = 0;
    RailTile currentTile;
    Direction currentDirection;

    private void Start()
    {
        currentTile = startTile;
        currentDirection = new Direction((int) startDir);
    }

    void Update()
    {
        currentTileProgress += Time.deltaTime * speed;
        if(currentTileProgress > 1)
        {
            currentTileProgress -= 1;
            currentDirection = currentTile.GetDirectionAfterTravel(currentDirection);
            currentTile = currentTile.GetNextTile(currentDirection);
        }
        transform.position = currentTile.GetPosition(currentTileProgress, currentDirection);
    }
}
