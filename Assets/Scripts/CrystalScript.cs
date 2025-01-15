using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalScript : MonoBehaviour, IHittable
{
    public Player player;
    public int shardsInCrystal = 3; // Standard amount of shards in a crystal is 3

    public void Hit() // When obj is hit
    {

        if (gameObject.tag == "Pile")
        {
            // When crystal is hit the gameObject is loses one shard and player gains one health. After all the shards are gone from the crystal it´s destroyed 
            shardsInCrystal -= 1;
            player.playerHP += 1;

            if (shardsInCrystal < 1)
            {
                Destroy(gameObject);
            }
        }

        else
        {
            // When shard is hit the gameObject is imediatley destroyed and the player gains health
            player.playerHP += 1;
            Destroy(gameObject);
        }
    }
}

