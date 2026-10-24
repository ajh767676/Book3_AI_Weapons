using UnityEngine;

public class ProjectileCollision : MonoBehaviour
{
    public GameObject explosionEffect;

    void OnCollisionEnter(Collision collision)
    {
        if (explosionEffect != null)
        {
            GameObject explosion = Instantiate(
                explosionEffect,
                transform.position,
                Quaternion.identity
            );

            Destroy(explosion, 3f);
        }

        Destroy(gameObject);
    }
}