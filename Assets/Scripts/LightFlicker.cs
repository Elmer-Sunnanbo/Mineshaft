using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Internal.Commands;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightFlicker : MonoBehaviour
{
    [SerializeField] Light2D controlledLight;
    [SerializeField] float smallFlickerStrength;
    [SerializeField] float smallFlickerSpeed;
    [SerializeField] float smallFlickerSpeedIntensity;
    [SerializeField] float smallFlickerStrengthIntensity;
    [SerializeField] float bigFlickerSpeed;
    [SerializeField] float bigFlickerRadiusStrength;
    [SerializeField] float bigFlickerIntensityStrength;
    [SerializeField] bool blink;
    [SerializeField] float averageBlinkDelay;
    [SerializeField] float blinkDuration;
    float standardRadiusOuter;
    float standardRadiusInner;
    float standardIntensity;
    float smallFlickerTarget;
    float smallFlickerTargetIntensity;
    float bigFlickerTarget;
    float bigFlickerStart;
    float bigFlickerProgress;
    float blinkModifier = 1;

    float currentRadius = 0; //Value representing how far above/below the standard radius the light is
    float currentIntensity = 1; //Value representing how far above 0 the light's intensity is

    float smallFlickerRadius;
    float smallFlickerRadiusIntensity;
    float bigFlickerRadius;

    float timeUntilNextBlink;
    float blinkTimeElapsed;
    void Start()
    {
        standardRadiusOuter = controlledLight.pointLightOuterRadius;
        standardRadiusInner = controlledLight.pointLightInnerRadius;
        standardIntensity = controlledLight.intensity;
        timeUntilNextBlink = averageBlinkDelay;
    }

    void Update()
    {
        //Small flicker radius
        if(Mathf.Abs(smallFlickerTarget - smallFlickerRadius) <= smallFlickerSpeed * Time.deltaTime) //If we're at the target
        {
            smallFlickerRadius = smallFlickerTarget;
            smallFlickerTarget = Random.Range(-1f, 1f);
        }
        else
        {
            if(smallFlickerRadius < smallFlickerTarget)
            {
                smallFlickerRadius += smallFlickerSpeed * Time.deltaTime;
            }
            else
            {
                smallFlickerRadius -= smallFlickerSpeed * Time.deltaTime;
            }
        }

        //Small flicker intensity
        if (Mathf.Abs(smallFlickerTargetIntensity - smallFlickerRadiusIntensity) <= smallFlickerSpeedIntensity * Time.deltaTime) //If we're at the target
        {
            smallFlickerRadiusIntensity = smallFlickerTargetIntensity;
            smallFlickerTargetIntensity = Random.Range(-1f, 1f);
        }
        else
        {
            if (smallFlickerRadiusIntensity < smallFlickerTargetIntensity)
            {
                smallFlickerRadiusIntensity += smallFlickerSpeedIntensity * Time.deltaTime;
            }
            else
            {
                smallFlickerRadiusIntensity -= smallFlickerSpeedIntensity * Time.deltaTime;
            }
        }

        //Big flicker intensity
        if (bigFlickerProgress + bigFlickerSpeed * Time.deltaTime > 1) //If we're at the target
        {
            bigFlickerProgress = 0;
            bigFlickerRadius = bigFlickerTarget;
            bigFlickerStart = bigFlickerTarget;
            bigFlickerTarget = smallFlickerTarget = Random.Range(-1f, 1f);
        }
        else
        {
            bigFlickerProgress += Time.deltaTime * bigFlickerSpeed;
            bigFlickerRadius = Mathf.Lerp(bigFlickerStart, bigFlickerTarget, bigFlickerProgress);
        }

        //Blink
        if(blink)
        {
            if (blinkTimeElapsed > 0)
            {
                //Continue blink
                blinkTimeElapsed += Time.deltaTime / blinkDuration;
                if (blinkTimeElapsed > 1)
                {
                    //End blink
                    blinkTimeElapsed = 0;
                    blinkModifier = 1;
                    if (Random.Range(0, 2) == 1)
                    {
                        timeUntilNextBlink = Random.Range(0.05f, 0.2f);
                    }
                    else
                    {
                        timeUntilNextBlink = averageBlinkDelay * Random.Range(0.3f, 3f);
                    }
                }
                else if (blinkTimeElapsed > 0.75f)
                {
                    blinkModifier = (Mathf.Lerp(0, 1, blinkTimeElapsed * 2 - 1) - 0.5f) * 2;
                }
                else if (blinkTimeElapsed < 0.25f)
                {
                    blinkModifier = (Mathf.Lerp(1, 0, blinkTimeElapsed * 2) - 0.5f) * 2;
                }
                else
                {
                    blinkModifier = 0;
                }
            }
            else if (timeUntilNextBlink <= 0)
            {
                //Start blink
                blinkTimeElapsed += Time.deltaTime;
                blinkModifier = Mathf.Lerp(1, 0, blinkTimeElapsed);
            }
            else
            {
                //Approach next blink
                timeUntilNextBlink -= Time.deltaTime;
            }
        }

        //Sum everything up
        currentRadius = (smallFlickerRadius * smallFlickerStrength + bigFlickerRadius * bigFlickerRadiusStrength);
        currentIntensity = (1 + smallFlickerRadiusIntensity * smallFlickerStrengthIntensity + bigFlickerRadius * bigFlickerIntensityStrength);

        //Apply values to light
        controlledLight.pointLightOuterRadius = standardRadiusOuter *  (1 + currentRadius) * blinkModifier;
        controlledLight.pointLightInnerRadius = standardRadiusInner * (1 + currentRadius) * blinkModifier;
        controlledLight.intensity = standardIntensity * currentIntensity * blinkModifier;
    }
}
