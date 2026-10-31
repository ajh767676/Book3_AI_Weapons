using UnityEngine;

public class ControlNPCFSM : MonoBehaviour
{
    private Animator anim;
    private GameObject player;
    private PlayerHealth playerHealth;

    public float sightDistance = 10f;
    public float followSpeed = 0.5f;
    public float stopDistance = 3f;

    public int attackDamage = 10;
    public float attackInterval = 1f;

    private float attackTimer = 0f;

    void Start()
    {
        anim = GetComponent<Animator>();
        player = GameObject.Find("FirstPersonController");

        if (player != null)
        {
            playerHealth = player.GetComponent<PlayerHealth>();
        }
    }

    void Update()
    {
        if (player == null || Time.timeScale == 0f)
        {
            return;
        }

        attackTimer -= Time.deltaTime;

        float distance = Vector3.Distance(
            transform.position,
            player.transform.position
        );

        bool canSeePlayer = distance <= sightDistance;

        if (anim != null)
        {
            anim.SetBool("canSeePlayer", canSeePlayer);
        }

        if (canSeePlayer)
        {
            Vector3 targetPosition = player.transform.position;
            targetPosition.y = transform.position.y;

            if (targetPosition != transform.position)
            {
                transform.LookAt(targetPosition);
            }

            if (distance > stopDistance)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    targetPosition,
                    followSpeed * Time.deltaTime
                );
            }
            else if (playerHealth != null && attackTimer <= 0f)
            {
                attackTimer = attackInterval;
                playerHealth.TakeDamage(attackDamage);
            }
        }
    }
}