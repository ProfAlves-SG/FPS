# _BossSetup — Boss animado (humanoid, clipes autorais)

Tudo que foi criado para animar o boss está nesta pasta. **Nada fora dela foi alterado**
(`Assets/Prefabs/Boss.prefab`, `Assets/Scripts/Boss.cs` e `Assets/Scripts/BossClass.cs` continuam
como estavam).

## Por que os clipes são autorais

O pacote Kevin Iglesias instalado é a versão **FREE**: tem 64 clipes do `HumanM`, mas **nenhum de
pulo, queda ou invocação**. Então os 4 clipes aqui foram escritos à mão como
**AnimationClips humanoid** (curvas de músculo do Mecanim, `customType: 8`), não como curvas de
bone. Isso significa que eles **retargetam para qualquer avatar Humanoid** — HumanF, um personagem
do Mixamo, qualquer rig configurado como Humanoid. É só arrastar nos estados do Animator do outro
personagem.

## Arquivos

| Arquivo | O que é |
|---|---|
| `BossAnimated.prefab` | Boss pronto: `HumanM_Model` escala 2 + capacete, CapsuleCollider, Rigidbody kinematic, 4 spawn points filhos. |
| `BossAnimator.controller` | 5 estados, 3 parâmetros. |
| `Boss_PrepareJump.anim` | 0,5 s — agacha e toma impulso. |
| `Boss_Jump.anim` | 0,6 s, **loop** — no ar, pernas recolhidas, braços erguidos. |
| `Boss_Land.anim` | 0,8 s — impacto, esmaga e recupera. |
| `Boss_Spawn.anim` | 1,0 s — gesto de invocar, braços sobem e abrem. |
| `BossAnimated.cs` | Cópia de `Boss.cs` com as linhas marcadas `// [ANIM]` e `// [FIX]`. |
| `GenerateBossClips.ps1` | Gera os 4 `.anim`. Edite as poses no topo e rode de novo. |

## Animator

**Parâmetros:** `spawn` (Trigger), `prepareJump` (Trigger), `isAirborne` (Bool).

| Estado | Clipe | Entra quando | Sai quando |
|---|---|---|---|
| `Idle` (default) | `HumanM@MilitaryIdle01` (do pacote) | — | `spawn` ou `prepareJump` |
| `Spawn` | `Boss_Spawn` | trigger `spawn` | exit time 0.85 → `Idle` (ou `prepareJump` interrompe) |
| `PrepareJump` | `Boss_PrepareJump` | trigger `prepareJump` | `isAirborne = true` |
| `Jump` | `Boss_Jump` (loop) | `isAirborne = true` | `isAirborne = false` |
| `Land` | `Boss_Land` | `isAirborne = false` | exit time 0.9 → `Idle` |

O pulo usa **bool** em vez de trigger porque `jumpDuration` é ajustável: o estado `Jump` fica em
loop no ar o tempo que o código precisar. Os outros são one-shot, então trigger serve.

Não existe linha "voltar pro Idle": `Spawn` e `Land` voltam sozinhos pelo exit time.

## Onde ficam as linhas no código

Em `BossAnimated.cs`, 4 pontos:

| Onde | Linha | Resultado |
|---|---|---|
| `SpawnEnemy()`, antes do `Instantiate` | `SetTrigger("spawn")` | estado `Spawn` |
| `JumpAttack()`, depois do `LookAt` | `SetTrigger("prepareJump")` | estado `PrepareJump` |
| `JumpAttack()`, depois do `WaitForSeconds(prepareJumpTime)` | `SetBool("isAirborne", true)` | estado `Jump` |
| `JumpAttack()`, depois do loop do arco | `SetBool("isAirborne", false)` | estado `Land` |

As durações batem com os valores do inspector: `prepareJumpTime` 0,5 s = duração do
`Boss_PrepareJump`; `restTimeAfterJump` 1,5 s cobre os 0,8 s do `Boss_Land` + blend.

