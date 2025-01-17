using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Rigidbody2D))]
public class StalagmiteEnemy : MonoBehaviour, IHittable, IEnemy
{
    [Header("Chase parameters")]
    [SerializeField] GameObject target;
    [SerializeField] GameObject stalagmiteProjectile;
    [SerializeField] float speed;
    [SerializeField] float wakeUpDistance;
    [SerializeField] float sleepDelay;
    [SerializeField] float projectileChargeTime;
    [SerializeField] float maxDistance;
    [SerializeField] float minDistance;

    float? timeUntilSleep = null;
    float? timeUntilProjectile = null;
    float lastKnownPosMinDistance = 0.2f;
    float attackReloadTimer;
    public int StalagmiteHealth;
    Rigidbody2D myRigidbody;
    SpriteRenderer mySpriteRenderer;
    Vector2 lastKnownPosition;
    bool lastKnownPositionActive;
    States state;
    [SerializeField] Animator MoleAni;

    enum States
    {
        Sleeping,
        Chasing,
        ChasingLKP,
        ChaseEnd,
        InRange,
        Charging,
    }
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        mySpriteRenderer = GetComponent<SpriteRenderer>();
        state = States.Sleeping;
        StalagmiteHealth = 4;
    }
    private void FixedUpdate()
    {
        if(StalagmiteHealth < 1)
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        Vector2 targetPos = target.transform.position;
        Vector2 vectorToTarget = targetPos - (Vector2)transform.position;
        bool hasLOS = LineOfSightCheck();
        int StateCheckPasses = 0;

    CheckStatesAgain:

        StateCheckPasses++;
        if (StateCheckPasses == 10)
        {
            Debug.LogWarning("Enemy ai passed through state checker 10 times, skipping");
            goto SkipStateChecking;
        }

        switch (state)
        {
            case States.Sleeping:
                myRigidbody.velocity = Vector2.zero;
                if (hasLOS && vectorToTarget.magnitude < wakeUpDistance)
                {
                    state = States.Chasing;
                    goto CheckStatesAgain;
                }
                break;

            case States.Chasing:
                if (vectorToTarget.magnitude > minDistance && vectorToTarget.magnitude < maxDistance && hasLOS) //If the target is in chase range but not attack range
                {
                    //Pursue target
                    Vector2 MoveDir = (targetPos - (Vector2)transform.position).normalized;
                    myRigidbody.velocity = MoveDir * speed;
                    lastKnownPosition = targetPos;
                    lastKnownPositionActive = true;
                }
                else if (vectorToTarget.magnitude < minDistance && hasLOS) //If we've reached the target
                {
                    //Stay in range and do whatever it is we do there (likely attack)
                    myRigidbody.velocity = Vector2.zero;
                    lastKnownPosition = targetPos;
                    lastKnownPositionActive = true;
                    state = States.InRange;
                }
                else if (lastKnownPositionActive == true) //If the target is out of range but there is an unchecked last known position
                {
                    //Go after it
                    state = States.ChasingLKP;
                    goto CheckStatesAgain;
                }
                else
                {
                    //End chase
                    state = States.ChaseEnd;
                    goto CheckStatesAgain;
                }
                break;

            case States.ChasingLKP:
                if (vectorToTarget.magnitude < maxDistance && hasLOS) //If the target is in chase range
                {
                    //Pursue target
                    state = States.Chasing;
                    lastKnownPosition = targetPos;
                    lastKnownPositionActive = true;
                    goto CheckStatesAgain;
                }
                else
                {
                    if (lastKnownPositionActive == false)
                    {
                        break;
                    }
                    else
                    {
                        if ((lastKnownPosition - (Vector2)transform.position).magnitude <= lastKnownPosMinDistance) //If we are at the last known position
                        {
                            //Forget it and end chase
                            state = States.ChaseEnd;
                            lastKnownPositionActive = false;
                            goto CheckStatesAgain;
                        }
                        else
                        {
                            //Go for it
                            Vector2 MoveDir = (lastKnownPosition - (Vector2)transform.position).normalized;
                            myRigidbody.velocity = MoveDir * speed;
                        }
                    }
                }
                break;

            case States.InRange:
                if (vectorToTarget.magnitude < minDistance && hasLOS) //If the target is still in range
                {
                    lastKnownPosition = targetPos;
                    lastKnownPositionActive = true;
                    state = States.Charging;
                    myRigidbody.velocity = Vector2.zero;
                    goto CheckStatesAgain;
                }
                else
                {
                    //Target has left the range, chase after it
                    state = States.Chasing;
                    goto CheckStatesAgain;
                }

            case States.ChaseEnd:
                if (vectorToTarget.magnitude < maxDistance && hasLOS) //If the target is in range
                {
                    //Chase it
                    state = States.Chasing;
                    timeUntilSleep = null;
                    goto CheckStatesAgain;
                }
                else
                {
                    //Chill and wait for time to go to sleep
                    myRigidbody.velocity = Vector2.zero;

                    if (timeUntilSleep == null) //If the timer hasn't been set
                    {
                        timeUntilSleep = sleepDelay;
                    }
                    
                    timeUntilSleep -= Time.deltaTime;

                    if (timeUntilSleep <= 0)
                    {
                        //Go to sleep
                        timeUntilSleep = null;
                        state = States.Sleeping;
                        goto CheckStatesAgain;
                    }
                }
                break;

            case States.Charging:
                //Chill and wait for attack to charge
                myRigidbody.velocity = Vector2.zero;
                if(vectorToTarget.magnitude < minDistance && hasLOS)
                {
                    lastKnownPosition = targetPos;
                    lastKnownPositionActive = true;
                }

                if (timeUntilProjectile == null) //If the timer hasn't been set
                {
                    timeUntilProjectile = projectileChargeTime;
                }

                timeUntilProjectile -= Time.deltaTime;

                if (timeUntilProjectile <= 0)
                {
                    //Fire it off
                    timeUntilProjectile = null;
                    state = States.Chasing;
                    SummonStalagmite();
                    goto CheckStatesAgain;
                }
                break;
        }
    SkipStateChecking:;
        // everything inbeetween these two messages are for the animations and nothing else
        if (myRigidbody.velocityX == 0 && myRigidbody.velocityY == 0)
        {
            MoleAni.SetBool("Mole+y", false);
            MoleAni.SetBool("Mole-y", false);
            MoleAni.SetBool("Mole+x", false);
            MoleAni.SetBool("Mole-x", false);
            MoleAni.SetBool("StopMoling", true);
        }
        else if (Mathf.Abs(myRigidbody.velocityX) > Mathf.Abs(myRigidbody.velocityY))
        {
            if (myRigidbody.velocityX > 0)
            {
                MoleAni.SetBool("Mole+y", false);
                MoleAni.SetBool("Mole-y", false);
                MoleAni.SetBool("Mole+x", true);
                MoleAni.SetBool("Mole-x", false);
                MoleAni.SetBool("StopMoling", false);
            }
            if (myRigidbody.velocityX < 0)
            {
                MoleAni.SetBool("Mole+y", false);
                MoleAni.SetBool("Mole-y", false);
                MoleAni.SetBool("Mole+x", false);
                MoleAni.SetBool("Mole-x", true);
                MoleAni.SetBool("StopMoling", false);
            }
        }
        else
        {
            if (0 < myRigidbody.velocityY)
            {
                if (myRigidbody.velocityY > 0)
                {
                    MoleAni.SetBool("Mole+y", false);
                    MoleAni.SetBool("Mole-y", true);
                    MoleAni.SetBool("Mole+x", false);
                    MoleAni.SetBool("Mole-x", false);
                    MoleAni.SetBool("StopMoling", false);
                }
            }
            else
            {
                if (myRigidbody.velocityY < 0)
                {
                    MoleAni.SetBool("Mole+y", true);
                    MoleAni.SetBool("Mole-y", false);
                    MoleAni.SetBool("Mole+x", false);
                    MoleAni.SetBool("Mole-x", false);
                    MoleAni.SetBool("StopMoling", false);
                }
            }
        }

        // this is the end of the code for animation
    }

    bool LineOfSightCheck()
    {
        RaycastHit2D[] rayCastHits = Physics2D.RaycastAll(transform.position, target.transform.position - transform.position);
        bool wallHit = false;
        foreach (RaycastHit2D hit in rayCastHits)
        {
            if (hit.collider.TryGetComponent<PathfindingBlocker>(out _))
            {
                wallHit = true; //There was an obstruction before the player
            }
            if (hit.collider.TryGetComponent<Player>(out _) && wallHit == false)
            {
                return true; //The player was before any obstructions
            }
            if (hit.collider.TryGetComponent<Player>(out _) && wallHit == true)
            {
                return false; //There were obstrucions before the player
            }
        }
        return false;
    }

    void SummonStalagmite()
    {
        Vector2 shotTarget;
        if (LineOfSightCheck())
        {
            shotTarget = target.transform.position;
        }
        else if (lastKnownPositionActive)
        {
            shotTarget = lastKnownPosition;
        }
        else
        {
            return;
        }
        Vector2 angleTarget = shotTarget - (Vector2) transform.position;
        float angle = Mathf.Atan2(angleTarget.y, angleTarget.x) * Mathf.Rad2Deg;
        GameObject latestSpawn = Instantiate(stalagmiteProjectile, transform.position, Quaternion.Euler(0, 0, angle));//Instantiating the stalagmite bullet in the direction of the player.
    }

    public void Hit()
    {
        StalagmiteHealth--;
    }
    public void SetTarget(GameObject target)
    {
        this.target = target;
    }
}