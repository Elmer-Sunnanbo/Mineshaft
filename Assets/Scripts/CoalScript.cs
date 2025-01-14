using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoalScript : MonoBehaviour, IHittable
{
    public CoalHolder coalHolder;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Hit();
        }

    }

    public void Hit()
    {
        // When coal is hit the gameObject is destroyed and the player gains coal

        coalHolder.amountCoal += 1;
        Destroy(gameObject);
    }
}
