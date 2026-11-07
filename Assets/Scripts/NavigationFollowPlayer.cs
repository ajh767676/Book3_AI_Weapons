using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NavigationFollowPlayer : MonoBehaviour
{
    public Transform target;

    private NavMeshAgent agent;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (!agent.isOnNavMesh)
            return;

        agent.isStopped = Time.timeScale == 0f;

        if (agent.isStopped || target == null)
            return;

        agent.SetDestination(target.position);
    }
}