using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoalScript : MonoBehaviour, IHittable
{
    public int coalInPile = 3; // Standard amount of coal in a coalpile is 3

    public void Hit() // When obj is hit
    {
        
        if (gameObject.tag == "Pile")
        {
            // When coalpile is hit the gameObject is loses one coal and player gains one coal. After all the coal is gone from the pile it´s destroyed 
            coalInPile -= 1;
            GameManager.instance.coal += 1;

            if(coalInPile < 1)
            {
                Destroy(gameObject);
            }
        }

        else
        {
            // When coal is hit the gameObject is imediatley destroyed and the player gains coal
            GameManager.instance.coal += 1;
            Destroy(gameObject);
        }
    }
}
