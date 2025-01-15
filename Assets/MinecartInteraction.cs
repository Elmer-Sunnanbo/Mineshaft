using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinecartInteraction : MonoBehaviour
{
    // Start is called before the first frame update
    public bool isInTrigger;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isInTrigger && Input.GetKeyDown(KeyCode.E))
        {
            //Do shit
            Debug.Log("you are in");
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Player playerScript))
        {
            isInTrigger = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Player playerScript))
        {
            isInTrigger = false;
        }
    }
}
