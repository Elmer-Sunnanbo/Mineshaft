using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldScript : MonoBehaviour, IHittable
{
    public GameManager gameManager;

    public void Hit()
    {
        // When coal is hit the gameObject is destroyed and the player gains coal

        gameManager.gold += 1;
        Destroy(gameObject);
    }
}

