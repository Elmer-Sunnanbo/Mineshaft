using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Torch : MonoBehaviour
{
    public bool lightOn = false;
    Light2D Light;

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
            Light = gameObject.GetComponent<Light2D>(); // Get light component
            Light.enabled = true; // Activate light component
        }
        else
        {
            Light = gameObject.GetComponent<Light2D>();
            Light.enabled = false; // Disable/inactivate light component
        }
    }
}
