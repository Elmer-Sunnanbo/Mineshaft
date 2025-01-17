using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurntableLever : MonoBehaviour, IHittable
{
    [SerializeField] List<RailTile> connectedTables = new List<RailTile>();
    [SerializeField] Animator LeverAni;
    bool Lever1 = true;
    bool Lever2 = false;
    public void Hit()
    {
        foreach (RailTile table in connectedTables)
        {
            table.RotateTurntable();
        }
        if(Lever2)
        {
            LeverAni.SetBool("LeverPressed2", false);
            LeverAni.SetBool("LeverPressed1", true);
            Lever2 = false;
            Lever1 = true;
        }
        else if (Lever1)
        {
            LeverAni.SetBool("LeverPressed1", false);
            LeverAni.SetBool("LeverPressed2", true);
            Lever1 = false;
            Lever2 = true;
        }
    }
}
