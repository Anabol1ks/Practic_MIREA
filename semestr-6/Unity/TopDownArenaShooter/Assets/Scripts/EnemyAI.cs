using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;
    private Transform player;
    private EnemyHealth enemyHealth;
    private float nextAttackTime;
    private float attackCooldown = 1f;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        enemyHealth = GetComponent<EnemyHealth>();
        
        // Настраиваем скорость движения из типа врага
        agent.speed = enemyHealth.enemyType.moveSpeed;
    }

    void Update()
    {
        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            
            // Если враг дальнего боя и игрок в пределах дальности атаки
            if (enemyHealth.enemyType.isRanged && distanceToPlayer <= enemyHealth.enemyType.attackRange)
            {
                agent.isStopped = true;
                if (Time.time >= nextAttackTime)
                {
                    Attack();
                    nextAttackTime = Time.time + attackCooldown;
                }
            }
            // Если враг ближнего боя или игрок вне дальности атаки
            else
            {
                agent.isStopped = false;
                agent.SetDestination(player.position);
            }
        }
    }

    void Attack()
    {
        if (enemyHealth.enemyType.isRanged)
        {
            // TODO: Реализовать стрельбу для врагов дальнего боя
            Debug.Log("Ranged attack!");
        }
        else
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(enemyHealth.enemyType.damage);
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && !enemyHealth.enemyType.isRanged)
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(enemyHealth.enemyType.damage);
            }
        }
    }
}
