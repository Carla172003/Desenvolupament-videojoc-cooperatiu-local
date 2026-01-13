# Copilot Instructions for Videojoc2D

## Project Overview
Unity 2022.3.32f1 cooperative 2D puzzle platformer game written in C# with Catalan codebase. Two players (Jugador1/Jugador2) cooperate to place objects (Objectes) in correct locations (PuntColocacio) within time limit. Features star rating system, level progression, object dependencies, and locked cabinets.

## Architecture & Core Systems

### Singleton Pattern for Global Systems
- **GameManager**: Persists across scenes with `DontDestroyOnLoad`, orchestrates victory conditions, score, scene transitions, and star tracking
- **AudioManager** (ControladorSo.cs): Manages sound effects via `Instance.ReproduirSoUncop()` and looping audio via `ReproduirSoEnBucle()`
- **ControladorPuntuacio**: Handles scoring system (100 pts per object + remaining time * 100), calculates stars (1-3) based on configurable thresholds
- **GestorDadesNivells**: Session-only progress storage using `Dictionary<string, int>` (no persistence between app runs)
- **GestorPausa**: Always-active GameObject that detects Escape key for pause menu

Access singletons: `GameManager.Instance`, `AudioManager.Instance`, `ControladorPuntuacio.Instance`, `GestorDadesNivells.Instance`

### Player System (MovimentsJugadors.cs)
Dual-player control scheme with distinct keybindings assigned via tags:
- **Jugador1**: A/D (horizontal), W/S (vertical/ladders), E (interact), assigned via `CompareTag("Jugador1")`
- **Jugador2**: J/L (horizontal), I/K (vertical/ladders), RightShift (interact), assigned via `CompareTag("Jugador2")`

**Critical behavior**: Players cannot climb ladders while holding objects (`agafarObjecte.teObjecte` check). Players can stand on each other (ground detection in `comprovarEstaAterra()`).

### Object Placement System with Dependencies
Victory requires placing all objects correctly:
1. **AgafarObjecte.cs**: Pick up/drop objects with special key, constrained to ground level via `PosicionarARasDeSol()`. Sets `estaAgafat` flag in ControladorObjecte
2. **ControladorObjecte.cs**: Objects snap to `PuntColocacio` when within `snapDistance` and `idObjecte` matches `idCorrecte`
   - **Dependencies**: `idObjecteDependencia` field - if set, object cannot be placed until dependency object is placed first
   - **Error sound**: `soError` plays when attempting placement without dependency (only if near valid point)
   - **Layering**: Objects with dependencies get `sortingOrder = 10`, base objects get `sortingOrder = 0`
   - **Static tracking**: `objectesColocats` HashSet tracks placed objects, cleared via `NetejaDependencies()` on level start
3. **PuntColocacio.cs**: Placement points with optional spotlight color validation (`ColorEsCorrecto()`)
4. **GameManager.ComprovarVictoria()**: Checks all `PuntColocacio.ocupat` and color correctness

### Cabinet System (ZonaObjectes.cs)
Generates random objects from list when players interact (E/RightShift):
- **Locking**: `estaBloquejar` bool enables lock, `spriteCadenat` Sprite shown when locked, `idLlave` string identifies key object
- **Unlock**: Drop key object (`idObjecte == idLlave`) inside zone to unlock. Checks `ControladorObjecte.estaAgafat` flag via `OnTriggerStay2D()`
- **Animation**: Uses Animator parameter `estaBloquejat` (bool) - true = locked state, false = trigger unlock animation
- **Original sprite**: Saved on Start, restored when unlocked via `AmagarCadenat()`
- **Types**: TipusArmari enum (Attrezzo, Llums, Vestimenta) with different close door triggers

### Star Rating & Progression System
- **ControladorPuntuacio**: 
  - Configurable thresholds: `puntuacioMin2Estrelles` (9600), `puntuacioMin3Estrelles` (12600)
  - `CalcularEstrelles(puntuacioFinal, partidaCompletada)` returns 0-3 stars
  - Only resets score in `Start()` if scene name starts with "Nivell"
- **GameManager**:
  - Stores `numEstrelles` and `nivellActual` (scene name)
  - `FinalitzarAmbVictoria()`: Calculates final score, stars, saves to GestorDadesNivells
  - `PerderPartida()`: Saves current score from ControladorPuntuacio to GameManager.puntuacio (NOT saved to progress)
