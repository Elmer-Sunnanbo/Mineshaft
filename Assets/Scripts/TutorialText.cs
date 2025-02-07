using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TutorialText : MonoBehaviour
{
    float bobProgress = 0;
    Vector2 truePosition;
    float bobAmount = 0.1f;
    float bobSpeed = 1;
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
        truePosition = transform.position;
    }

    void Update()
    {
        //Float up and down
        bobProgress += Time.deltaTime * bobSpeed;
        while(bobProgress > 2) //Loop back to 0
        {
            bobProgress -= 2;
        }
        if(bobProgress < 1)
        {
            transform.position = truePosition + new Vector2(0, Mathf.Lerp(0,bobAmount,bobProgress));
        }
        else
        {
            transform.position = truePosition + new Vector2(0, Mathf.Lerp(bobAmount, 0, bobProgress-1));
        }

        //Spawn logic
        if(spawning)
        {
            spawnProgress += Time.deltaTime * spawnSpeed;
            if(spawnProgress <= 0.5f)
            {
                //Become less transparent and more blue
                spriteRenderer.color = new Color(Mathf.Lerp(1, 0.5f, spawnProgress * 2), Mathf.Lerp(1, 0.5f, spawnProgress * 2), 1, Mathf.Lerp(0,1,spawnProgress*2));
            }
            else if(spawnProgress <= 1)
            {
                //Become less blue
                spriteRenderer.color = new Color(Mathf.Lerp(0.5f, 1, (spawnProgress - 0.5f) * 2), Mathf.Lerp(0.5f, 1, (spawnProgress-0.5f) * 2), 1, 1);
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
    /// <summary>
    /// Makes the text appear
    /// </summary>
    public void Spawn()
    {
        spawning = true;
    }
    /// <summary>
    /// Makes the text disappear
    /// </summary>
    public void Despawn()
    {
        despawning = true;
    }
}
