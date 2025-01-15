using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PlayerAttack : MonoBehaviour
{
    public Vector2 hitDirection;
    public float reach; // The maximum distance something can be from the player for the player to still be able to hit it
    [SerializeField] float attackReloadTimer;
    float timer;

    // Start is called before the first frame update
    void Start()
    {
        reach = 3;
        timer = attackReloadTimer;
    }

    // Update is called once per frame
    void Update()
    {
        if(timer < 0)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0)) // If mouse (left-)button is clicked
            {
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); // Get the mouse position in world coordinates
    
                // Calculate the direction from the player to the mouse position
                Vector2 direction = (mousePosition - (Vector2)transform.position).normalized;

                // Offset the starting position by 1 unit in the direction of the mouse
                Vector2 raycastOrigin = (Vector2)transform.position + direction; // Offset by 1 unit

                // Create the raycast
                RaycastHit2D hit = Physics2D.Raycast(raycastOrigin, direction, reach); // Send raycast

                if (hit.collider != null) // If a hitObj is found
                {
                    //Hit everything that should be hit
                    foreach (IHittable hittable in hit.collider.GetComponents<IHittable>())
                    {
                        hittable.Hit();
                    }
                }
                timer = attackReloadTimer;
            }
        }
        else //the attack reload counts down if you release an attack
        {
            timer -= Time.deltaTime; 
        }
        
    }
}
