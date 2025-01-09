using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StalagmiteObstacleScript : MonoBehaviour
{
    private float ObstacleActivationTimer;
    private float ObstacleDespawnTimer;
    BoxCollider2D ObstacleCol;
    void Start()
    {
        ObstacleCol = GetComponent<BoxCollider2D>();
        ObstacleCol.enabled = false;
        ObstacleActivationTimer = 0.2f;
        ObstacleDespawnTimer = 2;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(ObstacleDespawnTimer < 0)
        {
            Destroy(gameObject);
        }
        ObstacleDespawnTimer -= Time.deltaTime;
    }
    void Update()
    {
        if(ObstacleActivationTimer < 0)
        {
            ObstacleCol.enabled = true;
        }
        ObstacleActivationTimer -= Time.deltaTime;
    }
}
