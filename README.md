# Last 30 Seconds

2D roguelite action game built with Unity. Survive 30-second rounds, kill enemies, earn score, upgrade between rounds, and defeat three bosses to win.

## Tech Stack

- **Unity 6** (URP 2D)
- **Addressables** — scene loading (Persistent + GameScene)
- **DOTween** — UI animations, tweening
- **Cinemachine 3** — camera shake
- **New Input System** — keyboard input
- **TextMeshPro** — all UI text

## Architecture

Boot Scene → Persistent Scene (stays loaded) → GameScene (reloaded each round)


### State Machine

`Idle ←→ Move ←→ Attack → Idle`  
`↑                           ↓`   
`└────────── Hurt ←──────────┘`


- `PlayerStateMachine` owns the state machine
- Each state (`IdleState`, `MoveState`, `AttackState`, `HurtState`) is a separate class
- Attack supports input buffering for chaining
- Movement cancels Attack (at reduced speed)

### Key Systems

| System | Description |
|---|---|
| **DamageDealer** | Trigger/collision damage with layer filtering, knockback, boss insta-kill |
| **EnemySpawner** | Weighted random spawning with sliding pool window |
| **BossSpawner** | Spawns a single boss per round |
| **UpgradePanel** | 12-button upgrade UI, combined buttons, withdraw for enemy upgrades |
| **RuntimeData** | Runtime stats synced from UpgradeData at start of each round |
| **UpgradeData** | ScriptableObject with 14 upgrade dimensions, 21 stages each |
| **GameStats** | End-of-run stats tracking (score, kills, time, etc.) |
| **CountdownTimer** | 30s countdown with flash warning and beep countdown |
| **LoadSceneManager** | Addressables scene loading with fade transitions |

## Project Structure

Assets/Script/  
├── Player/ # Player + StateMachine + States  
├── Enemy/ # EnemyStats, EnemyMovement, EnemySpawner, BossSpawner  
├── UpgradePanel.cs # Upgrade UI logic  
├── UpgradeData.cs # Upgrade progression data (SO)  
├── RuntimeData.cs # Runtime stat singleton  
├── DamageDealer.cs # Damage/knockback system  
├── CountdownTimer.cs # 30-second timer  
├── GameManager.cs # Global state flags  
├── LoadSceneManager.cs # Scene loading  
├── VictoryPanel.cs # End screen stats  
├── FadePanel.cs # Screen fade transitions  
├── SlashWave.cs # Projectile visual effect  
├── DamagePopup.cs # Floating damage numbers  
├── BossHealthBar.cs # Dual-fill boss health bar  
├── Parallax.cs # Background parallax scrolling  
├── UIScroller.cs # UI image scrolling  
└── ...


## Upgrade System

14 upgrade dimensions across Combat, Movement, Defense, Economy, and Enemy categories. For each dimension:

- `UpgradeData` stores 21 stages per dimension with values and prices
- `RuntimeData` syncs from UpgradeData at round start
- Components read live from `RuntimeData.Instance`

Buttons 8-11 (Enemy upgrades) support withdraw. Button 2 (spawnSlashWave) switches to slashWaveDamage after unlock. Button 11 (enemyVariety) switches to enemyBoss when maxed.

## Boss Progression

1. Purchase Boss 1 in upgrade panel
2. Defeat Boss 1 → unlocks ability to purchase Boss 2
3. Purchase and defeat Boss 2 → unlocks Boss 3
4. Defeat Boss 3 → Victory screen

Boss deaths during combat trigger round end (boss 1-2) or victory (boss 3). Boss damage is lethal regardless of dodge.

## Scene Flow

1. **Boot** loads Persistent then GameScene
2. Press any key to start countdown
3. Survive 30 seconds, kill enemies for score
4. Timer ends or boss dies → Upgrade Panel opens
5. Spend currency on upgrades → Close → GameScene reloads
6. Repeat until Boss 3 defeated → Victory screen
