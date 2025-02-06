using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StalagmiteProjectileScript : MonoBehaviour
{
    private float speed = 7;
    private float instantiateTimer;
    private float projectileLongetivityTimer;
    private float instantiationTime;
    private float prefireTimer;//Instantiate and colliders does not run the first couple of milliseconds.
    private bool isInstantiating;
    

    [SerializeField] GameObject Stalagmites;

    Rigidbody2D myRigidBody;
    BoxCollider2D myCollider;


    void Start()
    {
        projectileLongetivityTimer = 4f;
        instantiationTime = 1.3f;  
        prefireTimer = 0.2f;
        myRigidBody = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<BoxCollider2D>();
        myCollider.enabled = false;
        isInstantiating = true;
        ScreenShake.Instance.ShakeCam(0.15f, 0.4f);
    }

    
    void Update()
    {
        if(prefireTimer < 0)
        {
            if (instantiateTimer < 0 && isInstantiating == true)
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
        if (instantiationTime < 0)
        {
            isInstantiating = false;
        }
        if(projectileLongetivityTimer < 0)
        {
            Destroy(gameObject);
        }
        instantiationTime -= Time.deltaTime;
        projectileLongetivityTimer -= Time.deltaTime;
        myRigidBody.velocity = myRigidBody.velocity.normalized * speed; //Ensure the projectile is always moving at max speed
    }
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.TryGetComponent<PathfindingBlocker>(out _))
        {
            isInstantiating = false;
        }
    }
}
