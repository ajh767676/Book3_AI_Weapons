using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class SoldierPatrol : MonoBehaviour
{
    public Transform[] waypoints;

    private NavMeshAgent agent;
    private int currentPoint;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        GoToCurrentPoint();
    }

    void Update()
    {
        if (Time.timeScale == 0f || !agent.isOnNavMesh)
            return;

        if (waypoints.Length == 0 || agent.pathPending)
            return;

        if (agent.hasPath &&
            agent.pathStatus == NavMeshPathStatus.PathComplete &&
            agent.remainingDistance <= agent.stoppingDistance + 0.1f)
        {
            currentPoint = (currentPoint + 1) % waypoints.Length;
            GoToCurrentPoint();
        }
    }

    void GoToCurrentPoint()
    {
        if (waypoints.Length == 0 || !agent.isOnNavMesh)
            return;

        if (waypoints[currentPoint] != null)
            agent.SetDestination(waypoints[currentPoint].position);
    }
}