## Dois bugs que corrigi na cópia (`// [FIX]`)

1. **`player` nulo.** `Boss.cs` faz `Vector3.Distance(player.position, ...)` e o campo vem vazio no
   prefab — `NullReferenceException` no primeiro frame. Adicionei a busca pela tag `Player` no
   `Start()`, igual o `EnemyClass` já faz, e um check de null no `while`.
2. **`spawnPoints` com slots vazios.** O `Boss.prefab` tem 7 slots todos nulos; `SpawnEnemy()`
   sorteia um e faz `spawnPoint.position` → NRE. Agora um slot vazio é ignorado. O
   `BossAnimated.prefab` já vem com 4 spawn points filhos preenchidos.

## Ajustes que você provavelmente vai querer

- **`enemyPrefab`** aponta para `Assets/Prefabs/Enemy.prefab` (a cápsula antiga). Troque por
  `_EnemySetup/EnemyAnimated.prefab` se quiser que o boss invoque o soldado animado.
- **Momento do dano.** `ApplyDamage()` roda no mesmo frame em que o `Land` começa, mas o pico do
  impacto no clipe está em `t = 0.10`. Se quiser o dano exatamente no esmagamento, mova esse
  keyframe para `t = 0` na Animation window, ou ponha um `yield return new WaitForSeconds(0.1f)`
  antes do `ApplyDamage()`.
- **As poses.** Foram escritas a partir dos valores reais medidos do rig (ver abaixo), mas eu não
  consegui pré-visualizar no Editor. Abra cada clipe na Animation window com o
  `BossAnimated.prefab` selecionado e ajuste o que não gostar.

## Como os números das poses foram obtidos

Nada aqui é chute, mas também nada foi visto na tela:

- **Índices das curvas (7–136):** extraídos do `HumanoidDefault.anim` que vem no pacote Timeline.
  Curvas humanoid usam índice de músculo em `m_ClipBindingConstant`, não hash de string.
- **Nomes das curvas:** extraídos dos clipes já importados deste projeto (`Library/Artifacts`),
  então são os nomes exatos do Unity 2022.3.
- **Pose de pé (baseline):** medida do `HumanM@MilitaryIdle01` — por exemplo `RootT.y = 0.94`,
  `Left Arm Down-Up = -0.45`, `Left Lower Leg Stretch = 0.65`, `Head Nod Down-Up = 0.47`.
  Repare que o neutro do rig **não é zero**, por isso partir de 0 deixaria a pose errada.
- **Sinais:** confirmados correlacionando `LeftFootT.z` (posição do pé) com
  `Left Upper Leg Front-Back` no `HumanM@Walk01_Forward`. Resultado:
  `Front-Back` **positivo = perna atrás**, e `Lower Leg Stretch` **alto = perna reta, baixo =
  joelho flexionado**.

A convenção geral do Mecanim é: o nome lê do negativo para o positivo. `Down-Up` → −1 = baixo,
+1 = cima. `Up-Down` (pé, dedos) → −1 = cima, +1 = baixo. `Front-Back` → −1 = frente, +1 = atrás.
`In-Out` → −1 = dentro, +1 = fora.

## Notas técnicas

- `Apply Root Motion` **off**: o código move o boss (`transform.position` com `Mathf.Sin`).
  O `RootT.y` dos clipes só posiciona o corpo em relação ao GameObject, então os dois compõem —
  é isso que faz o agachamento e o esmagamento funcionarem sem brigar com o arco do pulo.
- `Culling Mode` em **Always Animate**.
- O boss **não tem** o `SpineProxy` (ao contrário do enemy). Esse script copia a rotação do bone
  `B-spineProxy` para o `B-spine`, e só faz sentido quando há layer com Avatar Mask. Os clipes do
  pacote animam o `B-spineProxy` (ele está nas máscaras); os meus clipes, sendo só músculo
  humanoid, não — então com o script ativo ele travaria a espinha do boss na bind pose.
