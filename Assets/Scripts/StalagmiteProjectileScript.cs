using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StalagmiteProjectileScript : MonoBehaviour
{
    private float speed = 6;
    private float instantiateTimer;
    private float projectileLongetivityTimer;
    private float prefireTimer;//Instantiate does not run the first couple of milliseconds.

    [SerializeField] GameObject Stalagmites;

    Rigidbody2D myRigidBody;
    void Start()
    {
        projectileLongetivityTimer = 3.5f;
        prefireTimer = 0.7f;
        myRigidBody = GetComponent<Rigidbody2D>();
    }

    
    void Update()
    {
        if(prefireTimer < 0)
        {
            if (instantiateTimer < 0)
            {
                Instantiate(Stalagmites, transform.position, Quaternion.identity);
                instantiateTimer = 0.2f;
            }
            instantiateTimer -= Time.deltaTime;
        }
        prefireTimer -= Time.deltaTime;
    }
    void FixedUpdate()
    {
        myRigidBody.AddForce(transform.forward * speed, ForceMode2D.Impulse);
        if (projectileLongetivityTimer < 0)
        {
            Destroy(gameObject);
        }
        projectileLongetivityTimer -= Time.deltaTime;
    }
}
