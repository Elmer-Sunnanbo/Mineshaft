using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    Rigidbody2D myRigidbody;
    [SerializeField] float speed;

    public static Player playerInstance;
    public int playerHP;
    float iFrameTimer;

    bool isInMinecart;
    void Start()
    {
        playerInstance = this;
        myRigidbody = GetComponent<Rigidbody2D>();
        playerHP = 4;
    }


    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            PlayerAni.SetTrigger("Swing");
        }
        iFrameTimer -= Time.deltaTime;
        myRigidbody.velocity = GetMoveVector() * speed;
        Vector2 movement = GetMoveVector();
        if(movement.x > 0)
        {
            PlayerAni.SetBool("Walk_H", true);
        }
        else
        {
            PlayerAni.SetBool("Walk_H", false);
        }
        if (movement.x < 0)
        {
            PlayerAni.SetBool("Walk_V", true);
        }
        else
        {
            PlayerAni.SetBool("Walk_V", false);
        }
        if (movement.y > 0)
        {
            PlayerAni.SetBool("Walk_B", true);
        }
        else
        {
            PlayerAni.SetBool("Walk_B", false);
        }
        if (movement.y < 0)
        {
            PlayerAni.SetBool("Walk_F", true);
        }
        else
        {
            PlayerAni.SetBool("Walk_F", false);
        }
        if(playerHP < 1)
        {
            Debug.Log("Player died now and here");
            SceneManager.LoadScene(1);
        }
    }
    public Animator PlayerAni;
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

    public void Hit()
    {
        if (iFrameTimer < 0 && !isInMinecart)
        {
            if(ScreenShake.Instance)
            {
                ScreenShake.Instance.ShakeCam(0.2f, 0.4f);
            }

            if (UIUpdating.instance)
            {
                UIUpdating.instance.FlashHPDown();
            }
            playerHP -= 1;
            iFrameTimer = 1;
        }
    }

    public void PiercingHit()
    {
        if (iFrameTimer < 0 && !isInMinecart)
        {
            if (ScreenShake.Instance)
            {
                ScreenShake.Instance.ShakeCam(0.2f, 0.2f);
            }
            if (UIUpdating.instance)
            {
                UIUpdating.instance.FlashHPDown();
            }
                
            playerHP -= 1;
            iFrameTimer = 1;
            myRigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }

    public void UnPierce()
    {
        if(!isInMinecart)
        {
            myRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }

    public void EnterMinecart()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        isInMinecart = true;
        myRigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
    }
    public void ExitMinecart()
    {
        GetComponent<SpriteRenderer>().enabled = true;
        isInMinecart = false;
        myRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
}
