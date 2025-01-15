using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StalagmiteObstacleScript : MonoBehaviour
{
    [SerializeField] float ObstacleActivationTimer;
    [SerializeField] float ObstacleDespawnTimer;
    BoxCollider2D ObstacleCol;
    void Start()
    {
        ObstacleCol = GetComponent<BoxCollider2D>();
        ObstacleCol.enabled = false;
    }


    void Update()
    {
        ObstacleActivationTimer -= Time.deltaTime;
        ObstacleDespawnTimer -= Time.deltaTime;
        if (ObstacleActivationTimer < 0)
        {
            ObstacleCol.enabled = true;
        }
        if (ObstacleDespawnTimer < 0)
        {
            Destroy(gameObject);
        }
    }
}
