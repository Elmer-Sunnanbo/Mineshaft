using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MinecartMovement : MonoBehaviour
{
    [SerializeField] RailTile startTile;
    [SerializeField] Directions startDir;
    [SerializeField] float speed;
    [SerializeField] Animator animator;
    [SerializeField] AudioSource puffSource;
    [SerializeField] AudioSource rollSource;
    [SerializeField] AudioSource scrapeSource;
    [SerializeField] AudioSource shakeSource;
    [SerializeField] TutorialManager tutorialManager;
    bool hasBouncedOnCurrentTile;
    bool hasStoppedOnCurrentTile = true; //Prevents immediatley stopping after starting on stop tiles
    GameObject interacter;
    MinecartInteraction interacterScript;
    ParticleSystem smokeParticles;
    float currentTileProgress = 0;
    RailTile currentTile;
    public Direction displayDirection => currentTile.GetDisplayDirection(currentDirection, currentTileProgress);
    Direction currentDirection;
    bool moving;
    Animations currentAnimation;

    enum Animations
    {
        UpDownIdleMoving,
        LeftRightIdle,
        LeftMoving,
        RightMoving,
        LeftMovingSit,
        RightMovingSit,
        LeftIdleSit,
        RightIdleSit,
        UpIdleMovingSit,
        DownIdleMovingSit,
    }

    private void Start()
    {
        interacterScript = transform.GetComponentInChildren<MinecartInteraction>();
        interacter = interacterScript.gameObject;
        smokeParticles = GetComponent<ParticleSystem>();
        currentTile = startTile;
        currentTileProgress = 0.5f;
        currentDirection = new Direction((int)startDir);
        transform.position = currentTile.GetPosition(currentTileProgress, currentDirection);
        SetAnim(GetAnim());
        if(currentTile.isTurntable)
        {
            currentTile.EnterTurntable(this);
        }
    }

    public void AddFuel(int fuel)
    {
        //Make sure the tutorial isn't locking the minecart
        if(tutorialManager != null)
        {
            if(!tutorialManager.minecartMoveAllowed)
            {
                return;
            }
        }
        if(GameManager.instance != null)
        {
            if(GameManager.instance.coal - Mathf.Abs(fuel) < 0) //If we're out of coal
            {
                if (UIUpdating.instance)
                {
                    UIUpdating.instance.FlashCoal0(); //Show it on UI
                }
                return;
            }
            else
            {
                if (UIUpdating.instance)
                {
                    UIUpdating.instance.FlashCoalDown(); //Show coal reduction on UI
                }
                GameManager.instance.coal -= Mathf.Abs(fuel);
            }
        }
        if(fuel < 0) //If we're going backwards, turn around (and go forwards)
        {
            currentDirection.Rotate(2);
            currentTileProgress = 1 - currentTileProgress; //Invert progress on current tile
        }
        if(fuel != 0) //Start moving (unless supplied 0 fuel)
        {
            moving = true;
            smokeParticles.Play();
            if(tutorialManager != null)
            {
                tutorialManager.MinecartMove();
            }
        }
    }

    void Update()
    {
        interacter.transform.rotation = Quaternion.Euler(0, 0, displayDirection.Rotated(-1).direction * -90);

        Animations animation = GetAnim();
        if(animation != currentAnimation)
        {
            SetAnim(animation);
        }

        if(moving)
        {
            puffSource.volume = 1;
            rollSource.volume = 1;
            scrapeSource.volume = GetScrapeAmount();
        }
        else
        {
            puffSource.volume = 0;
            rollSource.volume = 0;
            scrapeSource.volume = 0;
        }

        if (moving)
        {
            currentTileProgress += Time.deltaTime * speed;

           
            if (currentTileProgress > 1)
            {
                currentTileProgress -= 1;
                hasBouncedOnCurrentTile = false;
                hasStoppedOnCurrentTile = false;
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
            }
            else if (currentTileProgress >= 0.5f && !hasBouncedOnCurrentTile && currentTile.GetDirectionAfterTravel(currentDirection).direction == currentDirection.Rotated(2).direction) //If we just bounced
            {
                shakeSource.Play();
            }
            if (currentTileProgress >= 0.5f && currentTile.isStop && moving && !hasStoppedOnCurrentTile) //If we've reached the stop point on a stop tile
            {
                //Stop
                currentTileProgress = 0.5f;
                moving = false;
                hasStoppedOnCurrentTile = true;
                smokeParticles.Stop();
                shakeSource.Play();
            }
            transform.position = currentTile.GetPosition(currentTileProgress, currentDirection);
        }
    }

    public void TurnTableRotation()
    {
        currentDirection.Rotate(1);
        transform.position = currentTile.GetPosition(currentTileProgress, currentDirection);
    }

    Animations GetAnim()
    {
        if(!interacterScript.playerIn)
        {
            if(moving)
            {
                switch (displayDirection.direction)
                {
                    case 0:
                    case 2:
                        return Animations.UpDownIdleMoving;
                    case 1:
                        return Animations.RightMoving;
                    case 3:
                        return Animations.LeftMoving;
                }
            }
            else
            {
                switch(displayDirection.direction)
                {
                    case 0:
                    case 2:
                        return Animations.UpDownIdleMoving;
                    case 1:
                    case 3:
                        return Animations.LeftRightIdle;
                }
            }
        }
        else
        {
            if (moving)
            {
                switch (displayDirection.direction)
                {
                    case 0:
                        return Animations.UpIdleMovingSit;
                    case 2:
                        return Animations.DownIdleMovingSit;
                    case 1:
                        return Animations.RightMovingSit;
                    case 3:
                        return Animations.LeftMovingSit;
                }
            }
            else
            {
                switch (displayDirection.direction)
                {
                    case 0:
                        return Animations.UpIdleMovingSit;
                    case 2:
                        return Animations.DownIdleMovingSit;
                    case 1:
                        return Animations.RightIdleSit;
                    case 3:
                        return Animations.LeftIdleSit;
                }
            }
        }
        Debug.LogError("Minecart animation failed to find a suitable state");
        return Animations.UpDownIdleMoving;
    }

    void SetAnim(Animations anim)
    {
        currentAnimation = anim;
        switch (anim)
        {
            case Animations.UpDownIdleMoving:
                animator.SetTrigger("UpDownIdleMoving");
                break;
            case Animations.LeftRightIdle:
                animator.SetTrigger("LeftRightIdle");
                break;
            case Animations.LeftMoving:
                animator.SetTrigger("LeftMoving");
                break;
            case Animations.RightMoving:
                animator.SetTrigger("RightMoving");
                break;
            case Animations.UpIdleMovingSit:
                animator.SetTrigger("UpMovingIdleSit");
                break;
            case Animations.DownIdleMovingSit:
                animator.SetTrigger("DownMovingIdleSit");
                break;
            case Animations.LeftIdleSit:
                animator.SetTrigger("LeftIdleSit");
                break;
            case Animations.LeftMovingSit:
                animator.SetTrigger("LeftMovingSit");
                break;
            case Animations.RightIdleSit:
                animator.SetTrigger("RightIdleSit");
                break;
            case Animations.RightMovingSit:
                animator.SetTrigger("RightMovingSit");
                break;
        }
    }

    float GetScrapeAmount()
    {
        if(currentTile.GetIsCurved())
        {
            if(currentTileProgress > 0.5f)
            {
                return (1 - currentTileProgress) * 2;
            }
            else
            {
                return currentTileProgress * 2;
            }
        }
        //Don't scrape if the tile isn't curved
        return 0;
    }
}
