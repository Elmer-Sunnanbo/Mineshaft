using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinecartInteraction : MonoBehaviour
{
    public bool isInTrigger;
    public bool playerIn;
    Player currentPlayerInTrigger;
    Camera mainCam;
    [SerializeField] MinecartMovement movementScript;
    [SerializeField] Transform rightDisembark;
    [SerializeField] Transform leftDisembark;

    void Start()
    {
        mainCam = Camera.main;
    }
    void Update()
    {
        if (isInTrigger && Input.GetKeyDown(KeyCode.E) && !playerIn) //If the player attempts to enter the minecart
        {
            playerIn = true;
            currentPlayerInTrigger.EnterMinecart();
            currentPlayerInTrigger.transform.parent = transform.parent;
            currentPlayerInTrigger.transform.localPosition = Vector2.zero;
        }
        else if(playerIn && Input.GetKeyDown(KeyCode.E))
        {
            Vector2 mouseVector = mainCam.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            Direction clickDirection;
            if (Mathf.Abs(mouseVector.x) > Mathf.Abs(mouseVector.y)) //If the x component (regardless of left/right) is bigger than the y component.
            {
                if (mouseVector.x > 0)
                {
                    //Right
                    clickDirection = new Direction(1).Rotated(-movementScript.displayDirection.direction);
                }
                else
                {
                    //Left
                    clickDirection = new Direction(3).Rotated(-movementScript.displayDirection.direction);
                }
            }
            else //If the y component (regardless of up/down) is bigger than the x component.
            {
                if (mouseVector.y > 0)
                {
                    //Up
                    clickDirection = new Direction(0).Rotated(-movementScript.displayDirection.direction);
                }
                else
                {
                    //Down
                    clickDirection = new Direction(2).Rotated(-movementScript.displayDirection.direction);
                }
            }

            switch (clickDirection.direction)
            {
                case 0:
                    movementScript.AddFuel(-1);
                    break;
                case 1:
                    playerIn = false;
                    currentPlayerInTrigger.ExitMinecart();
                    currentPlayerInTrigger.transform.parent = null;
                    currentPlayerInTrigger.transform.position = rightDisembark.transform.position;
                    break;
                case 2:
                    movementScript.AddFuel(1);
                    break;
                case 3:
                    playerIn = false;
                    currentPlayerInTrigger.ExitMinecart();
                    currentPlayerInTrigger.transform.parent = null;
                    currentPlayerInTrigger.transform.position = leftDisembark.transform.position;
                    break;
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Player playerScript) && currentPlayerInTrigger == null)
        {
            isInTrigger = true;
            currentPlayerInTrigger = playerScript;
        }
        else if(collision.gameObject.TryGetComponent<Player>(out _))
        {
            Debug.LogWarning("A player entered the minecart trigger when there was already one in there");
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Player playerScript))
        {
            isInTrigger = false;
            currentPlayerInTrigger = null;
        }
    }
}
