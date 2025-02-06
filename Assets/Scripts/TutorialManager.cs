using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    bool tutorialActive;
    bool tutorialFinished;
    [SerializeField] TutorialWall wall;
    [SerializeField] TutorialText startText;
    [SerializeField] TutorialText walkText;
    [SerializeField] TutorialText attackText;
    [SerializeField] TutorialText attackTextGold;
    [SerializeField] TutorialText attackTextCoal;
    [SerializeField] TutorialText attackTextCrystal;
    [SerializeField] TutorialText attackTextLever;
    [SerializeField] TutorialText enterMinecartText;
    [SerializeField] TutorialText moveMinecartText;
    public bool minecartMoveAllowed = true;
    bool walked;
    bool attacked;
    bool leverHit;
    bool coalHit;
    bool crystalHit;
    bool goldHit;
    bool minecartEntered;
    void Start()
    {
        startText.Spawn();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) //Start the tutorial on tab press unless it has already been finished
        {
            if(!tutorialFinished && !tutorialActive)
            {
                tutorialActive = true;
                wall.Spawn();
                startText.Despawn();
                walkText.Spawn();
                minecartMoveAllowed = false;
            }
        }

        if((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D)) && !walked && tutorialActive)
        {
            walked = true;
            walkText.Despawn();
            attackText.Spawn();
        }

        if(Input.GetMouseButtonDown(0) && walked && !attacked && tutorialActive)
        {
            attacked = true;
            attackText.Despawn();
            if(!goldHit)
            {
                attackTextGold.Spawn();
            }
            if(!coalHit)
            {
                attackTextCoal.Spawn();
            }
            if (!crystalHit)
            {
                attackTextCrystal.Spawn();
            }
            attackTextLever.Spawn();
        }
    }

    /// <summary>
    /// Mark the tutorial as done unless it is already started
    /// </summary>
    public void SkipTutorial()
    {
        if(!tutorialActive && !tutorialFinished)
        {
            tutorialFinished = true;
            startText.Despawn();
        }
    }

    /// <summary>
    /// Informs the tutorial manager that an object has been hit 1: Coal 2: Crystal 3: Gold 4: Lever 
    /// </summary>
    /// <param name="Id"></param>
    public void HitObject(int id)
    {
        switch(id)
        {
            case 1:
                if(!coalHit)
                {
                    coalHit = true;
                    attackTextCoal.Despawn();
                }
                break;
            case 2:
                if (!crystalHit)
                {
                    crystalHit = true;
                    attackTextCrystal.Despawn();
                }
                break;
            case 3:
                if (!goldHit)
                {
                    goldHit = true;
                    attackTextGold.Despawn();
                }
                break;
            case 4:
                if (!leverHit && tutorialActive)
                {
                    leverHit = true;
                    attackTextLever.Despawn();
                }
                break;
            default:
                Debug.LogError("The tutorial manager had an object hit with an unexpected id: " + id);
                break;
        }
    }
    public void MinecartEntered()
    {
        if(tutorialActive && !minecartEntered && coalHit && crystalHit && leverHit && goldHit)
        {
            minecartEntered = true;
            enterMinecartText.Despawn();
            moveMinecartText.Spawn();
            minecartMoveAllowed = true;
        }
    }
    public void MinecartMove()
    {
        if (tutorialActive && minecartEntered)
        {
            tutorialFinished = true;
            tutorialActive = false;
            moveMinecartText.Despawn();
            wall.Despawn();
        }
    }
}
