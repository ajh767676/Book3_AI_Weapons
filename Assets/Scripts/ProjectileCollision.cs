using System.Collections.Generic;
using UnityEngine;

public class ProjectileCollision : MonoBehaviour
{
    public GameObject explosionEffect;
    public float explosionRadius = 3f;
    public int explosionDamage = 30;

    private bool hasExploded = false;

    void OnCollisionEnter(Collision collision)
    {
        if (hasExploded)
            return;

        hasExploded = true;

        if (explosionEffect != null)
        {
            GameObject explosion = Instantiate(
                explosionEffect,
                transform.position,
                Quaternion.identity
            );

            Destroy(explosion, 3f);
        }

        Collider[] nearbyColliders = Physics.OverlapSphere(
            transform.position,
            explosionRadius
        );

        HashSet<EnemyHealth> damagedEnemies =
            new HashSet<EnemyHealth>();

        foreach (Collider nearbyCollider in nearbyColliders)
        {
            EnemyHealth enemy =
                nearbyCollider.GetComponentInParent<EnemyHealth>();

            if (enemy != null && damagedEnemies.Add(enemy))
            {
                enemy.TakeDamage(explosionDamage);
            }
        }

        Destroy(gameObject);
    }
}