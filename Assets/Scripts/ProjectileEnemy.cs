using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ProjectileEnemy : MonoBehaviour, IHittable, IEnemy
{
    [Header("Chase parameters")]
    public GameObject target;
    [SerializeField] GameObject projectile;
    [SerializeField] float speed;
    [SerializeField] float wakeUpDistance;
    [SerializeField] float sleepDelay;
    [SerializeField] float projectileChargeTime;
    [SerializeField] float maxDistance;
    [SerializeField] float minDistance;

    [Header("Colors")]
    [SerializeField] Color sleepColor;
    [SerializeField] Color chaseColor;
    [SerializeField] Color chaseLKPColor;
    [SerializeField] Color chaseEndColor;
    [SerializeField] Color inRangeColor;

    float? timeUntilSleep = null;
    public float ProEnemyHealth;
    float lastKnownPosMinDistance = 0.2f;
    float? timeUntilProjectile;
    Rigidbody2D myRigidbody;
    SpriteRenderer mySpriteRenderer;
    Vector2 lastKnownPosition;
    bool lastKnownPositionActive;
    States state;

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
        ProEnemyHealth = 3;
    }
    private void FixedUpdate()
    {
        if(ProEnemyHealth < 1)
        {
            Destroy(gameObject);
        }
    }
    void Update()
    {
        RenderState();
        Vector2 targetPos = target.transform.position;
        Vector2 vectorToTarget = targetPos - (Vector2)transform.position;
        bool hasLOS = LineOfSightCheck();
        int StateCheckPasses = 0;

        if (timeUntilProjectile <= 0)
        {
            SummonProjectile();
            timeUntilProjectile = null;
        }
        else if (timeUntilProjectile != null)
        {
            timeUntilProjectile -= Time.deltaTime;
        }
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
                    if(timeUntilProjectile == null)
                    {
                        timeUntilProjectile = projectileChargeTime;
                    }
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
                    //Attack or something
                    lastKnownPosition = targetPos;
                    lastKnownPositionActive = true;
                    myRigidbody.velocity = Vector2.zero;
                }
                else
                {
                    //Target has left the range, chase after it
                    state = States.Chasing;
                    goto CheckStatesAgain;
                }
                if (timeUntilProjectile == null)
                {
                    timeUntilProjectile = projectileChargeTime;
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

    
    void SummonProjectile()
    {
        Vector2 shotTarget;
        if(LineOfSightCheck())
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
        GameObject latestSpawn = Instantiate(projectile, transform.position, Quaternion.Euler(0, 0, angle)); //Summons projectile in the direction to the player.
        //myRigidbody.velocity = angleTarget * -10;//Move backwards after shooting. (Was buggy)
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

    public void Hit()
    {
        ProEnemyHealth--;
    }
    public void SetTarget(GameObject target)
    {
        this.target = target;
    }
}