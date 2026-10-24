using UnityEngine;

public class LaunchProjectile : MonoBehaviour
{
    public GameObject projectile;
    public float projectileIntensity = 500f;
    public float spawnInterval = 3f;

    private float timer;

    void Start()
    {
        timer = 0f;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            Launch();
            timer = 0f;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Launch();
        }
    }

    void Launch()
    {
        GameObject newProjectile = Instantiate(
            projectile,
            transform.position + transform.forward * 1.5f,
            transform.rotation
        );

        newProjectile.GetComponent<Rigidbody>().AddForce(
            transform.forward * projectileIntensity
        );

        Destroy(newProjectile, 10f);
    }
}