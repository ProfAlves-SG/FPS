using System;
using UnityEngine;
using UnityEngine.AI;

// Cópia de Assets/Scripts/EnemyClass.cs com as linhas de animação adicionadas.
// Toda linha nova/alterada em relação ao original está marcada com "// [ANIM]".
// O script só seta as bools; quem decide qual clipe toca é o EnemyAnimator.controller.
public class EnemyClassAnimated : MonoBehaviour
{
    // [ANIM] nomes dos parâmetros bool do EnemyAnimator.controller
    private const string IsChasingParam = "isChasing";  // [ANIM]
    private const string IsShootingParam = "isShooting"; // [ANIM]

    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator animator; // [ANIM] Animator que está no root do soldado
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
        if (animator == null) // [ANIM] fallback caso o campo não esteja ligado no prefab
        {
            animator = GetComponent<Animator>(); // [ANIM]
        }

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

        SetAnimation(false, true); // [ANIM] parado + atirando -> estado "Shoot"

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

        SetAnimation(false, false); // [ANIM] nada acontecendo -> estado "Idle"
    }

    private void Chase()
    {
        agent.SetDestination(player.position);

        SetAnimation(true, false); // [ANIM] perseguindo -> estado "Chase"
    }

    // [ANIM] único ponto que fala com o Animator: seta as duas bools e pronto.
    private void SetAnimation(bool chasing, bool shooting) // [ANIM]
    {
        if (animator == null) return; // [ANIM]

        animator.SetBool(IsChasingParam, chasing);   // [ANIM]
        animator.SetBool(IsShootingParam, shooting); // [ANIM]
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
    }
}
