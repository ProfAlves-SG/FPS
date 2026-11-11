using System.Collections;
using UnityEngine;

public class BossClass : MonoBehaviour
{
    [Header("Alvo")]
    public Transform jogador;                 // se ficar vazio, o boss procura pela tag "Player"
    public float distanciaParaComecar = 15f;  // o player precisa chegar perto assim para a luta começar

    [Header("Spawn de inimigos")]
    public GameObject inimigoPrefab;      // prefab do inimigo que já está pronto
    public Transform[] pontosDeSpawn;     // arraste aqui os pontos de spawn
    public float tempoEntreSpawns = 2f;   // de quanto em quanto tempo nasce um inimigo

    [Header("Ataque de pulo")]
    public float tempoAtePular = 8f;      // quanto tempo o boss fica spawnando antes de pular
    public float tempoDePreparo = 0.5f;   // agachadinha antes de pular
    public float alturaDoPulo = 5f;
    public float duracaoDoPulo = 1f;
    public float raioDoImpacto = 3f;      // área do soco no chão
    public int dano = 20;
    public float tempoDescanso = 1.5f;    // tempo parado depois do ataque

    // public Animator animator;          // descomente quando tiver as animações

    void Start()
    {
        StartCoroutine(CicloDoBoss());
    }

    IEnumerator CicloDoBoss()
    {
        while (jogador == null || Vector3.Distance(transform.position, jogador.position) > distanciaParaComecar)
        {
            yield return null; // espera o próximo frame e checa de novo
        }

        // aqui é um bom lugar para tocar a música do boss ou mostrar a barra de vida!

        // O boss repete para sempre: spawna inimigos -> pula -> spawna -> pula...
        while (true)
        {
            // FASE 1: parado, spawnando inimigos
            float tempo = 0f;
            while (tempo < tempoAtePular)
            {
                SpawnarInimigo();
                yield return new WaitForSeconds(tempoEntreSpawns);
                tempo += tempoEntreSpawns;
            }

            // FASE 2: pula no jogador e soca o chão
            yield return StartCoroutine(PularEAtacar());
        }
    }

    void SpawnarInimigo()
    {
        // escolhe um ponto de spawn aleatório
        Transform ponto = pontosDeSpawn[Random.Range(0, pontosDeSpawn.Length)];
        Instantiate(inimigoPrefab, ponto.position, ponto.rotation);
    }

    IEnumerator PularEAtacar()
    {
        if (jogador == null) yield break; // sem jogador, não pula

        Vector3 inicio = transform.position;
        Vector3 alvo = jogador.position;
        alvo.y = inicio.y; // cai na mesma altura do chão onde estava

        // vira para o jogador
        transform.LookAt(new Vector3(alvo.x, transform.position.y, alvo.z));

        // ===== ANIMAÇÃO: INÍCIO DO PULO (agachar / tomar impulso) =====
        // animator.SetTrigger("InicioPulo");
        yield return new WaitForSeconds(tempoDePreparo);

        // ===== ANIMAÇÃO: VOANDO =====
        // animator.SetTrigger("Voando");

        bool jaDescendo = false;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duracaoDoPulo;

            // anda em linha reta até o alvo...
            Vector3 posicao = Vector3.Lerp(inicio, alvo, t);
            // ...e soma uma curva para cima (o seno faz o formato de arco)
            posicao.y += Mathf.Sin(t * Mathf.PI) * alturaDoPulo;
            transform.position = posicao;

            // passou do ponto mais alto: começa a descer com o soco
            if (t >= 0.5f && !jaDescendo)
            {
                jaDescendo = true;
                // ===== ANIMAÇÃO: ATAQUE PARA BAIXO (soco no chão) =====
                // animator.SetTrigger("AtaqueBaixo");
            }

            yield return null; // espera o próximo frame
        }

        transform.position = alvo;
        CausarImpacto();

        // aqui dá para colocar partícula de poeira e tremer a câmera!

        yield return new WaitForSeconds(tempoDescanso);
        // ===== ANIMAÇÃO: VOLTAR PARA IDLE =====
        // animator.SetTrigger("Idle");
    }

    void CausarImpacto()
    {
        // o jogador está perto o suficiente do impacto?
        float distancia = Vector3.Distance(transform.position, jogador.position);

        if (distancia <= raioDoImpacto)
        {
            if (jogador.TryGetComponent(out Health playerHealth))
            {
                playerHealth.TakeDamage(dano);
            }
        }
    }

    // desenha as áreas na Scene quando o boss está selecionado
    void OnDrawGizmosSelected()
    {
        // vermelho = área do soco
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, raioDoImpacto);

        // amarelo = distância para a luta começar
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distanciaParaComecar);
    }
}