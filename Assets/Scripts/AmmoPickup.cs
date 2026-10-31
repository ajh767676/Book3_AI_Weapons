using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    public int ammoAmount = 10;

    void OnTriggerEnter(Collider other)
    {
        WeaponController weapon =
            other.GetComponentInChildren<WeaponController>();

        if (weapon == null)
        {
            weapon = other.GetComponentInParent<WeaponController>();
        }

        if (weapon != null)
        {
            weapon.AddAmmo(ammoAmount);
            Destroy(gameObject);
        }
    }
}