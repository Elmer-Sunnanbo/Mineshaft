using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailTile : MonoBehaviour
{
    [SerializeField] RailTile[] neighbours;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector2 GetPosition(float progress)
    {
        return (Vector2) transform.position + new Vector2(0,progress - 0.5f);
    }
}
