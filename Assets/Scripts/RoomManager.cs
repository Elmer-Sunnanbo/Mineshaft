using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public RailTile borderRailNorth;
    public RailTile borderRailEast;
    public RailTile borderRailSouth;
    public RailTile borderRailWest;

    public List<GameObject> enemiesInRoom;

    public void PrepEnemies(GameObject target)
    {
        foreach(GameObject enemy in enemiesInRoom)
        {
            //Set target to player
            foreach(IEnemy enemyScript in enemy.GetComponents<IEnemy>())
            {
                enemyScript.SetTarget(target);
            }
            //Detach from room
            enemy.transform.SetParent(null);
        }
    }
}
