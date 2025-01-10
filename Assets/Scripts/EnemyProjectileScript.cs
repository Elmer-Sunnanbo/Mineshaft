using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectileScript : MonoBehaviour
{
    private float speed = 7;
    private float projectileLongetivityTimer;
    private float prefireTimer;//Instantiate and colliders does not run the first couple of milliseconds.

    Rigidbody2D myRigidBody;
    BoxCollider2D myCollider;


    void Start()
    {
        projectileLongetivityTimer = 10;
        prefireTimer = 0.1f;
        myRigidBody = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<BoxCollider2D>();
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
    }
}