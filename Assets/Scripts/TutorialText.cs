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
    float spawnSpeed = 2;
    bool despawning;
    float despawnProgress = 0;
    float despawnSpeed = 2;
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
                spriteRenderer.color = new Color(1, 1, 1, Mathf.Lerp(0,1,spawnProgress*2));
            }
            else if(spawnProgress <= 1)
            {

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
        Destroy(gameObject);
    }
}
