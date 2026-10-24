using UnityEngine;

public class GrenadeLauncher : MonoBehaviour
{
    public GameObject grenadePrefab;
    public float grenadeSpeed = 4f;
    public float grenadeLifetime = 20f;

    void Update()
    {
        if (Time.timeScale == 0f)
            return;

        if (Input.GetKeyDown(KeyCode.G))
        {
            ThrowGrenade();
        }
    }

    void ThrowGrenade()
    {
        Camera playerCamera = Camera.main;

        Vector3 spawnPosition =
            playerCamera.transform.position +
            playerCamera.transform.forward * 1f;

        GameObject grenade = Instantiate(
            grenadePrefab,
            spawnPosition,
            Quaternion.identity
        );

        Rigidbody grenadeRigidbody =
            grenade.GetComponent<Rigidbody>();

        grenadeRigidbody.linearVelocity =
            playerCamera.transform.forward * grenadeSpeed;

        Destroy(grenade, grenadeLifetime);
    }
}