- **GestorDadesNivells**: 
  - Session-only storage: `Dictionary<string, int> puntuacionsNivells/estrellesNivells`
  - NO PlayerPrefs usage - data clears on app close
  - `GuardarDadesNivell()` only saves if new score > previous
  - Uses `transform.root.gameObject` for DontDestroyOnLoad to handle nested GameObjects
- **BotoNivell**: 
  - Shows stars for each level button
  - `nomNivellAnterior` field for level locking
  - `ComprovarBloqueig()` checks previous level >= 1 star
  - `boto.interactable = false` and alpha 0.3f for locked levels
- **MostrarEstrelles**: Displays stars on Victoria screen using `Image.sprite` swapping (estrellaPlena/Vacia)

### Pause System
- **GestorPausa**: Always-active script on root GameObject that detects `Input.GetKeyDown(KeyCode.Escape)`
- **ControladorPausa**: Simple UI controller with `PausarJoc()` / `ReprendreJoc()` methods
- **Blocking**: Checks `ControladorPanellsInfo.panells` active state - if info panels shown, pause is blocked

### Timer System (ControladorCrono.cs)
- **Critical warning**: When `tempsRestant <= tempsAvís` (default 10s):
  - Changes `cronoUI.color` to `colorCrític` (red)
  - Plays `soTicTac` AudioClip once (10 second duration matches countdown)
  - `avisCritic` bool prevents re-triggering
- **Reset**: `EstablirCrono()` resets color to white and `avisCritic = false`

### Interactive Elements
- **BotoPlataforma.cs**: Multi-player button that activates when `jugadoresEncima > 0`
- **ControladorPlataforma.cs**: Moving platforms with audio feedback during movement
- **ControladorFocus.cs**: Light-based puzzles with color matching
  - Random initial color from `colorsDisponibles` array on placement
  - `ConfigurarDesdePunt()` called by ControladorObjecte when placed
  - `EncenderLuz()` sets spotlight intensity to 1f

### Scene Management
- **ControladorEscena.cs**: Scene loading with optional audio transition via `SoCarregarEscena()`
- **GameManager.OnSceneLoaded()**: Auto-initializes level systems for scenes starting with "Nivell"
- **GameManager.InicialitzarNivell()**: Calls `ControladorObjecte.NetejaDependencies()` to clear object tracking
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
- Object dependencies: Check `objectesColocats.Contains(idObjecteDependencia)` before allowing placement
- Cabinet unlocking: Use `OnTriggerStay2D()` to check `ControladorObjecte.estaAgafat` flag for dropped objects
- Star calculation: Always call after victory, pass `partidaCompletada = true` for wins, `false` for defeats
- Level progression: Check `GestorDadesNivells.Instance.ObtenirEstrellesMaximes(nomNivellAnterior) >= 1` for unlock

## Key Script Summaries

### GameManager.cs
- **Singleton lifecycle**: Awake checks Instance, subscribes to SceneManager.sceneLoaded
- **Level init**: `InicialitzarNivell()` finds controllers, calls `ControladorObjecte.NetejaDependencies()`
- **Victory**: `FinalitzarAmbVictoria()` calculates score (base + time*100), stars, saves via GestorDadesNivells
- **Defeat**: `PerderPartida()` saves `controladorPuntuacio.puntuacio` to GameManager (NOT to progress system)
- **Score persistence**: `puntuacio` and `numEstrelles` fields persist across scene loads via DontDestroyOnLoad

### ControladorObjecte.cs
- **Static tracking**: `static HashSet<string> objectesColocats` tracks placed objects across all instances
- **Dependency check**: Before snap, checks `objectesColocats.Contains(idObjecteDependencia)`, plays `soError` if fails
- **Layering logic**: Objects with `idObjecteDependencia` set get `sortingOrder = 10`, else `sortingOrder = 0`
- **Victory trigger**: After successful placement, calls `GameManager.Instance?.ComprovarVictoria()`
- **Cleanup**: `NetejaDependencies()` static method clears HashSet, called by GameManager on level start

