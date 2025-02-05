using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footstepper : MonoBehaviour
{
    [SerializeField] AudioSource stepSource;
    [SerializeField] float timeBetweenSteps;
    public bool walking;
    float timeUntilNextStep = 0;
    void Start()
    {
        
    }

    void Update()
    {
        if(walking)
        {
            timeUntilNextStep -= Time.deltaTime;
            if(timeUntilNextStep <= 0)
            {
                stepSource.PlayOneShot(stepSource.clip);
                timeUntilNextStep += timeBetweenSteps;
            }
        }
        else
        {
            timeUntilNextStep = 0;
        }
    }
}
