# Copilot Instructions for Videojoc2D

## Project Overview
Unity 2022.3.32f1 cooperative 2D puzzle platformer game written in C# with Catalan codebase. Two players (Jugador1/Jugador2) cooperate to place objects (Objectes) in correct locations (PuntColocacio) within time limit.

## Architecture & Core Systems

### Singleton Pattern for Global Systems
- **GameManager**: Persists across scenes with `DontDestroyOnLoad`, orchestrates victory conditions, score, and scene transitions
- **AudioManager** (ControladorSo.cs): Manages sound effects via `Instance.ReproduirSoUncop()` and looping audio via `ReproduirSoEnBucle()`
- **ControladorPuntuacio**: Handles scoring system (100 pts per object + remaining time)

Access singletons: `GameManager.Instance`, `AudioManager.Instance`, `ControladorPuntuacio.Instance`

### Player System (MovimentsJugadors.cs)
Dual-player control scheme with distinct keybindings assigned via tags:
- **Jugador1**: A/D (horizontal), W/S (vertical/ladders), assigned via `CompareTag("Jugador1")`
- **Jugador2**: J/L (horizontal), I/K (vertical/ladders), assigned via `CompareTag("Jugador2")`

**Critical behavior**: Players cannot climb ladders while holding objects (`agafarObjecte.teObjecte` check). Players can stand on each other (ground detection in `comprovarEstaAterra()`).

### Object Placement System
Victory requires placing all objects correctly:
1. **AgafarObjecte.cs**: Pick up/drop objects with special key, constrained to ground level via `PosicionarARasDeSol()`
2. **ControladorObjecte.cs**: Objects snap to `PuntColocacio` when within `snapDistance` and `idObjecte` matches `idCorrecte`
3. **PuntColocacio.cs**: Placement points with optional spotlight color validation (`ColorEsCorrecto()`)
4. **GameManager.ComprovarVictoria()**: Checks all `PuntColocacio.ocupat` and color correctness

### Interactive Elements
- **BotoPlataforma.cs**: Multi-player button that activates when `jugadoresEncima > 0`
- **ControladorPlataforma.cs**: Moving platforms with audio feedback during movement
- **ControladorFocus.cs**: Light-based puzzles with color matching

### Scene Management
- **ControladorEscena.cs**: Scene loading with optional audio transition via `SoCarregarEscena()`
- **GameManager.OnSceneLoaded()**: Auto-initializes level systems for scenes starting with "Nivell"
- Scenes: Menu, Config, Instruccions, Nivell1-3, Victoria, Derrota, SeleccioNivell, Credits

### Localization
Unity Localization package with dropdown selection (ControladorIdioma.cs):
- Supported: Catalan (ca), Spanish (es), English (en)
- UI uses TextMeshPro (TMPro) components

## Coding Conventions

### Naming (Catalan/Spanish)
- **Variables**: camelCase in Catalan (e.g., `estaAterra`, `movimentHorizontal`, `velocitatMoviment`)
- **Methods**: PascalCase in Catalan (e.g., `ComprovarVictoria`, `ReproduirSoUncop`, `AturarSo`)
- **Boolean fields**: Start with `esta/es/te` (e.g., `estaAgafat`, `estaAterra`, `teObjecte`)
- **UI Text**: Use `TextMeshProUGUI` (TMPro) for all text elements

### Physics & Layers
- Use `Physics2D.OverlapBoxAll()` for ground/ladder detection
- Tags: `Jugador1`, `Jugador2`, `Objecte`, `Terra`
- Sorting layers: Default → "Decoracions" when object placed
- Set `rb.isKinematic = true` and destroy colliders after object placement

### Audio Integration
Always use `AudioManager.Instance` for SFX:
```csharp
AudioManager.Instance?.ReproduirSoUncop(clipVariable);  // One-shot
AudioManager.Instance.ReproduirSoEnBucle(clipVariable);  // Loop
AudioManager.Instance.AturarSo();  // Stop looping
```

### Animator Parameters
Standard parameters in MovimentsJugadors: `Horizontal` (float), `VelocitatY` (float), `estaAterra` (bool), `estaEscales` (bool), `teObjecte` (bool)

## Development Workflow

### Unity Version & Packages
- Unity 2022.3.32f1 LTS
- Addressables, TextMeshPro, Localization, Universal Render Pipeline 2D
- Open project via Unity Hub, not VS Code

### Key Directories
- `Assets/Scripts/`: All C# gameplay scripts
- `Assets/Scenes/`: Unity scenes (Menu, Nivell1-3, etc.)
- `Assets/Prefabs/Managers/`: Singleton manager prefabs
- `Assets/Animacions/`, `Assets/Audio/`, `Assets/Sprites/`: Media assets

### Testing Levels
- Test cooperative mechanics in Nivell1-3 scenes
- Use Menu → SeleccioNivell for level access
- Victory requires correct object placement + optional color matching
- Defeat triggered by `ControladorCrono` timeout

### Common Patterns
- Scene initialization: Use `FindObjectOfType<>()` in `Start()` or `OnSceneLoaded()`
- Coroutines: Wait one frame with `yield return new WaitForEndOfFrame()` for scene object initialization
- Victory check: Always call `GameManager.Instance?.ComprovarVictoria()` after placement/color changes
