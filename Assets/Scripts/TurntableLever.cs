using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurntableLever : MonoBehaviour, IHittable
{
    [SerializeField] List<RailTile> connectedTables = new List<RailTile>();
    public void Hit()
    {
        foreach (RailTile table in connectedTables)
        {
            table.RotateTurntable();
        }
    }
}
