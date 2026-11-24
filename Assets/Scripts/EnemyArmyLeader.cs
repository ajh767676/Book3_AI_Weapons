using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class EnemyArmyLeader : MonoBehaviour
{
    public Transform player;
    public Transform[] waypoints;
    public float patrolDelay = 5f;
    public float detectionDistance = 10f;

    private NavMeshAgent agent;
    private Animator animator;
    private int waypointIndex;
    private float timer;
    private bool patrolStarted;
    private bool attackOrdered;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Time.timeScale == 0f || !agent.isOnNavMesh)
        {
            return;
        }

        AnimatorStateInfo state =
            animator.GetCurrentAnimatorStateInfo(0);

        if (state.IsName("idle"))
        {
            agent.isStopped = true;
            timer += Time.deltaTime;

            if (!patrolStarted && timer >= patrolDelay)
            {
                patrolStarted = true;
                animator.SetTrigger("startPatrolling");
            }
        }
        else if (state.IsName("patrol"))
        {
            Patrol();

            if (player != null &&
                Vector3.Distance(transform.position,
                    player.position) <= detectionDistance)
            {
                agent.isStopped = true;
                animator.SetTrigger("closeToEnemy");
            }
        }
        else if (state.IsName("attack"))
        {
            agent.isStopped = true;

            if (!attackOrdered)
            {
                attackOrdered = true;
                OrderAttack();
            }
        }
    }

    void Patrol()
    {
        if (waypoints == null || waypoints.Length == 0)
        {
            agent.isStopped = true;
            return;
        }

        Transform waypoint = waypoints[waypointIndex];

        if (waypoint == null)
        {
            waypointIndex =
                (waypointIndex + 1) % waypoints.Length;
            return;
        }

        agent.isStopped = false;
        agent.SetDestination(waypoint.position);

        if (!agent.pathPending &&
            agent.remainingDistance <= 1f)
        {
            waypointIndex =
                (waypointIndex + 1) % waypoints.Length;
        }
    }

    public void OrderAttack()
    {
        GameObject[] soldiers =
            GameObject.FindGameObjectsWithTag("team2");

        GameObject[] targets =
            GameObject.FindGameObjectsWithTag("teamMember");

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
}