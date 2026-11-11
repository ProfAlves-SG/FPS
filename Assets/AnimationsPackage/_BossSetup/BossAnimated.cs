using System.Collections;
using UnityEngine;

// Cópia de Assets/Scripts/Boss.cs com as linhas de animação adicionadas.
// Toda linha nova/alterada está marcada com "// [ANIM]" ou "// [FIX]".
// O script só dispara os triggers e seta a bool; quem decide qual clipe toca é
// o BossAnimator.controller.
public class BossAnimated : MonoBehaviour
{
    // [ANIM] nomes dos parâmetros do BossAnimator.controller
    private const string SpawnParam = "spawn";             // [ANIM] Trigger
    private const string PrepareJumpParam = "prepareJump"; // [ANIM] Trigger
    private const string AirborneParam = "isAirborne";     // [ANIM] Bool

    [Header("References")]
    [SerializeField] private Animator animator; // [ANIM] Animator no root do boss
    [SerializeField] private Transform player;
    [SerializeField] private float startBattleDistance = 25f;

    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float spawnInterval = 2f;

    [SerializeField] private float timeBeforeJump = 8f;

    [Header("Jump Settings")]
    [SerializeField] private float prepareJumpTime = 0.5f;
    [SerializeField] private float jumpHeight = 5f;
    [SerializeField] private float jumpDuration = 1f;
    [SerializeField] private float impactRadius = 3f;
    [SerializeField] private int damage = 20;
    [SerializeField] private float restTimeAfterJump = 1.5f;

    private void Start()
    {
        if (animator == null) // [ANIM] fallback caso o campo não esteja ligado no prefab
        {
            animator = GetComponent<Animator>(); // [ANIM]
        }

        if (player == null) // [FIX] sem isso o BossCycle dá NullReference no player.position
        {
            GameObject playerObject = GameObject.FindWithTag("Player"); // [FIX]

            if (playerObject != null) // [FIX]
            {
                player = playerObject.transform; // [FIX]
            }
        }

        StartCoroutine(BossCycle());
    }

    private IEnumerator BossCycle()
    {
        while (player == null || Vector3.Distance(player.position, transform.position) > startBattleDistance) // [FIX] checa null
        {
            yield return null;
        }

        while (true)
        {
            float time = 0f;

            while (time < timeBeforeJump)
            {
                SpawnEnemy();
                yield return new WaitForSeconds(spawnInterval);
                time += spawnInterval;
            }

            yield return StartCoroutine(JumpAttack());
        }
    }

    private void SpawnEnemy()
    {
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        if (spawnPoint == null) return; // [FIX] slot vazio no array não derruba o boss

        SetTrigger(SpawnParam); // [ANIM] gesto de invocar -> estado "Spawn"

        Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
    }

    private IEnumerator JumpAttack()
    {
        if (player == null) yield break;

        Vector3 startPos = transform.position;
        Vector3 targetPos = player.position;

        targetPos.y = startPos.y;

        transform.LookAt(new Vector3(targetPos.x, transform.position.y, targetPos.z));

        SetTrigger(PrepareJumpParam); // [ANIM] agacha e toma impulso -> estado "PrepareJump"

        yield return new WaitForSeconds(prepareJumpTime);

        SetAirborne(true); // [ANIM] saiu do chão -> estado "Jump" (fica em loop no ar)

        float time = 0f;

        while (time < 1f)
        {
            time += Time.deltaTime / jumpDuration;
            Vector3 position = Vector3.Lerp(startPos, targetPos, time);
            position.y += Mathf.Sin(time * Mathf.PI) * jumpHeight;
            transform.position = position;
            yield return null;
        }

        transform.position = targetPos;

        SetAirborne(false); // [ANIM] encostou no chão -> estado "Land" (impacto)

        ApplyDamage();

        yield return new WaitForSeconds(restTimeAfterJump);
        // [ANIM] não precisa setar nada aqui: Land volta pro Idle sozinho pelo exit time
    }

    // [ANIM] os dois únicos pontos que falam com o Animator
    private void SetTrigger(string param) // [ANIM]
    {
        if (animator == null) return;  // [ANIM]
        animator.SetTrigger(param);    // [ANIM]
    }

    private void SetAirborne(bool airborne) // [ANIM]
    {
        if (animator == null) return;                    // [ANIM]
        animator.SetBool(AirborneParam, airborne);       // [ANIM]
    }

    private void ApplyDamage()
    {
        if (player == null) return;

        float distance = Vector3.Distance(player.position, transform.position);

        if (distance <= impactRadius)
        {
            if (player.TryGetComponent(out Health health))
            {
                health.TakeDamage(damage);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, startBattleDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, impactRadius);
    }
}
