using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    public static ScreenShake Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance != null) //Ensures there is always only 1 ScreenShake, accessible through the static ScreenShake.Instance.
        {
            Destroy(gameObject);
            Debug.LogWarning("Destroyed a camera as there was already a screenshake instance");
        }
        else
        {
            Instance = this;
        }
    }
    



    List<Shake> ActiveShakes = new List<Shake>(); //All ongoing shake effects.
    //Vector3 CameraPosition; //Camera position to return to.
    public Transform trackedTransform; //The transform the camera follows

    void Update()
    {
        transform.position = new Vector3(trackedTransform.position.x, trackedTransform.position.y, -10);
        ShakeAll();
    }

    /// <summary>
    /// Queues a camera shake with the seleced time and strength.
    /// </summary>
    /// <param name="Time"></param>
    /// <param name="Strength"></param>
    public void ShakeCam(float Time, float Strength)
    {
        ActiveShakes.Add(new Shake(Time, Strength));
    }

    /// <summary>
    /// Deteriorates all active shakes and shakes by the strongest one
    /// </summary>
    void ShakeAll()
    {
        if (ActiveShakes.Count > 0)
        {
            Shake StrongestShake = new Shake(69, 420); //Serves no purpose other than to make variable not empty so errors aren't thrown
            foreach (Shake shake in ActiveShakes) //Gets the strongest shake
            {
                float MaxStrength = 0;
                //shake.RemainingTime -= Time.deltaTime;

                if (shake.RemainingStrength >= MaxStrength)
                {
                    StrongestShake = shake;
                }
            }
            ShakeCamera(StrongestShake); //Shakes by the strongest shake's stength
            List<Shake> ToBeRemoved = new List<Shake>(); //List of finished shakes to clear from list
            foreach (Shake shake in ActiveShakes)
            {
                if (shake.Deteriorate()) //If the shake is over, mark it for deletion
                {
                    ToBeRemoved.Add(shake);
                }
            }
            foreach (Shake shake in ToBeRemoved)
            {
                ActiveShakes.Remove(shake);
            }
        }
    }

    /// <summary>
    /// Moves the camera in a random direction a distance equal to the shake's strength
    /// </summary>
    /// <param name="shake"></param>
    void ShakeCamera(Shake shake)
    {
        float RandomAngle = Random.Range(0f, 90f);  //Gets a random angle up to 90 degrees

        //Gets the x and y for that angle
        float Opposite = shake.RemainingStrength * Mathf.Sin(RandomAngle);
        float Adjacent = shake.RemainingStrength * Mathf.Cos(RandomAngle);

        switch (Random.Range(1, 4))//Applies the angle to one of four quadrants of a circle.
        {
            case 1:
                transform.position = new Vector3(trackedTransform.position.x, trackedTransform.position.y, -10) + new Vector3(Adjacent, Opposite);
                break;
            case 2:
                transform.position = new Vector3(trackedTransform.position.x, trackedTransform.position.y, -10) + new Vector3(Adjacent, -Opposite);
                break;
            case 3:
                transform.position = new Vector3(trackedTransform.position.x, trackedTransform.position.y, -10) + new Vector3(-Adjacent, -Opposite);
                break;
            case 4:
                transform.position = new Vector3(trackedTransform.position.x, trackedTransform.position.y, -10) + new Vector3(-Adjacent, Opposite);
                break;
        }
    }
}
/// <summary>
/// Keeps track of Time and Strength
/// </summary>
class Shake
{
    public float StartTime;
    public float StartStrength;
    public float RemainingTime;
    public float RemainingStrength;

    public Shake(float Time, float Strength)
    {
        StartTime = Time;
        StartStrength = Strength;
        RemainingTime = Time;
        RemainingStrength = Strength;
    }
    /// <summary>
    /// Reduces time and strength of the shake
    /// </summary>
    /// <returns></returns>
    public bool Deteriorate()
    {
        RemainingTime -= Time.deltaTime;
        if (RemainingTime < 0f)
        {
            return true;
        }
        else
        {
            RemainingStrength = StartStrength * (RemainingTime / StartTime);
            return false;
        }
    }
}
