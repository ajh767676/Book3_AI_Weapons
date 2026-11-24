using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class TeamMember : MonoBehaviour
{
    public Transform leader;
    public float followDistance = 5f;
    public float attackDistance = 2f;
    public int attackDamage = 20;
    public float attackInterval = 1f;

    private NavMeshAgent agent;
    private Animator animator;
    private GameObject target;
    private float attackTimer;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    public void Attack(GameObject newTarget)
    {
        if (newTarget == null)
        {
            return;
        }

        target = newTarget;
        attackTimer = 0f;

        animator.SetBool("targetDestroyed", false);
        animator.SetBool("closeToTarget", false);
        animator.ResetTrigger("retreat");
        animator.SetTrigger("attackOneToOne");
    }

    public void Retreat()
    {
        target = null;
        attackTimer = 0f;

        animator.ResetTrigger("attackOneToOne");
        animator.SetBool("closeToTarget", false);
        animator.SetBool("targetDestroyed", false);
        animator.SetTrigger("retreat");
    }

    void Update()
    {
        if (Time.timeScale == 0f || !agent.isOnNavMesh)
        {
            return;
        }

        bool closeToLeader = leader == null ||
            Vector3.Distance(transform.position,
                leader.position) <= followDistance;

        animator.SetBool("closeToLeader", closeToLeader);

        AnimatorStateInfo state =
            animator.GetCurrentAnimatorStateInfo(0);

        bool approaching = state.IsName("goToTarget");
        bool attacking = state.IsName("attackTarget");

        if (approaching || attacking)
        {
            UpdateAttack(attacking);
            return;
        }

        attackTimer = 0f;

        if (state.IsName("moveTowardsLeader") &&
            !closeToLeader && leader != null)
        {
            agent.isStopped = false;
            agent.SetDestination(leader.position);
        }
        else
        {
            agent.isStopped = true;
        }
    }

    void UpdateAttack(bool attacking)
    {
        ArmyHealth targetHealth = target == null
            ? null : target.GetComponent<ArmyHealth>();

        if (targetHealth == null || targetHealth.health <= 0)
        {
            animator.SetBool("targetDestroyed", true);
            agent.isStopped = true;
            attackTimer = 0f;
            return;
        }

        float distance = Vector3.Distance(
            transform.position, target.transform.position);

        bool closeToTarget = distance <= attackDistance;
        animator.SetBool("closeToTarget", closeToTarget);

        if (!closeToTarget)
        {
            agent.isStopped = false;
            agent.SetDestination(target.transform.position);
            attackTimer = 0f;
            return;
        }

        agent.isStopped = true;

        Vector3 direction =
            target.transform.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }

        if (attacking && !animator.IsInTransition(0))
        {
            attackTimer += Time.deltaTime;

            if (attackTimer >= attackInterval)
            {
                attackTimer = 0f;
                targetHealth.TakeDamage(attackDamage);
            }
        }
    }
}