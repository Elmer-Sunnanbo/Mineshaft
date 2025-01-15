using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoalScript : MonoBehaviour, IHittable
{
    public GameManager gameManager;

    public void Hit()
    {
        // When coal is hit the gameObject is destroyed and the player gains coal

        gameManager.coal += 1;
        Destroy(gameObject);
    }
}
