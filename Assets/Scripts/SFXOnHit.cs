using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SFXOnHit : MonoBehaviour, IHittable
{
    [SerializeField] AudioSource sound;
    public void Hit()
    {
        AudioSource.PlayClipAtPoint(sound.clip, Camera.main.transform.position);
    }
}
