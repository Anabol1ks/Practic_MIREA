using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("Основные настройки")]
    public Transform player;
    public NavMeshAgent agent;
    public float chaseRadius = 10f;
    public float attackRadius = 2f;
    public float attackCooldown = 1.5f; // КД между атаками
    public float attackDamage = 15f;

    [Header("Скрытное поведение")]
    public float stealthRadius = 15f;
    public LayerMask obstacleLayers;

    [Header("Патрулирование")]
    public Transform[] patrolPoints;
    public float patrolSpeed = 3f;
    public float waitTimeAtPoint = 2f;
    public float pointReachThreshold = 0.5f;

    private int currentPatrolIndex;
    private float lastAttackTime;
    private float waitTimer;
    private bool isPlayerVisible;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        currentPatrolIndex = 0;
        agent.speed = patrolSpeed;
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        isPlayerVisible = IsPlayerInSight();

        // Приоритет действий: Атака -> Преследование -> Скрытное поведение -> Патрулирование
        if (distanceToPlayer <= attackRadius && isPlayerVisible)
        {
            Attack();
        }
        else if (distanceToPlayer <= chaseRadius && isPlayerVisible)
        {
            ChasePlayer();
        }
        else if (distanceToPlayer <= stealthRadius)
        {
            StealthBehavior();
        }
        else
        {
            Patrol();
        }
    }

    bool IsPlayerInSight()
    {
        Vector3 direction = player.position - transform.position;
        if (Physics.Raycast(
            transform.position, 
            direction, 
            out RaycastHit hit,
            stealthRadius,
            ~obstacleLayers))
        {
            return hit.transform == player;
        }
        return false;
    }

    void Attack()
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
            player.GetComponent<PlayerHealth>().TakeDamage((int)attackDamage); // Явное приведение к int
            lastAttackTime = Time.time;
        }
        agent.ResetPath();
    }

    void ChasePlayer()
    {
        agent.speed = patrolSpeed * 1.5f; // Ускорение при преследовании
        agent.SetDestination(player.position);
    }

    void StealthBehavior()
    {
        if (isPlayerVisible)
        {
            MoveToCover();
        }
        else
        {
            ChasePlayer(); // Продолжить преследование, если игрок был замечен
        }
    }

    void MoveToCover()
    {
        // Улучшенный поиск укрытия
        if (NavMesh.FindClosestEdge(transform.position, out NavMeshHit hit, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
        else
        {
            // Резервный вариант: случайная точка
            Vector3 randomPoint = transform.position + Random.insideUnitSphere * 10f;
            if (NavMesh.SamplePosition(randomPoint, out hit, 10f, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }
        }
    }

    void Patrol()
    {
        agent.speed = patrolSpeed;
        
        if (patrolPoints.Length == 0)
        {
            RandomPatrol();
            return;
        }

        // Движение по заданным точкам
        if (agent.remainingDistance <= pointReachThreshold)
        {
            waitTimer += Time.deltaTime;
            
            if (waitTimer >= waitTimeAtPoint)
            {
                currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
                agent.SetDestination(patrolPoints[currentPatrolIndex].position);
                waitTimer = 0f;
            }
        }
    }

    void RandomPatrol()
    {
        if (agent.remainingDistance <= pointReachThreshold)
        {
            waitTimer += Time.deltaTime;
            
            if (waitTimer >= waitTimeAtPoint)
            {
                Vector3 randomPoint = transform.position + Random.insideUnitSphere * 15f;
                if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 15f, NavMesh.AllAreas))
                {
                    agent.SetDestination(hit.position);
                }
                waitTimer = 0f;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        // Визуализация радиусов
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, stealthRadius);
    }
}