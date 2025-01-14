using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    Rigidbody2D myRigidbody;
    [SerializeField] float speed;

    public int playerHP;
    float iFrameTimer;
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        playerHP = 4;
    }


    void Update()
    {
        myRigidbody.velocity = GetMoveVector() * speed;
        if(GetMoveVector().magnitude != 0)
        {
            //We are walking
        }
        iFrameTimer -= Time.deltaTime;
        if(playerHP < 1)
        {
            Debug.Log("Player died now and here");
            SceneManager.LoadScene(1);
        }
    }
    private void OnCollisionStay2D(Collision2D col)
    {
        if(col.gameObject.tag == "enemy")
        {
            if(iFrameTimer < 0)
            {
                playerHP -= 1;
                iFrameTimer = 1;
            }
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
