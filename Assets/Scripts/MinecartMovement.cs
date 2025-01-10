using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinecartMovement : MonoBehaviour
{
    [SerializeField] RailTile currentTile;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = currentTile.GetPosition(0.7f);
    }
}
