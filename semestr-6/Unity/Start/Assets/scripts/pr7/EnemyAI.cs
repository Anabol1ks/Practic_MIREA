using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Transform player;
    public float chaseRadius = 10f;
    public float attackRadius = 2f;
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRadius)
        {
            // Атаковать игрока
            Attack();
        }
        else if (distanceToPlayer <= chaseRadius)
        {
            // Преследовать игрока
            agent.SetDestination(player.position);
        }
        else
        {
            // Вернуться к патрулированию
            Patrol();
        }
    }

    void Attack()
    {
        // Реализация атаки (например, отнимание здоровья)
        Debug.Log("Атакую игрока!");
    }

    void Patrol()
    {
        // Логика патрулирования (см. следующий раздел)
    }
}