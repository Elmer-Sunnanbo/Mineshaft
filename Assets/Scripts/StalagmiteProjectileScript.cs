using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StalagmiteProjectileScript : MonoBehaviour
{
    private float speed = 7;
    private float instantiateTimer;
    private float projectileLongetivityTimer;
    private float prefireTimer;//Instantiate and colliders does not run the first couple of milliseconds.

    [SerializeField] GameObject Stalagmites;

    Rigidbody2D myRigidBody;
    BoxCollider2D myCollider;


    void Start()
    {
        projectileLongetivityTimer = 1;
        prefireTimer = 0.2f;
        myRigidBody = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<BoxCollider2D>();
        myCollider.enabled = false;
    }

    
    void Update()
    {
        if(prefireTimer < 0)
        {
            if (instantiateTimer < 0)
            {
                Instantiate(Stalagmites, transform.position, Quaternion.identity);
                instantiateTimer = 0.1f;
            }
            myCollider.enabled = true;
            instantiateTimer -= Time.deltaTime;
        }
        prefireTimer -= Time.deltaTime;
    }
    void FixedUpdate()
    {
        myRigidBody.velocity = transform.right * speed;
        if (projectileLongetivityTimer < 0)
        {
            Destroy(gameObject);
        }
        projectileLongetivityTimer -= Time.deltaTime;
        myRigidBody.velocity = myRigidBody.velocity.normalized * speed; //Ensure the projectile is always moving at max speed
    }
    private void OnCollisionEnter2D(Collision2D col)
    {
        if(col.gameObject.TryGetComponent<PathfindingBlocker>(out _))
        {
            Destroy(gameObject);
        }
    }
}
