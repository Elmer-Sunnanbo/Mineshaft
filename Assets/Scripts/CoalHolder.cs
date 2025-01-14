using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoalHolder : MonoBehaviour
{
    public int amountCoal;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("Amount of coal: " + amountCoal);
        }
    }
}
