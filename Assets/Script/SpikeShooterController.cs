using System.Collections;
using UnityEngine;

public class SpikeShooterController : MonoBehaviour
{
    [SerializeField] private GameObject projectile; // Projectile prefab
    [SerializeField] private Transform spawnPoint;  // Point from where projectiles are fired
    [SerializeField] private float projectileSpeed = 10f; // Speed of the projectile
    [SerializeField] private float lifetime = 5f; // Time before the projectile is destroyed
    [SerializeField] private float shootInterval = 1f; // Time interval between shots

    private float shootTimer; // Tracks time since the last shot
    [SerializeField] private bool isFlipped;

    private void Update()
    {
        // Increment the shoot timer
        shootTimer += Time.deltaTime;

        // Check if it's time to shoot
        if (shootTimer >= shootInterval)
        {
            if (isFlipped) ShootProjectileLeft(); else ShootProjectile();
            shootTimer = 0f;
        }
    }

    private void ShootProjectile()
    {
        if (projectile != null && spawnPoint != null)
        {
            // Instantiate the projectile at the spawn point
            GameObject newProjectile = Instantiate(projectile, spawnPoint.position, Quaternion.identity);

            newProjectile.transform.rotation = Quaternion.Euler(0f, 0f, -90f);

            // Add velocity to the projectile
            Rigidbody2D rb = newProjectile.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = spawnPoint.right * projectileSpeed;
            }

            // Destroy the projectile after a specified lifetime
            Destroy(newProjectile, lifetime);
        }
    }

    private void ShootProjectileLeft()
    {
        if (projectile != null && spawnPoint != null)
        {
            // Instantiate the projectile at the spawn point
            GameObject newProjectile = Instantiate(projectile, spawnPoint.position, Quaternion.identity);

            // Rotate the projectile to face the firing direction
            newProjectile.transform.rotation = Quaternion.Euler(0f, 0f, 90f);

            // Add velocity to the projectile
            Rigidbody2D rb = newProjectile.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = -spawnPoint.right * projectileSpeed;
            }

            // Destroy the projectile after a specified lifetime
            Destroy(newProjectile, lifetime);
        }
    }

    //private IEnumerator IEHideObject(GameObject proyektil)
    //{
    //    yield return new WaitForSeconds(lifetime);
    //    proyektil.SetActive(false);
    //}
}
