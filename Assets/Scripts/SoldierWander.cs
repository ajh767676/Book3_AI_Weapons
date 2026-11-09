using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class SoldierWander : MonoBehaviour
{
    public float wanderRadius = 15f;
    public float changeInterval = 4f;

    private NavMeshAgent agent;
    private float timer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        ChooseDestination();
    }

    void Update()
    {
        if (Time.timeScale == 0f || !agent.isOnNavMesh)
            return;

        timer += Time.deltaTime;

        bool arrived = !agent.pathPending &&
                       agent.hasPath &&
                       agent.remainingDistance <=
                       agent.stoppingDistance + 0.1f;

        if (timer >= changeInterval || arrived)
            ChooseDestination();
    }

    void ChooseDestination()
    {
        timer = 0f;

        if (!agent.isOnNavMesh)
            return;

        // Try a few nearby locations on the NavMesh.
        for (int attempt = 0; attempt < 10; attempt++)
        {
            Vector2 offset = Random.insideUnitCircle * wanderRadius;

            Vector3 candidate = transform.position +
                new Vector3(offset.x, 0f, offset.y);

            if (!NavMesh.SamplePosition(candidate, out NavMeshHit hit,
                                        2f, agent.areaMask))
                continue;

            if (Vector3.Distance(transform.position, hit.position) < 2f)
                continue;

            NavMeshPath path = new NavMeshPath();

            if (agent.CalculatePath(hit.position, path) &&
                path.status == NavMeshPathStatus.PathComplete)
            {
                agent.SetPath(path);
                return;
            }
        }
    }
}
