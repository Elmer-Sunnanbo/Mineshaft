using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Library : MonoBehaviour
{
    //Doesn't do anything, just hold classes and stuff

}
public enum Directions
{
    North,
    East,
    South,
    West,
}

public interface IHittable
{
    void Hit();
}

public interface IEnemy
{
    GameObject target { get; set; }
}
