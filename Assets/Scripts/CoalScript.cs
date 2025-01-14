using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoalScript : MonoBehaviour, IHittable
{
    public CoalHolder coalHolder;

    public void Hit()
    {
        // When coal is hit the gameObject is destroyed and the player gains coal

        coalHolder.amountCoal += 1;
        Destroy(gameObject);
    }
}
