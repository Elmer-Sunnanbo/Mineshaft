using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalScript : MonoBehaviour, IHittable
{
    Player player;
    public int shardsInCrystal = 3; // Standard amount of shards in a crystal is 3

    void Start()
    {
        player = GameManager.instance.player.GetComponent<Player>();
    }

    public void Hit() // When obj is hit
    {
        

        if (gameObject.tag == "Pile")
        {
            // When crystal is hit the gameObject is loses one shard and player gains one health. After all the shards are gone from the crystal it´s destroyed 
            shardsInCrystal -= 1;
            player.playerHP += 1;

            if (shardsInCrystal < 1)
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
            // When shard is hit the gameObject is imediatley destroyed and the player gains health
            player.playerHP += 1;
            Destroy(gameObject);
            ScreenShake.Instance.ShakeCam(0.07f, 0.2f); //Screenshake
        }
    }
}

