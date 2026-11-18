using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class SoldierSenses : MonoBehaviour
{
    public Transform player;
    public float hearingDistance = 3f;
    public float sightDistance = 10f;
    public float fieldOfView = 90f;

    public Transform[] waypoints;

    private int currentPoint;
    private Vector3 startingPosition;
    private bool wasPatrolling;

    private NavMeshAgent agent;
    private Animator animator;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        startingPosition = transform.position;
    }
    

    void Update()
    {
        if (Time.timeScale == 0f || !agent.isOnNavMesh)
            return;

        if (player == null)
            return;

        float distance = Vector3.Distance(
            transform.position, player.position);

        animator.SetBool(
            "canHearPlayer", distance <= hearingDistance);

        Vector3 directionToPlayer =
            player.position - transform.position;

        float angleToPlayer = Vector3.Angle(
            transform.forward, directionToPlayer);

        bool canSeePlayer =
            distance <= sightDistance &&
            angleToPlayer <= fieldOfView / 2f;

        if (canSeePlayer)
        {
            Vector3 eyePosition =
                transform.position + Vector3.up * 1.5f;

            Vector3 playerPosition =
                player.position + Vector3.up;

            if (Physics.Linecast(
                eyePosition, playerPosition, out RaycastHit hit,
                Physics.DefaultRaycastLayers,
                QueryTriggerInteraction.Ignore))
            {
                canSeePlayer =
                    hit.transform == player ||
                    hit.transform.IsChildOf(player);
            }
        }

        animator.SetBool("canSeePlayer", canSeePlayer);

        AnimatorStateInfo state =
    animator.GetCurrentAnimatorStateInfo(0);

        bool isPatrolling = state.IsName("patrol");
        animator.SetBool("hasReachedStart", false);

        if (state.IsName("followPlayer"))
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
        }
        else if (isPatrolling)
        {
            if (!wasPatrolling)
            {
                currentPoint = 0;
                agent.ResetPath();
                animator.ResetTrigger("startToGoBackToStart");
            }

            if (waypoints.Length > 0 &&
                currentPoint < waypoints.Length &&
                waypoints[currentPoint] != null)
            {
                agent.isStopped = false;

                if (!agent.hasPath && !agent.pathPending)
                {
                    agent.SetDestination(
                        waypoints[currentPoint].position);
                }
                else if (!agent.pathPending &&
                         agent.pathStatus == NavMeshPathStatus.PathComplete &&
                         agent.remainingDistance <=
                         agent.stoppingDistance + 0.1f)
                {
                    currentPoint++;
                    agent.ResetPath();

                    if (currentPoint >= waypoints.Length)
                    {
                        agent.isStopped = true;
                        animator.SetTrigger("startToGoBackToStart");
                    }
                }
            }
            else
            {
                agent.isStopped = true;
                animator.SetTrigger("startToGoBackToStart");
            }
        }
        else if (state.IsName("backToStart"))
        {
            agent.isStopped = false;
            agent.SetDestination(startingPosition);

            if (Vector3.Distance(
                transform.position, startingPosition) <=
                agent.stoppingDistance + 0.1f)
            {
                agent.isStopped = true;
                animator.SetBool("hasReachedStart", true);
            }
        }
        else
        {
            agent.isStopped = true;
        }

        wasPatrolling = isPatrolling;
    }
}