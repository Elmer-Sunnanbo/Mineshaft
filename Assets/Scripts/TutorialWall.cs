using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialWall : MonoBehaviour
{
    [SerializeField] TutorialManager manager;
    [SerializeField] BoxCollider2D hitBox;

    public void Spawn()
    {
        GetComponent<SpriteRenderer>().enabled = true;
        hitBox.enabled = true;
    }
    public void Despawn()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.TryGetComponent<Player>(out _) || collision.gameObject.TryGetComponent<MinecartInteraction>(out _)) //If a player or minecart enters the trigger
        {
            manager.SkipTutorial();
        }
    }
}
