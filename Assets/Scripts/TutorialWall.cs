using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialWall : MonoBehaviour
{
    [SerializeField] TutorialManager manager;
    [SerializeField] BoxCollider2D hitBox;
    bool spawning;
    float spawnProgress = 0;
    float spawnSpeed = 1.5f;
    bool despawning;
    float despawnProgress = 0;
    float despawnSpeed = 3;
    SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        //Spawn logic
        if (spawning)
        {
            spawnProgress += Time.deltaTime * spawnSpeed;
            if (spawnProgress <= 0.5f)
            {
                //Become less transparent and more blue
                spriteRenderer.color = new Color(Mathf.Lerp(1, 0.5f, spawnProgress * 2), Mathf.Lerp(1, 0.5f, spawnProgress * 2), 1, Mathf.Lerp(0, 1, spawnProgress * 2));
            }
            else if (spawnProgress <= 1)
            {
                //Become less blue
                spriteRenderer.color = new Color(Mathf.Lerp(0.5f, 1, (spawnProgress - 0.5f) * 2), Mathf.Lerp(0.5f, 1, (spawnProgress - 0.5f) * 2), 1, 1);
            }
            else
            {
                spriteRenderer.color = new Color(1, 1, 1, 1);
                spawning = false;
            }
        }

        //Depawn logic
        if (despawning)
        {
            despawnProgress += Time.deltaTime * despawnSpeed;
            if (despawnProgress <= 0.5f)
            {
                //Become more blue
                spriteRenderer.color = new Color(Mathf.Lerp(1, 0.5f, despawnProgress * 2), Mathf.Lerp(1, 0.5f, despawnProgress * 2), 1, 1);
            }
            else if (despawnProgress <= 1)
            {
                //Become more transparent and less blue
                spriteRenderer.color = new Color(Mathf.Lerp(0.5f, 1, (despawnProgress - 0.5f) * 2), Mathf.Lerp(0.5f, 1, (despawnProgress - 0.5f) * 2), 1, Mathf.Lerp(1, 0, (despawnProgress - 0.5f) * 2));
            }
            else
            {
                spriteRenderer.color = new Color(1, 1, 1, 0);
                despawning = false;
            }
        }
    }

    public void Spawn()
    {
        spawning = true;
        hitBox.enabled = true;
    }
    public void Despawn()
    {
        despawning = true;
        hitBox.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.TryGetComponent<Player>(out _) || collision.gameObject.TryGetComponent<MinecartInteraction>(out _)) //If a player or minecart enters the trigger
        {
            manager.SkipTutorial();
        }
    }
}
