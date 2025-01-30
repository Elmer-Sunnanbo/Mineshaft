using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoalScript : MonoBehaviour, IHittable
{
    public int coalInPile = 3; // Standard amount of coal in a coalpile is 3

    public void Hit() // When obj is hit
    {
        UIUpdating.instance.FlashCoalUp();

        if (gameObject.tag == "Pile")
        {
            // When coalpile is hit the gameObject is loses one coal and player gains one coal. After all the coal is gone from the pile it´s destroyed 
            coalInPile -= 1;
            GameManager.instance.coal += 1;

            if(coalInPile < 1)
            {
                ScreenShake.Instance.ShakeCam(0.15f, 0.4f); //Screenshake
                Destroy(gameObject);
            }
            else
            {
                ScreenShake.Instance.ShakeCam(0.07f, 0.2f); //Screenshake
            }
        }

        else
        {
            // When coal is hit the gameObject is imediatley destroyed and the player gains coal
            GameManager.instance.coal += 1;
            Destroy(gameObject);
            ScreenShake.Instance.ShakeCam(0.07f, 0.2f); //Screenshake
        }
    }
}
