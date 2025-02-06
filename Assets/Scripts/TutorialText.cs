using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialText : MonoBehaviour
{
    public void Spawn()
    {
        GetComponent<SpriteRenderer>().enabled = true;
    }
    public void Despawn()
    {
        Destroy(gameObject);
    }
}
