using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurntableLever : MonoBehaviour, IHittable
{
    [SerializeField] List<RailTile> connectedTables = new List<RailTile>();
    [SerializeField] Animator LeverAni;
    [SerializeField] float Delay = 0.3f;
    List<float> Timers = new List<float>(); //Keep tracks of all flicks waiting to happen (usually only 1, list is only for flicking back at the edge case of flicking the lever during the delay)
    bool Lever1 = true;
    bool Lever2 = false;

    void Update()
    {
        for(int i = Timers.Count - 1; i >= 0; i--) //Iterates over the list backwards so deleted timers won't mess up the indexing of the loop
        {
            Timers[i] += Time.deltaTime;
            if(Timers[i] > Delay)
            {
                foreach (RailTile table in connectedTables)
                {
                    table.RotateTurntable();
                }
                Timers.RemoveAt(i);
            }
        }
    }

    public void Hit()
    {
        if(ScreenShake.Instance)
        {
            ScreenShake.Instance.ShakeCam(0.07f, 0.2f);
        }

        Timers.Add(0); //Set a timer

        if(Lever2)
        {
            LeverAni.SetTrigger("LeverPressed");
            Lever2 = false;
            Lever1 = true;
        }
        else if (Lever1)
        {
            LeverAni.SetTrigger("LeverPressed");
            Lever1 = false;
            Lever2 = true;
        }
    }
}
