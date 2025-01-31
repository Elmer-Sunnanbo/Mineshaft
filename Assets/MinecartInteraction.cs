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
    [SerializeField] Transform frontDisembark;
    [SerializeField] Transform backDisembark;

    void Start()
    {
        mainCam = Camera.main;
    }
    void Update()
    {
        if (isInTrigger && Input.GetKeyDown(KeyCode.LeftShift) && !playerIn) //If the player attempts to enter the minecart
        {
            playerIn = true;
            currentPlayerInTrigger.EnterMinecart();
            currentPlayerInTrigger.transform.parent = transform.parent;
            currentPlayerInTrigger.transform.localPosition = Vector2.zero;
        }
        else if(playerIn && Input.GetKeyDown(KeyCode.LeftShift))
        {
            Vector2 mouseVector = mainCam.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            Direction clickDirection;
            if(Input.GetKey(KeyCode.W))
            {
                //Up
                clickDirection = new Direction(0).Rotated(-movementScript.displayDirection.direction);
            }
            else if (Input.GetKey(KeyCode.S))
            {
                //Down
                clickDirection = new Direction(2).Rotated(-movementScript.displayDirection.direction);
            }
            else if (Input.GetKey(KeyCode.A))
            {
                //Left
                clickDirection = new Direction(3).Rotated(-movementScript.displayDirection.direction);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                //Right
                clickDirection = new Direction(1).Rotated(-movementScript.displayDirection.direction);
            }
            else if (Mathf.Abs(mouseVector.x) > Mathf.Abs(mouseVector.y)) //If the x component (regardless of left/right) is bigger than the y component.
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
                    playerIn = false;
                    currentPlayerInTrigger.ExitMinecart();
                    currentPlayerInTrigger.transform.parent = null;
                    currentPlayerInTrigger.transform.position = frontDisembark.transform.position;
                    break;
                case 1:
                    playerIn = false;
                    currentPlayerInTrigger.ExitMinecart();
                    currentPlayerInTrigger.transform.parent = null;
                    currentPlayerInTrigger.transform.position = rightDisembark.transform.position;
                    break;
                case 2:
                    playerIn = false;
                    currentPlayerInTrigger.ExitMinecart();
                    currentPlayerInTrigger.transform.parent = null;
                    currentPlayerInTrigger.transform.position = backDisembark.transform.position;
                    break;
                case 3:
                    playerIn = false;
                    currentPlayerInTrigger.ExitMinecart();
                    currentPlayerInTrigger.transform.parent = null;
                    currentPlayerInTrigger.transform.position = leftDisembark.transform.position;
                    break;
            }
        }
        if (playerIn && Input.GetKeyDown(KeyCode.W))
        {
            Direction clickDirection;
            //Up
            clickDirection = new Direction(0).Rotated(-movementScript.displayDirection.direction);
            switch(clickDirection.direction)
            {
                case 0:
                    movementScript.AddFuel(1);
                    break;
                case 1:
                    //Can't go that way
                    if(ScreenShake.Instance)
                    {
                        ScreenShake.Instance.ShakeCam(0.05f, 0.3f);
                    }
                    break;
                case 2:
                    movementScript.AddFuel(-1);
                    break;
                case 3:
                    //Can't go that way
                    if (ScreenShake.Instance)
                    {
                        ScreenShake.Instance.ShakeCam(0.05f, 0.3f);
                    }
                    break;
            }
        }
        if (playerIn && Input.GetKeyDown(KeyCode.S))
        {
            //Down
            switch (new Direction(2).Rotated(-movementScript.displayDirection.direction).direction)
            {
                case 0:
                    movementScript.AddFuel(1);
                    break;
                case 1:
                    //Can't go that way
                    if (ScreenShake.Instance)
                    {
                        ScreenShake.Instance.ShakeCam(0.05f, 0.3f);
                    }
                    break;
                case 2:
                    movementScript.AddFuel(-1);
                    break;
                case 3:
                    //Can't go that way
                    if (ScreenShake.Instance)
                    {
                        ScreenShake.Instance.ShakeCam(0.05f, 0.3f);
                    }
                    break;
            }
        }
        if (playerIn && Input.GetKeyDown(KeyCode.A))
        {
            //Left
            switch (new Direction(3).Rotated(-movementScript.displayDirection.direction).direction)
            {
                case 0:
                    movementScript.AddFuel(1);
                    break;
                case 1:
                    //Can't go that way
                    if (ScreenShake.Instance)
                    {
                        ScreenShake.Instance.ShakeCam(0.05f, 0.3f);
                    }
                    break;
                case 2:
                    movementScript.AddFuel(-1);
                    break;
                case 3:
                    //Can't go that way
                    if (ScreenShake.Instance)
                    {
                        ScreenShake.Instance.ShakeCam(0.05f, 0.3f);
                    }
                    break;
            }
        }
        if (playerIn && Input.GetKeyDown(KeyCode.D))
        {
            //Right
            switch (new Direction(1).Rotated(-movementScript.displayDirection.direction).direction)
            {
                case 0:
                    movementScript.AddFuel(1);
                    break;
                case 1:
                    //Can't go that way
                    if (ScreenShake.Instance)
                    {
                        ScreenShake.Instance.ShakeCam(0.05f, 0.3f);
                    }
                    break;
                case 2:
                    movementScript.AddFuel(-1);
                    break;
                case 3:
                    //Can't go that way
                    if (ScreenShake.Instance)
                    {
                        ScreenShake.Instance.ShakeCam(0.05f, 0.3f);
                    }
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
