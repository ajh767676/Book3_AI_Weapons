using UnityEngine;

public class ArmyHealth : MonoBehaviour
{
    public int health = 100;

    public void TakeDamage(int amount, GameObject attacker = null)
    {
        if (health <= 0 || amount <= 0)
        {
            return;
        }

        health = Mathf.Max(0, health - amount);

        if (health == 0)
        {
            Destroy(gameObject);
            return;
        }

        TeamMember member = GetComponent<TeamMember>();

        if (member != null && attacker != null)
        {
            member.RespondToAttack(attacker);
        }
    }
}