using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Burst.CompilerServices;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PlayerAttack : MonoBehaviour
{
    public Vector2 hitDirection;
    public float reach; // The maximum distance something can be from the player for the player to still be able to hit it
    [SerializeField] float attackReloadTimer;
    [SerializeField] GameObject swing;
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

                Vector2 raycastOrigin = transform.position;

                // Create raycast for wall detection
                RaycastHit2D[] hitsRay = Physics2D.RaycastAll(raycastOrigin, direction, reach); // Send raycast

                float attackDirectionDegrees = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                // Create circlecast for hit detection
                RaycastHit2D[] hitsCircle = Physics2D.CircleCastAll(raycastOrigin, 0.3f, direction, reach); //Send circlecast
                Vector2 swingPosition = Vector2.zero;
                float? wallDistance = null; //The distance to a wall (hits beyond this distance are invalid)
                foreach (RaycastHit2D hit in hitsCircle)
                {
                    if (hit.collider != null) // If a hitObj is found
                    {
                        if(wallDistance != null) //Ignore the hit if it's past a wall
                        {
                            if (hit.distance > wallDistance)
                            {
                                continue;
                            }
                        }
                        
                        //Hit everything that should be hit
                        foreach (IHittable hittable in hit.collider.GetComponents<IHittable>())
                        {
                            //Hit the hittable
                            hittable.Hit();

                            //Place the swing on the furthest object hit
                            //(this code always updates the position to anything with a hittable and the objects are handled from near to far)
                            swingPosition = (Vector2)transform.position + direction * (hit.distance + 0.5f); 
                        }

                        if (hit.collider.TryGetComponent<PathfindingBlocker>(out _)) //If we hit a wall
                        {
                            if (hitsRay.ToList().Select(t => t.collider.gameObject).Contains(hit.collider.gameObject)) //If the raycast also hit the wall
                            {
                                //Get the raycasthit that hit the wall
                                RaycastHit2D matchedRay = new RaycastHit2D();
                                float matchDistance = 0;
                                foreach(RaycastHit2D rayHit in hitsRay)
                                {
                                    Debug.Log(rayHit.distance);
                                    if (rayHit.collider.gameObject == hit.collider.gameObject)
                                    {
                                        matchedRay = rayHit;
                                        matchDistance = rayHit.distance;
                                        break;
                                    }
                                }
                                if(matchedRay = new RaycastHit2D()) //If the loop failed to find a ray
                                {
                                    Debug.LogError("Player attack failed to match a wall ray");
                                    continue;
                                }
                                if(wallDistance == null)
                                {
                                    wallDistance = matchDistance; //Ignore all hits beyond the wall
                                }
                                else if (wallDistance > matchDistance)
                                {
                                    wallDistance = matchDistance; //Ignore all hits beyond the wall
                                }
                            }
                        }
                    }
                }
                
                if(swingPosition == Vector2.zero) //If we didn't hit anything
                {
                    if(wallDistance != null) //If we hit a wall
                    {
                        swingPosition = (Vector2)transform.position + direction * ((float) wallDistance); //Place the swing on the wall
                    }
                    else
                    {
                        swingPosition = (Vector2)transform.position + direction * reach; //Place the swing at the end of our reach
                    }
                    
                }
                timer = attackReloadTimer;

                //Create swing effect
                Instantiate(swing, swingPosition, Quaternion.Euler(0, 0, attackDirectionDegrees - 90));
            }
        }
        else //the attack reload counts down if you release an attack
        {
            timer -= Time.deltaTime; 
        }
        
    }
}
