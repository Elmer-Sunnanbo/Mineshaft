using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldScript : MonoBehaviour, IHittable
{
    public int goldInPile = 3; // Standard amount of gold in a coalpile is 3

    public void Hit() // When obj is hit
    {
        if (UIUpdating.instance)
        {
            UIUpdating.instance.FlashGoldUp();
        }

        if (gameObject.tag == "Pile")
        {
            // When goldpile is hit the gameObject is loses one gold and player gains one gold. After all the gold is gone from the pile it�s destroyed 
            goldInPile -= 1;
            GameManager.instance.gold += 1;

            if (goldInPile < 1)
            {
                if (ScreenShake.Instance)
                {
                    ScreenShake.Instance.ShakeCam(0.15f, 0.4f); //Screenshake
                }

                Destroy(gameObject);
            }
            else
            {
                if (ScreenShake.Instance)
                {
                    ScreenShake.Instance.ShakeCam(0.07f, 0.2f); //Screenshake
                }
                
            }
        }

        else
        {
            // When gold is hit the gameObject is imediatley destroyed and the player gains gold
            GameManager.instance.gold += 1;
            Destroy(gameObject);
            if (ScreenShake.Instance)
            {
                ScreenShake.Instance.ShakeCam(0.07f, 0.2f); //Screenshake
            }
        }
    }
}

