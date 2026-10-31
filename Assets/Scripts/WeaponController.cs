using System.Collections;
using UnityEngine;
using TMPro;

public class WeaponController : MonoBehaviour
{
    public int gunAmmo = 10;
    public int maxAmmo = 10;
    public float reloadTime = 2f;
    public float range = 100f;

    private bool isReloading;
    private int startingMaxAmmo = 10;
    private float startingReloadTime = 2f;

    public GameObject bulletPrefab;
    public float bulletSpeed = 15f;
    public float bulletLifetime = 5f;

    public TMP_Text ammoText;
    public TMP_Text maxAmmoText;
    public TMP_Dropdown maxAmmoDropdown;
    public TMP_Dropdown reloadTimeDropdown;

    void Start()
    {
        maxAmmoDropdown.onValueChanged.AddListener(SetMaximumAmmo);

        SetMaximumAmmo(maxAmmoDropdown.value);

        reloadTimeDropdown.onValueChanged.AddListener(SetReloadTime);
        SetReloadTime(reloadTimeDropdown.value);
    }

    void Update()
    {
        if (Time.timeScale == 0f)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(Reload());
        }
    }

    public void SetMaximumAmmo(int option)
    {
        if (option == 0)
        {
            maxAmmo = startingMaxAmmo * 2;
        }
        else if (option == 1)
        {
            maxAmmo = startingMaxAmmo * 5;
        }
        else
        {
            maxAmmo = startingMaxAmmo * 10;
        }

        gunAmmo = maxAmmo;

        UpdateAmmoText();
        UpdateMaxAmmoText();
    }

    public void SetReloadTime(int option)
    {
        if (option == 0)
        {
            reloadTime = startingReloadTime * 0.5f;
        }
        else if (option == 1)
        {
            reloadTime = startingReloadTime;
        }
        else
        {
            reloadTime = startingReloadTime * 2f;
        }

        Debug.Log("Reload Time: " + reloadTime + " seconds");
    }

    void Shoot()
    {
        if (isReloading || gunAmmo <= 0)
            return;

        gunAmmo--;
        LaunchVisualBullet();

        UpdateAmmoText();

        Ray ray = Camera.main.ViewportPointToRay(
            new Vector3(0.5f, 0.5f, 0f)
        );

        if (Physics.Raycast(ray, out RaycastHit hit, range))
        {
            Debug.Log("Hit: " + hit.collider.name);

            EnemyHealth enemy =
                hit.collider.GetComponentInParent<EnemyHealth>();

            if (enemy != null)
            {
                enemy.TakeDamage(10);
            }
        }

        Debug.Log("Ammo: " + gunAmmo);
    }

    void LaunchVisualBullet()
    {
        if (bulletPrefab == null || Camera.main == null)
            return;

        Camera playerCamera = Camera.main;

        Vector3 direction = playerCamera.transform.forward;

        Vector3 spawnPosition =
            playerCamera.transform.position +
            direction * 1f;

        GameObject visualBullet = Instantiate(
            bulletPrefab,
            spawnPosition,
            Quaternion.LookRotation(direction)
        );

        Rigidbody bulletRigidbody =
            visualBullet.GetComponent<Rigidbody>();

        if (bulletRigidbody != null)
        {
            bulletRigidbody.linearVelocity =
                direction * bulletSpeed;
        }

        Destroy(visualBullet, bulletLifetime);
    }

    IEnumerator Reload()
    {
        if (isReloading || gunAmmo == maxAmmo)
            yield break;

        isReloading = true;
        Debug.Log("Reloading...");

        yield return new WaitForSeconds(reloadTime);

        gunAmmo = maxAmmo;
        UpdateAmmoText();
        isReloading = false;

        Debug.Log("Reload complete. Ammo: " + gunAmmo);
    }

    public void AddAmmo(int amount)
    {
        gunAmmo += amount;

        if (gunAmmo > maxAmmo)
        {
            gunAmmo = maxAmmo;
        }

        UpdateAmmoText();
    }

    void UpdateAmmoText()
    {
        if (ammoText != null)
        {
            ammoText.text = "Ammo: " + gunAmmo;
        }
    }

    void UpdateMaxAmmoText()
    {
        if (maxAmmoText != null)
        {
            maxAmmoText.text = "Max Ammo: " + maxAmmo;
        }
    }
}