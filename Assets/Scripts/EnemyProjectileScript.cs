using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectileScript : MonoBehaviour
{
    private float speed = 7;
    private float projectileLongetivityTimer;
    private float prefireTimer;//Instantiate and colliders does not run the first couple of milliseconds.
    [SerializeField] int MaxBounceCount;
    int TimesBounced;
    Rigidbody2D myRigidBody;
    CircleCollider2D myCollider;


    void Start()
    {
        projectileLongetivityTimer = 20;
        prefireTimer = 0.1f;
        myRigidBody = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<CircleCollider2D>();
        myCollider.enabled = false;
        myRigidBody.velocity = transform.right * speed;
    }


    void Update()
    {
        if (prefireTimer < 0)
        {
            myCollider.enabled = true;
        }
        prefireTimer -= Time.deltaTime;
    }
    void FixedUpdate()
    {
        if (projectileLongetivityTimer < 0)
        {
            Destroy(gameObject);
        }
        projectileLongetivityTimer -= Time.deltaTime;
        myRigidBody.velocity = myRigidBody.velocity.normalized * speed; //Ensure the projectile is always moving at max speed
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.TryGetComponent<Player>(out Player hitPlayer))
        {
            hitPlayer.Hit();
            Destroy(gameObject);
        }
        else
        {
            TimesBounced++;
            if(TimesBounced > MaxBounceCount)
            {
                Destroy(gameObject);
            }
        }
    }
}