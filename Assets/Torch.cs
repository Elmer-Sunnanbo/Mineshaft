using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Torch : MonoBehaviour
{
    public bool lightOn = false;
    Light2D Light;
    Animator animator;

    private void Awake()
    {
        Light = gameObject.GetComponent<Light2D>();
        animator = GetComponent<Animator>();
    }

    public void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.TryGetComponent<Player>(out _))
        {
            lightOn = true;
        }
    }

    private void Update()
    {
        if(lightOn) // If lightOn bool = true
        {
            Light.enabled = true; // Activate light component
            animator.SetBool("Lit", true);
        }
        else
        {
            Light.enabled = false; // Disable/inactivate light component
            animator.SetBool("Lit", false);
        }
    }
}
