using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;
    private Transform player;
    private Transform playerBase;
    private EnemyHealth enemyHealth;
    private float nextAttackTime;
    private float attackCooldown = 1f;

    [Header("Приоритеты")]
    public float baseAttackPriority = 0.7f; // 70% приоритет на атаку базы
    public float detectionRadius = 15f; // Радиус в котором враг видит цели

    private bool isAttackingBase = false;
    private BaseHealth baseHealth;
    private PlayerHealth playerHealth;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerHealth = player.GetComponent<PlayerHealth>();
        
        GameObject baseObj = GameObject.FindGameObjectWithTag("PlayerBase");
        if (baseObj != null)
        {
            playerBase = baseObj.transform;
            baseHealth = baseObj.GetComponent<BaseHealth>();
        }
        
        agent = GetComponent<NavMeshAgent>();
        enemyHealth = GetComponent<EnemyHealth>();
        
        // Настраиваем скорость движения из типа врага
        agent.speed = enemyHealth.enemyType.moveSpeed;
    }

    void Update()
    {
        if (playerBase == null && player == null) return;
        
        // Если база уничтожена - атакуем только игрока
        if (playerBase == null || baseHealth == null || baseHealth.currentHealth <= 0)
        {
            if (player != null)
                ChaseAndAttackPlayer();
            return;
        }
        
        // Оцениваем расстояния до целей
        float distanceToPlayer = player != null ? Vector3.Distance(transform.position, player.position) : float.MaxValue;
        float distanceToBase = Vector3.Distance(transform.position, playerBase.position);
        
        // Если мы в пределах радиуса обнаружения
        if (distanceToPlayer <= detectionRadius || distanceToBase <= detectionRadius)
        {
            // Выбираем цель атаки на основе приоритетов
            if (Random.value <= baseAttackPriority)
            {
                isAttackingBase = true;
                ChaseAndAttackBase();
            }
            else
            {
                isAttackingBase = false;
                ChaseAndAttackPlayer();
            }
        }
        else
        {
            // Если далеко от обеих целей, выбираем ближайшую
            if (distanceToBase < distanceToPlayer)
            {
                isAttackingBase = true;
                ChaseAndAttackBase();
            }
            else
            {
                isAttackingBase = false;
                ChaseAndAttackPlayer();
            }
        }
    }

    void ChaseAndAttackPlayer()
    {
        if (player == null) return;
        
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        
        // Если враг дальнего боя и игрок в пределах дальности атаки
        if (enemyHealth.enemyType.isRanged && distanceToPlayer <= enemyHealth.enemyType.attackRange)
        {
            agent.isStopped = true;
            if (Time.time >= nextAttackTime)
            {
                AttackPlayer();
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

    void ChaseAndAttackBase()
    {
        if (playerBase == null) return;
        
        float distanceToBase = Vector3.Distance(transform.position, playerBase.position);
        
        // Если враг дальнего боя и база в пределах дальности атаки
        if (enemyHealth.enemyType.isRanged && distanceToBase <= enemyHealth.enemyType.attackRange)
        {
            agent.isStopped = true;
            if (Time.time >= nextAttackTime)
            {
                AttackBase();
                nextAttackTime = Time.time + attackCooldown;
            }
        }
        // Если враг ближнего боя или база вне дальности атаки
        else
        {
            agent.isStopped = false;
            agent.SetDestination(playerBase.position);
        }
    }

    void AttackPlayer()
    {
        if (playerHealth != null)
        {
            if (enemyHealth.enemyType.isRanged)
            {
                // TODO: Реализовать стрельбу для врагов дальнего боя
                Debug.Log("Дальняя атака по игроку!");
                playerHealth.TakeDamage(enemyHealth.enemyType.damage);
            }
            else
            {
                playerHealth.TakeDamage(enemyHealth.enemyType.damage);
            }
        }
    }

    void AttackBase()
    {
        if (baseHealth != null)
        {
            if (enemyHealth.enemyType.isRanged)
            {
                // Дальняя атака базы
                Debug.Log("Дальняя атака по базе!");
                baseHealth.TakeDamage(enemyHealth.enemyType.damage);
            }
            else
            {
                baseHealth.TakeDamage(enemyHealth.enemyType.damage);
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
        else if (collision.gameObject.CompareTag("PlayerBase") && !enemyHealth.enemyType.isRanged)
        {
            BaseHealth baseHealth = collision.gameObject.GetComponent<BaseHealth>();
            if (baseHealth != null)
            {
                baseHealth.TakeDamage(enemyHealth.enemyType.damage);
            }
        }
    }
}
