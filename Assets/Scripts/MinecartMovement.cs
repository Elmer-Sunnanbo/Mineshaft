using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MinecartMovement : MonoBehaviour
{
    [SerializeField] RailTile startTile;
    [SerializeField] Directions startDir;
    [SerializeField] float speed;
    GameObject interacter;
    ParticleSystem smokeParticles;
    float currentTileProgress = 0;
    int currentFuel;
    RailTile currentTile;
    Direction currentDirection;
    bool moving;

    private void Start()
    {
        interacter = transform.GetComponentInChildren<MinecartInteraction>().gameObject;
        smokeParticles = GetComponent<ParticleSystem>();
        currentTile = startTile;
        currentTileProgress = 0.5f;
        currentDirection = new Direction((int)startDir);
        transform.position = currentTile.GetPosition(currentTileProgress, currentDirection);
        
    }

    public void AddFuel(int fuel)
    {
        if(fuel < 0) //If we're going backwards, turn around (and go forwards)
        {
            currentDirection.Rotate(2);
        }
        currentFuel += Mathf.Abs(fuel);
        if(fuel != 0)
        {
            moving = true;
            smokeParticles.Play();
        }
    }

    void Update()
    {
        interacter.transform.rotation = Quaternion.Euler(0, 0, currentDirection.Rotated(-1).direction * -90);

        if (moving)
        {
            currentTileProgress += Time.deltaTime * speed;
            if (currentTileProgress > 1)
            {
                currentTileProgress -= 1;
                currentDirection = currentTile.GetDirectionAfterTravel(currentDirection);
                if (currentTile.isTurntable)
                {
                    currentTile.ExitTurntable(this);
                }
                currentTile = currentTile.GetNextTile(currentDirection);
                if (currentTile.isTurntable)
                {
                    currentTile.EnterTurntable(this);
                }
                if (currentTile.isStop)
                {
                    currentFuel--;
                }
            }
            if (currentTileProgress >= 0.5f && currentTile.isStop && currentFuel <= 0)
            {
                currentTileProgress = 0.5f;
                moving = false;
                smokeParticles.Stop();
            }
            transform.position = currentTile.GetPosition(currentTileProgress, currentDirection);
        }
    }

    public void TurnTableRotation()
    {
        currentDirection.Rotate(1);
        transform.position = currentTile.GetPosition(currentTileProgress, currentDirection);
    }
}
