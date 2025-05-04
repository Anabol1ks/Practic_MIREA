using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(EnemyHealth))]
public class EnemyAI : MonoBehaviour
{
    [Header("Movement")]
    public float normalSpeed = 3.5f;
    public float chaseSpeed = 5f;
    public float rotationSpeed = 5f;

    [Header("Attack")]
    public int damageAmount = 10;
    public float attackDistance = 1.5f;
    public float attackCooldown = 1f;
    public float attackDelay = 0.3f;
    public float sightRange = 15f;

    [Header("Effects")]
    public GameObject hitEffect;
    public AudioClip attackSound;

    private Transform player;
    private NavMeshAgent agent;
    private PlayerHealth playerHealth;
    private EnemyHealth enemyHealth;
    private Animator animator;
    private float lastAttackTime;
    private bool isAttacking;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        enemyHealth = GetComponent<EnemyHealth>();
        animator = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerHealth = player.GetComponent<PlayerHealth>();
        agent.speed = normalSpeed;
    }

    void Update()
    {
        if (enemyHealth.IsDead) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= sightRange)
        {
            agent.SetDestination(player.position);

            if (distanceToPlayer <= attackDistance)
            {
                FacePlayer();
                TryAttack();
            }
            else
            {
                agent.speed = chaseSpeed;
            }
        }
        else
        {
            agent.speed = normalSpeed;
        }

        UpdateAnimations();
    }

    void FacePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    void TryAttack()
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            isAttacking = true;
            lastAttackTime = Time.time;
            Invoke("PerformAttack", attackDelay);
        }
    }

    void PerformAttack()
    {
        if (!isAttacking) return;
        
        if (Vector3.Distance(transform.position, player.position) <= attackDistance * 1.2f)
        {
            playerHealth.TakeDamage(damageAmount);
            if (hitEffect) Instantiate(hitEffect, player.position, Quaternion.identity);
            if (attackSound) AudioSource.PlayClipAtPoint(attackSound, transform.position);
        }
        isAttacking = false;
    }

    void UpdateAnimations()
    {
        if (animator)
        {
            animator.SetFloat("Speed", agent.velocity.magnitude);
            animator.SetBool("IsAttacking", isAttacking);
        }
    }

    public void OnDeath()
    {
        agent.enabled = false;
        enabled = false;
        if (animator) animator.SetTrigger("Die");
    }
}