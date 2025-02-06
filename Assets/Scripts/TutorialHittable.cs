using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialHittable : MonoBehaviour, IHittable
{
    [SerializeField] int id;
    [SerializeField] TutorialManager manager;
    public void Hit()
    {
        manager.HitObject(id);
    }
}
