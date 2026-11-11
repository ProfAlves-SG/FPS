using System;
using UnityEngine;
using UnityEngine.AI;

public class EnemyClass : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform player;

    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform firePoint;
    
    [Header("Ranges")]
    [SerializeField] private float chaseRange = 15f;
    
    [SerializeField] private float attackRange = 10f;
    [SerializeField] private float timeBetweenAttacks = 1f;

    private float nextShootTime;

    private void Start()
    {
        if (player == null)
        {
            GameObject playerObject = GameObject.FindWithTag("Player");
            
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }
    }

    private void Update()
    {
        if (player == null) return;
        
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

    private void Attack()
    {
        agent.ResetPath();
        LookAtPlayer();

        if (Time.time >= nextShootTime)
        {
            Instantiate(bullet, firePoint.position, firePoint.rotation);
            nextShootTime = Time.time + timeBetweenAttacks;
        }
    }
    
    private void LookAtPlayer()
    {
        Vector3 target = player.position;
        target.y = transform.position.y;
        transform.LookAt(target);
    }
    
    private void Idle()
    {
        agent.ResetPath();
    }
    
    private void Chase()
    {
        agent.SetDestination(player.position);
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
    }
        
}
