using UnityEngine;

public class ArmyHealth : MonoBehaviour
{
    public int health = 100;

    public void TakeDamage(int amount)
    {
        if (health <= 0 || amount <= 0)
        {
            return;
        }

        health = Mathf.Max(0, health - amount);

        if (health == 0)
        {
            Destroy(gameObject);
        }
    }
}