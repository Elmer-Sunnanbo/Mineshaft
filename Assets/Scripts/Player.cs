using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody2D myRigidbody;
    [SerializeField] float speed;
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        myRigidbody.velocity = GetMoveVector() * speed;
        if(GetMoveVector().magnitude != 0)
        {
            //We are walking
        }
    }

    Vector2 GetMoveVector()
    {
        float xComponent = 0;
        float yComponent = 0;
        if (Input.GetKey(KeyCode.D))
        {
            xComponent++;
        }
        if (Input.GetKey(KeyCode.A))
        {
            xComponent--;
        }
        if (Input.GetKey(KeyCode.W))
        {
            yComponent++;
        }
        if (Input.GetKey(KeyCode.S))
        {
            yComponent--;
        }
        return new Vector2(xComponent, yComponent).normalized;
    }
}
