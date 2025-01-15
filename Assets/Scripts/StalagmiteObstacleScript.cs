using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StalagmiteObstacleScript : MonoBehaviour
{
    [SerializeField] float ObstacleActivationTimer;
    [SerializeField] float ObstacleDespawnTimer;
    BoxCollider2D ObstacleCol;
    List<Player> HitPlayers = new List<Player>();
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
            foreach (RaycastHit2D hit in Physics2D.BoxCastAll(transform.position, new Vector2(0.2f,0.2f), 0, Vector2.zero, 0))
            {
                if(hit.collider.gameObject.TryGetComponent(out Player player))
                {
                    player.PiercingHit();
                    HitPlayers.Add(player);
                }
            }

            ObstacleCol.enabled = true;
        }
        if (ObstacleDespawnTimer < 0)
        {
            foreach(Player player in HitPlayers)
            {
                player.UnPierce();
            }
            Destroy(gameObject);
        }
    }
}
