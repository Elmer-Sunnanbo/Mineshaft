using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldScript : MonoBehaviour, IHittable
{
    public GameManager gameManager;
    public int goldInPile = 3; // Standard amount of gold in a coalpile is 3

    public void Hit() // When obj is hit
    {

        if (gameObject.tag == "Pile")
        {
            // When goldpile is hit the gameObject is loses one gold and player gains one gold. After all the gold is gone from the pile it´s destroyed 
            goldInPile -= 1;
            gameManager.gold += 1;

            if (goldInPile < 1)
            {
                Destroy(gameObject);
            }
        }

        else
        {
            // When gold is hit the gameObject is imediatley destroyed and the player gains gold
            gameManager.gold += 1;
            Destroy(gameObject);
        }
    }
}

