using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StalagmiteProjectileScript : MonoBehaviour
{
    private float speed = 2;
    private float InstantiateTimer = 0.5f;

    [SerializeField] GameObject Stalagmites;

    Rigidbody2D myRigidBody;
    void Start()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
    }

    
    void Update()
    {
        myRigidBody.AddForce(transform.forward * speed, ForceMode2D.Impulse);
        Instantiate(Stalagmites, transform.position, Quaternion.identity);
    }
}
