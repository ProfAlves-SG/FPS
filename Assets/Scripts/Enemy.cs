using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform player;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;

    [Header("Ranges")]
    [SerializeField] private float chaseRange = 15f;
    [SerializeField] private float attackRange = 8f;

    [Header("Attack")]
    [SerializeField] private float timeBetweenShots = 1f;

    private float nextShotTime;

    private void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= attackRange)
        {
            Attack();
        }
        else if (distance <= chaseRange)
        {
            Chase();
        }
        else
        {
            Idle();
        }
    }

    private void Idle()
    {
        agent.ResetPath();
    }

    private void Chase()
    {
        agent.SetDestination(player.position);
    }

    private void Attack()
    {
        agent.ResetPath();
        LookAtPlayer();

        if (Time.time >= nextShotTime)
        {
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            nextShotTime = Time.time + timeBetweenShots;
        }
    }

    private void LookAtPlayer()
    {
        Vector3 target = player.position;
        target.y = transform.position.y;
        transform.LookAt(target);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
    }
    
}