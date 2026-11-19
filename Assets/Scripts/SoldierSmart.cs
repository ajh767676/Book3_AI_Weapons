using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class SoldierSmart : MonoBehaviour
{
    public Transform player;
    public float hearingDistance = 3f;
    public float sightDistance = 10f;
    public float fieldOfView = 90f;
    public float shootingDistance = 5f;

    public int shootingDamage = 10;
    public float shootingInterval = 1f;

    public Transform[] waypoints;

    public Transform healthPickup;
    public Transform ammoPickup;
    public GameObject grenadePrefab;
    public float grenadeFlightTime = 2f;
    public Transform ambushSpot;
    public BoxCollider ambushArea;

    public int ammo = 100;
    public float health = 100f;
    public float healthDrainRate = 1f;

    private float nextShotTime;
    private int currentPoint;
    private Vector3 startingPosition;
    private bool wasPatrolling;
    private bool isDead;
    private bool wasInsideAmbushArea;
    private bool ambushInProgress;
    private bool wasInAmbushState;

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
        if (Time.timeScale == 0f || isDead ||
            !agent.isOnNavMesh)
        {
            return;
        }

        if (player == null)
            return;

        health = Mathf.Max(
            0f, health - healthDrainRate * Time.deltaTime);

        animator.SetFloat("health", health);
        animator.SetFloat("ammo", ammo);

        // Check pickup availability every frame.
        bool healthPackAvailable =
            healthPickup != null &&
            healthPickup.gameObject.activeInHierarchy;

        animator.SetBool(
            "healthPackAvailable", healthPackAvailable);

        if (health < 1f)
        {
            isDead = true;
            agent.isStopped = true;
            agent.ResetPath();

            animator.SetBool("canHearPlayer", false);
            animator.SetBool("canSeePlayer", false);
            animator.SetBool("canShootPlayer", false);

            animator.Play("die", 0, 0f);
            Destroy(gameObject, 4f);
            return;
        }

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
                eyePosition,
                playerPosition,
                out RaycastHit hit,
                Physics.DefaultRaycastLayers,
                QueryTriggerInteraction.Ignore))
            {
                canSeePlayer =
                    hit.transform == player ||
                    hit.transform.IsChildOf(player);
            }
        }

        animator.SetBool("canSeePlayer", canSeePlayer);

        bool canShootPlayer =
            canSeePlayer &&
            distance <= shootingDistance &&
            health > 20f &&
            ammo > 20;

        animator.SetBool("canShootPlayer", canShootPlayer);

        AnimatorStateInfo state =
            animator.GetCurrentAnimatorStateInfo(0);
        bool inAmbushState =
    state.IsName("goToAmbushSpot") ||
    state.IsName("throwGrenade");

        if (wasInAmbushState && !inAmbushState)
        {
            ambushInProgress = false;
        }

        bool insideAmbushArea =
            ambushArea != null &&
            ambushArea.enabled &&
            ambushArea.gameObject.activeInHierarchy &&
            ambushArea.bounds.Contains(player.position);

        if (insideAmbushArea &&
            !wasInsideAmbushArea &&
            !ambushInProgress &&
            ambushSpot != null &&
            health > 20f &&
            ammo > 20)
        {
            ambushInProgress = true;

            animator.SetBool("hasReachedAmbushSpot", false);
            animator.SetBool("hasThrownGrenade", false);
            animator.SetTrigger("startAmbush");
        }

        animator.SetBool("isAmbushing", ambushInProgress);

        wasInsideAmbushArea = insideAmbushArea;
        wasInAmbushState = inAmbushState;

        bool isPatrolling = state.IsName("patrol");

        animator.SetBool("hasReachedStart", false);

        if (state.IsName("goToAmbushSpot"))
        {
            if (ambushSpot != null)
            {
                agent.isStopped = false;
                agent.SetDestination(ambushSpot.position);

                Vector3 offset =
                    ambushSpot.position - transform.position;
                offset.y = 0f;

                if (!agent.pathPending &&
                    offset.magnitude <=
                    agent.stoppingDistance + 0.2f)
                {
                    agent.isStopped = true;
                    animator.SetBool("hasReachedAmbushSpot", true);
                }
            }
            else
            {
                agent.isStopped = true;
            }
        }
        else if (state.IsName("throwGrenade"))
        {
            agent.isStopped = true;

            if (!animator.GetBool("hasThrownGrenade") &&
                grenadePrefab != null)
            {
                Vector3 direction = player.position - transform.position;
                direction.y = 0f;

                if (direction.sqrMagnitude < 0.001f)
                    direction = transform.forward;

                Vector3 launchPosition =
                    transform.position +
                    Vector3.up * 1.5f +
                    direction.normalized * 0.8f;

                GameObject grenade = Instantiate(
                    grenadePrefab,
                    launchPosition,
                    Quaternion.identity);

                Rigidbody grenadeBody = grenade.GetComponent<Rigidbody>();

                if (grenadeBody != null)
                {
                    float flightTime = Mathf.Max(0.5f, grenadeFlightTime);

                    Vector3 velocity =
                        (player.position - launchPosition) / flightTime -
                        Physics.gravity * flightTime * 0.5f;

                    grenadeBody.AddForce(
                        velocity,
                        ForceMode.VelocityChange);
                }

                Destroy(grenade, 10f);
                animator.SetBool("hasThrownGrenade", true);
            }
        }
        else if (state.IsName("flee"))
        {
            Vector3 awayFromPlayer =
                transform.position - player.position;

            awayFromPlayer.y = 0f;

            if (awayFromPlayer.sqrMagnitude < 0.001f)
                awayFromPlayer = -transform.forward;

            Vector3 fleeTarget =
                transform.position +
                awayFromPlayer.normalized * 25f;

            if (NavMesh.SamplePosition(
                fleeTarget,
                out NavMeshHit fleeHit,
                10f,
                agent.areaMask))
            {
                agent.isStopped = false;
                agent.SetDestination(fleeHit.position);
            }
        }
        else if (state.IsName("lookForHealth"))
        {
            if (healthPackAvailable)
            {
                agent.isStopped = false;
                agent.SetDestination(healthPickup.position);

                if (Vector3.Distance(
                    transform.position,
                    healthPickup.position) <= 2f)
                {
                    health = 100f;
                    animator.SetFloat("health", health);

                    healthPickup.gameObject.SetActive(false);
                    animator.SetBool(
                        "healthPackAvailable", false);

                    agent.ResetPath();
                }
            }
            else
            {
                agent.isStopped = true;
            }
        }
        else if (state.IsName("lookForAmmo"))
        {
            if (ammoPickup != null &&
                ammoPickup.gameObject.activeInHierarchy)
            {
                agent.isStopped = false;
                agent.SetDestination(ammoPickup.position);

                if (Vector3.Distance(
                    transform.position,
                    ammoPickup.position) <= 2f)
                {
                    ammo = 100;
                    animator.SetFloat("ammo", ammo);
                    agent.ResetPath();
                }
            }
            else
            {
                agent.isStopped = true;
            }
        }
        else if (state.IsName("shootPlayer"))
        {
            agent.isStopped = true;

            if (canShootPlayer &&
                Time.time >= nextShotTime)
            {
                PlayerHealth playerHealth =
                    player.GetComponentInParent<PlayerHealth>();

                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(shootingDamage);

                    ammo = Mathf.Max(0, ammo - 1);
                    animator.SetFloat("ammo", ammo);
                }

                nextShotTime =
                    Time.time + shootingInterval;
            }

            Vector3 lookDirection =
                player.position - transform.position;

            lookDirection.y = 0f;

            if (lookDirection.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation =
                    Quaternion.LookRotation(lookDirection);

                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    targetRotation,
                    agent.angularSpeed * Time.deltaTime);
            }
        }
        else if (state.IsName("followPlayer"))
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

                animator.ResetTrigger(
                    "startToGoBackToStart");
            }

            if (waypoints != null &&
                waypoints.Length > 0 &&
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
                         agent.pathStatus ==
                             NavMeshPathStatus.PathComplete &&
                         agent.remainingDistance <=
                             agent.stoppingDistance + 0.1f)
                {
                    currentPoint++;
                    agent.ResetPath();

                    if (currentPoint >= waypoints.Length)
                    {
                        agent.isStopped = true;

                        animator.SetTrigger(
                            "startToGoBackToStart");
                    }
                }
            }
            else
            {
                agent.isStopped = true;

                animator.SetTrigger(
                    "startToGoBackToStart");
            }
        }
        else if (state.IsName("backToStart"))
        {
            agent.isStopped = false;
            agent.SetDestination(startingPosition);

            if (Vector3.Distance(
                transform.position,
                startingPosition) <=
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