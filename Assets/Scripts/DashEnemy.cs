using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class DashEnemy : MonoBehaviour, IHittable, IEnemy
{
    [SerializeField] AudioSource dashSound;
    [Header("Chase parameters")]
    [SerializeField] GameObject target;
    [SerializeField] float speed;
    [SerializeField] float dashSpeed;
    [SerializeField] float wakeUpDistance;
    [SerializeField] float sleepDelay;
    [SerializeField] float dashChargeTime;
    [SerializeField] float dashDuration;
    [SerializeField] float maxDistance;
    [SerializeField] float minDistance;


    float? timeUntilSleep = null;
    float? timeUntilDash = null;
    float? timeUntilDashEnds = null;
    float lastKnownPosMinDistance = 0.2f;
    public int Health;
    Rigidbody2D myRigidbody;
    Vector2 lastKnownPosition;
    Vector2 dashDirection;
    bool lastKnownPositionActive;
    States state;
    [SerializeField] Animator RATANI;

    enum States
    {
        Sleeping,
        Chasing,
        ChasingLKP,
        ChaseEnd,
        InRange,
        Charging,
        Dashing
    }
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        state = States.Sleeping;
        Health = 4;
    }
    private void FixedUpdate()
    {
        if (Health < 1)
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

                if (timeUntilDash == null) //If the timer hasn't been set
                {
                    timeUntilDash = dashChargeTime;
                }

                timeUntilDash -= Time.deltaTime;

                if (timeUntilDash <= 0)
                {
                    //Fire it off
                    timeUntilDash = null;
                    if (LineOfSightCheck())
                    {
                        state = States.Dashing;
                        dashDirection = (target.transform.position - transform.position).normalized;
                    }
                    else if (lastKnownPositionActive)
                    {
                        state = States.Dashing;
                        dashDirection = (lastKnownPosition - (Vector2) transform.position).normalized;
                    }
                    else
                    {
                        state = States.ChaseEnd;
                    }
                    goto CheckStatesAgain;
                }
                break;

            case States.Dashing:
                //Dash toward selected position
                myRigidbody.velocity = dashDirection * dashSpeed;

                if (timeUntilDashEnds == null) //If the timer hasn't been set
                {
                    timeUntilDashEnds = dashDuration;
                    dashSound.Play();
                }

                timeUntilDashEnds -= Time.deltaTime;

                if (timeUntilDashEnds <= 0)
                {
                    //End it
                    timeUntilDashEnds = null;
                    state = States.Chasing;
                    goto CheckStatesAgain;
                }
                break;
        }
    SkipStateChecking:;
        // everything inbeetween these two messages are for the animations and nothing else
        //in the anitmation tree rat up and rat down are swapped i do not know why i did it, this includes the idle antimations
        if (myRigidbody.velocityX == 0 && myRigidbody.velocityY == 0)
        {
            RATANI.SetBool("RATUP", false);
            RATANI.SetBool("RATDOWN", false);
            RATANI.SetBool("RATRIGHT", false);
            RATANI.SetBool("RATLEFT", false);
            RATANI.SetBool("RATIDLE", true);
        }
        else if (Mathf.Abs(myRigidbody.velocityX) > Mathf.Abs(myRigidbody.velocityY))
        {
            if (myRigidbody.velocityX > 0)
            {
                RATANI.SetBool("RATUP", false);
                RATANI.SetBool("RATDOWN", false);
                RATANI.SetBool("RATRIGHT", true);
                RATANI.SetBool("RATLEFT", false);
                RATANI.SetBool("RATIDLE", false);
            }
            if (myRigidbody.velocityX < 0)
            {
                RATANI.SetBool("RATUP", false);
                RATANI.SetBool("RATDOWN", false);
                RATANI.SetBool("RATRIGHT", false);
                RATANI.SetBool("RATLEFT", true);
                RATANI.SetBool("RATIDLE", false);
            }
        }
        else
        {
            if (0 < myRigidbody.velocityY)
            {
                if (myRigidbody.velocityY > 0)
                {
                    RATANI.SetBool("RATUP", false);
                    RATANI.SetBool("RATDOWN", true);
                    RATANI.SetBool("RATRIGHT", false);
                    RATANI.SetBool("RATLEFT", false);
                    RATANI.SetBool("RATIDLE", false);
                }
            }
            else
            {
                if (myRigidbody.velocityY < 0)
                {
                    RATANI.SetBool("RATUP", true);
                    RATANI.SetBool("RATDOWN", false);
                    RATANI.SetBool("RATRIGHT", false);
                    RATANI.SetBool("RATLEFT", false);
                    RATANI.SetBool("RATIDLE", false);
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


    public void Hit()
    {
        if (ScreenShake.Instance)
        {
            ScreenShake.Instance.ShakeCam(0.15f, 0.4f);
        }
        Health--;
    }
    public void SetTarget(GameObject target)
    {
        this.target = target;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (state == States.Dashing)
        {
            if (collision.gameObject.layer == 9) //If we've hit a wall
            {
                if (state == States.Dashing)
                {
                    timeUntilDashEnds = 0;
                }
            }
            if (collision.gameObject.TryGetComponent<Player>(out Player hitPlayer))
            {
                hitPlayer.Hit();
            }
        }
    }
}
