using UnityEngine;
using DG.Tweening; // Ensure DoTween is imported

public class GrassAnimation : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the collided object has the tag "Player"
        if (collision.CompareTag("Player"))
        {
            // Animate the grass to tilt left and right, then return to its original rotation
            transform.DORotate(new Vector3(0, 0, -15f), 0.2f) // Tilt to the left
                .OnComplete(() =>
                {
                    transform.DORotate(new Vector3(0, 0, 15f), 0.2f) // Tilt to the right
                        .OnComplete(() =>
                        {
                            transform.DORotate(Vector3.zero, 0.2f); // Return to original rotation
                        });
                });
        }
    }
}
