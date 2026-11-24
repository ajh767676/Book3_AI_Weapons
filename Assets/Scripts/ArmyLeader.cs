using UnityEngine;

public class ArmyLeader : MonoBehaviour
{
    void Update()
    {
        if (Time.timeScale == 0f)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            Attack();
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            Retreat();
        }
    }

    public void Attack()
    {
        GameObject[] soldiers =
            GameObject.FindGameObjectsWithTag("teamMember");

        GameObject[] targets =
            GameObject.FindGameObjectsWithTag("armyTarget");

        System.Array.Sort(soldiers,
            (a, b) => a.name.CompareTo(b.name));

        System.Array.Sort(targets,
            (a, b) => a.name.CompareTo(b.name));

        int count = Mathf.Min(soldiers.Length, targets.Length);

        for (int i = 0; i < count; i++)
        {
            TeamMember member =
                soldiers[i].GetComponent<TeamMember>();

            if (member != null)
            {
                member.Attack(targets[i]);
            }
        }
    }

    public void Retreat()
    {
        GameObject[] soldiers =
            GameObject.FindGameObjectsWithTag("teamMember");

        foreach (GameObject soldier in soldiers)
        {
            TeamMember member =
                soldier.GetComponent<TeamMember>();

            if (member != null)
            {
                member.Retreat();
            }
        }
    }
}