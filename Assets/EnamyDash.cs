using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnamyDash : MonoBehaviour
{
    public float enemyHealth;
    public float dashDuration = 0.5f; // Duration of the dash
    private Vector2 targetPosition;

    public void OnTriggerEnter2D(Collider2D col)
    {
        Vector2 playerPosition = col.transform.position; // Get player position
        StartCoroutine(DashTowardsPlayer(playerPosition));
    }

    private IEnumerator DashTowardsPlayer(Vector2 playerPosition)
    {
        Vector2 startPosition = transform.position;
        float elapsedTime = 0;

        while (elapsedTime < dashDuration)
        {
            transform.position = Vector2.Lerp(startPosition, playerPosition, (elapsedTime / dashDuration));
            elapsedTime += Time.deltaTime; // Increment elapsed time
            yield return null; // Wait for the next frame
        }

        // Ensure the final position is exactly the player's position
        transform.position = playerPosition;
    }
}