### ZonaObjectes.cs
- **Lock state**: `estaBloquejar` bool, `estaDesbloquejat` bool (internal), `spriteCadenat` Sprite, `idLlave` string
- **Visual feedback**: `MostrarCadenat()` swaps to lock sprite, `AmagarCadenat()` restores original via `spriteOriginal`
- **Unlock detection**: `OnTriggerStay2D()` checks `llaveDetectada != null && !controlador.estaAgafat`
- **Animation**: Sets Animator bool `estaBloquejat` (true on Start if locked, false when unlocked)
- **Object generation**: Blocked if `estaBloquejar && !estaDesbloquejat`, only generates when `!hayObjetoDentro`

### ControladorPuntuacio.cs
- **Score accumulation**: `SumarPunts(100)` called by ControladorObjecte on each placement
- **Final calculation**: `CalcularPuntuacioFinal(tempsRestant)` returns `puntuacio + Mathf.RoundToInt(tempsRestant)`
- **Star logic**: `CalcularEstrelles()` checks thresholds: 0 if not completed, else 1/2/3 based on score
- **Scene-aware reset**: `Start()` only resets if `escenaActual.StartsWith("Nivell")`, preserves score for Victoria/Derrota
- **Public access**: `numEstrelles` property (get-only) accessed by MostrarEstrelles

### GestorDadesNivells.cs
- **Session-only storage**: Dictionary, NO PlayerPrefs, data clears on app close
- **Save logic**: `GuardarDadesNivell()` only updates if `puntuacio > ObtenirPuntuacioMaxima(nomNivell)`
- **DontDestroyOnLoad fix**: Uses `transform.root.gameObject` to handle nested hierarchy
- **Access methods**: `ObtenirPuntuacioMaxima(nomNivell)`, `ObtenirEstrellesMaximes(nomNivell)`

### BotoNivell.cs
- **UI references**: `Image[] estrelles` (3 stars), `Button boto`, `string nomNivell`, `string nomNivellAnterior`
- **Lock check**: `ComprovarBloqueig()` gets stars from previous level, locks if < 1
- **Visual feedback**: Locked buttons have `interactable = false` and alpha 0.3f (via `SetAlpha()`)
- **Star display**: `ActualitzarEstrelles()` swaps Image.sprite between estrellaPlena/Vacia

### ControladorCrono.cs
- **Warning system**: At `tempsRestant <= tempsAvís` (10s), sets `cronoUI.color = colorCrític` (red)
- **Audio**: Plays `soTicTac` once (10s duration) via `ReproduirSoUncop()` when warning triggers
- **State tracking**: `avisCritic` bool prevents re-triggering, reset in `EstablirCrono()`
- **Normal operation**: Counts down, calls `GameManager.Instance.PerderPartida()` at 0

### GestorPausa.cs
- **Input detection**: `Update()` checks `Input.GetKeyDown(KeyCode.Escape)`
- **Blocking**: If `ControladorPanellsInfo.panells` active (info screens), pause is blocked
- **Controller access**: Finds `ControladorPausa`, calls `PausarJoc()` / `ReprendreJoc()`
- **Always active**: Must be on root GameObject in scene, never gets deactivated

## Implementation Notes

### Object Dependency Example
```csharp
// In Unity Inspector:
// Ataüd: idObjecte = "Ataud", idObjecteDependencia = "" (no dependency)
// Momia: idObjecte = "Momia", idObjecteDependencia = "Ataud" (requires Ataud first)
// When placing Momia before Ataud: soError plays, placement blocked
// After Ataud placed: Momia can be placed, appears with sortingOrder=10 (above Ataud)
```

### Cabinet Locking Example
```csharp
// In Unity Inspector on ZonaObjectes:
// estaBloquejar = true
// spriteCadenat = [Sprite with lock icon]
// idLlave = "Llave"
// Animator has bool parameter "estaBloquejat"
// Animation states: obert (default) ↔ bloquejat (when bool true) → obrirPortes (transition when bool false)
```

### Star Rating Thresholds
```csharp
// Default values in ControladorPuntuacio:
// puntuacioMin2Estrelles = 9600
// puntuacioMin3Estrelles = 12600
// Calculation: base score (100 per object) + time remaining * 100
// Example: 16 objects (1600 pts) + 110 seconds (11000 pts) = 12600 pts → 3 stars
```

### Session-Only Progress
```csharp
// GestorDadesNivells stores in Dictionary, NOT PlayerPrefs
// Progress exists only while app running
// On app close/restart: all progress cleared
// Design decision: No database/persistent storage required
```
