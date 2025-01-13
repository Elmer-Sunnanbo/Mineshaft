using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]

public class EnamyDash : MonoBehaviour
{
    [Header("Chase parameters")]
    [SerializeField] GameObject target;
    [SerializeField] float speed;
    [SerializeField] float wakeUpDistance;
    [SerializeField] float sleepDelay;
    [SerializeField] float maxDistance;
    [SerializeField] float minDistance;

    [Header("Colors")]
    [SerializeField] Color sleepColor;
    [SerializeField] Color chaseColor;
    [SerializeField] Color chaseLKPColor;
    [SerializeField] Color chaseEndColor;
    [SerializeField] Color inRangeColor;

    float? timeUntilSleep = null;
    float lastKnownPosMinDistance = 0.2f;
    Rigidbody2D myRigidbody;
    SpriteRenderer mySpriteRenderer;
    Vector2 lastKnownPosition;
    bool lastKnownPositionActive;
    States state;

    public float enemyHealth;
    public float dashDuration = 0.5f; // Duration of the dash
    private bool isDashing = false;

    enum States
    {
        Sleeping,
        Chasing,
        ChasingLKP,
        ChaseEnd,
        InRange
    }
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        mySpriteRenderer = GetComponent<SpriteRenderer>();
        state = States.Sleeping;
    }

    void Update()
    {
        RenderState();
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
                if (vectorToTarget.magnitude < minDistance && hasLOS && !isDashing) //If the target is still in range
                {
                    //Attack or something
                    //myRigidbody.velocity = Vector2.zero; /// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                    StartCoroutine(DashTowardsPlayer(lastKnownPosition));
                }
                else
                {
                    //Target has left the range, chase after it
                    state = States.Chasing;
                    goto CheckStatesAgain;
                }
                break;

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
        }
    SkipStateChecking:;
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
        Debug.LogWarning("An enemy's line of sight check did not find the player object");
        return false;
    }

    /// <summary>
    /// Changes the color of the object to represent it's state
    /// </summary>
    void RenderState()
    {
        switch (state)
        {
            case States.Sleeping:
                mySpriteRenderer.color = sleepColor;
                break;
            case States.Chasing:
                mySpriteRenderer.color = chaseColor;
                break;
            case States.ChasingLKP:
                mySpriteRenderer.color = chaseLKPColor;
                break;
            case States.ChaseEnd:
                mySpriteRenderer.color = chaseEndColor;
                break;
            case States.InRange:
                mySpriteRenderer.color = inRangeColor;
                break;
        }
    }

    private IEnumerator DashTowardsPlayer(Vector2 playerPosition)
    {
        isDashing = true;
        Vector2 startPosition = transform.position;
        float elapsedTime = 0;

        while (elapsedTime < dashDuration)
        {
            transform.position = Vector2.Lerp(startPosition, lastKnownPosition, (elapsedTime / dashDuration));
            elapsedTime += Time.deltaTime; // Increment elapsed time
            yield return null; // Wait for the next frame
        }

        // Ensure the final position is exactly the player's position
        transform.position = lastKnownPosition;

        // Start the countdown coroutine
        yield return StartCoroutine(DashCountDown());
    }

    private IEnumerator DashCountDown()
    {
        int duration = 2; // Wait time
        yield return new WaitForSeconds(duration); // Wait for durantion-amount of seconds 
        isDashing = false; // Set isDashing to false thereby enabling enemy to be able to dash again
    }
}