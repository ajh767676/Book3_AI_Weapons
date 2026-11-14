using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class SoldierPatrol : MonoBehaviour
{
    public Transform[] waypoints;
    public bool randomPatrol;

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
            ChooseNextPoint();
            GoToCurrentPoint();
        }
    }

    void ChooseNextPoint()
    {
        if (randomPatrol && waypoints.Length > 1)
        {
            // Choose another point without repeating the current one.
            int nextPoint = Random.Range(0, waypoints.Length - 1);

            if (nextPoint >= currentPoint)
                nextPoint++;

            currentPoint = nextPoint;
        }
        else
        {
            currentPoint = (currentPoint + 1) % waypoints.Length;
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