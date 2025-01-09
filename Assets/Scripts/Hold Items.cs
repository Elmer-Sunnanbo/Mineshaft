using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldItems : MonoBehaviour
{
    public int HeldItemID;
    //0 = no item
    //1: Pickaxe
    //2: Torch
    void Start()
    {
        HeldItemID = 0;
    }
}