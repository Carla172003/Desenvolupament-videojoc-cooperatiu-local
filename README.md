# Break a Leg!

**Autora:** Carla López Campos

---

## Índex
1. [Descripció del Projecte](#descripció-del-projecte)
2. [Arquitectura](#arquitectura)
3. [Navegació d'Interfícies](#navegació-dinterfícies)
4. [Scripts Controladors](#scripts-controladors)
5. [Elements d'Art i Reutilització](#elements-dart-i-reutilització)
6. [Convencions de Codi](#convencions-de-codi)
7. [Llibreries Utilitzades](#llibreries-utilitzades)

---

## Descripció del Projecte

**Break a Leg** és un joc trencaclosques cooperatiu 2D desenvolupat amb Unity 2022.3.32f1. Dos jugadors col·laboren per col·locar objectes escènics en les seves ubicacions correctes dins d'un límit de temps, resolent puzzles de dependències.

### Característiques Principals
- Mode cooperatiu local amb controls independents per a 2 jugadors
- Sistema de progressió amb estrelles (1-3 per nivell)
- Puzzles amb dependències entre objectes i armaris bloquejats amb claus
- Sistema de puntuació basat en objectes col·locats i temps restant
- Multiidioma: Català, Espanyol, Anglès

---

## Arquitectura

### Patró ECS (Entity-Component-System)

El projecte segueix el patró ECS d'Unity amb una clara separació entre dades i lògica:

- **Entities (GameObjects)**: Jugadors, objectes col·locables, punts de col·locació, armaris
- **Components**: MonoBehaviours que contenen dades (`ControladorObjecte`, `MovimentsJugadors`, etc.)
- **Systems**: Scripts que processen la lògica (GameManager gestiona victòria, ControladorPuntuacio calcula estrelles)

### Arquitectura de 3 Capes

```
┌─────────────────────────────────────┐
│     CAPA DE PRESENTACIÓ (UI)        │
│  - ControladorPanellsInfo           │
│  - BotoNivell, BotoColor            │
│  - MostrarEstrelles                 │
│  - ControladorPausa                 │
└─────────────────────────────────────┘
              ↕
┌─────────────────────────────────────┐
│    CAPA DE LÒGICA (GESTORS)         │
│  - GameManager (victòria/derrota)   │
│  - ControladorPuntuacio (càlculs)   │
│  - GestorDadesNivells (progressió)  │
│  - ControladorCrono (temps)         │
└─────────────────────────────────────┘

Capa de persistència no implementada.
```

**Separació de Responsabilitats:**
- **UI**: Només visualització i interacció bàsica
- **Controladors**: Lògica de negoci, càlculs, decisions

**Patró Singleton**: Els controladors (`GameManager`, `ControladorSo`, `ControladorPuntuacio`, `GestorDadesNivells`) utilitzen Singleton amb `DontDestroyOnLoad` per persistir entre escenes.

---

## Navegació d'Interfícies

### Flux de Navegació

```
Menu Principal
├─→ Instruccions → (Torna a Menu)
├─→ Configuració → (Torna a Menu)
├─→ Selecció Nivells
│   ├─→ Nivell 1
│   ├─→ Nivell 2 
│   ├─→ Nivell 3 
│   ├─→ Nivell 4 
│   └─→ Nivell 5 
│       ├─→ Victoria → (Torna a Menu)
│       ├─→ Derrota → (Torna a Menu)
│       └─→ Pausar → (Continuar, Reiniciar, Torna a Menu)
└─→ Credits → (Torna a Menu)
```

### Escenes del Projecte

| Escena | Propòsit | Controladors Clau |
|--------|----------|------------------|
| `Menu` | Pantalla inicial | ControladorEscena |
| `Instruccions` | Tutorial del joc | ControladorPanellsInfo |
| `Config` | Ajustos d'idioma i volum | ControladorIdioma, ControladorMusica, ControladorSo |
| `SeleccioNivell` | Selecció i bloqueig | BotoNivell, GestorDadesNivells |
| `Nivell1-5` | Gameplay | GameManager, ControladorCrono, ControladorPuntuacio, ControladorJugador, ... |
| `Victoria` | Pantalla èxit | MostrarEstrelles |
| `Derrota` | Pantalla fracàs | Mostra puntuació actual |
| `Credits` | Crèdits del joc | ControladorEscena |

---

## Scripts Controladors

### GameManager.cs
**Responsabilitat**: Gestor principal del joc amb patró Singleton.

- Persisteix entre escenes amb `DontDestroyOnLoad`
- Gestiona transicions d'escenes amb fade
- Comprova condicions de victòria (tots els objectes col·locats + colors correctes)
- Calcula puntuació final i estrelles
- Inicialitza nivells

**Mètodes clau**: `ComprovarVictoria()`, `FinalitzarAmbVictoria()`, `PerderPartida()`, `InicialitzarNivell()`

### ControladorObjecte.cs
**Responsabilitat**: Gestió de col·locació d'objectes amb sistema de dependències.

- Valida dependències abans de permetre col·locació
- Reprodueix so d'error si falta dependència
- Assigna `sortingOrder = 10` per objectes dependents, `0` per objectes base
- Suma punts i crida `ComprovarVictoria()` després de col·locar

**Camps clau**: `idObjecte`, `idObjecteDependencia`, `snapDistance`, `soError`

### ControladorPuntuacio.cs
**Responsabilitat**: Gestió de puntuació i càlcul d'estrelles.

- Acumula punts durant la partida (100 pts per objecte)
- Calcula puntuació final amb bonus de temps (`temps * 100`)
- Determina estrelles segons llindars configurables
- Reset selectiu: només reinicia en escenes que comencen amb "Nivell"

**Mètodes clau**: `SumarPunts()`, `CalcularPuntuacioFinal()`, `CalcularEstrelles()`

### GestorDadesNivells.cs
**Responsabilitat**: Emmagatzematge temporal de progressió (només durant sessió).

- Utilitza `Dictionary<string, int>` per puntuacions i estrelles
- NO usa PlayerPrefs (dades es perden en tancar app)
- Només guarda si la nova puntuació supera l'anterior
- Persisteix amb `DontDestroyOnLoad(transform.root.gameObject)`

**Mètodes clau**: `GuardarDadesNivell()`, `ObtenirPuntuacioMaxima()`, `ObtenirEstrellesMaximes()`

### MovimentsJugadors.cs
**Responsabilitat**: Control de moviment i animació dels jugadors.

- Detecta input segons tag (`Jugador1`: A/D/W/S/E, `Jugador2`: Fletxes/RightShift)
- Gestiona moviment horitzontal, salt i pujada d'escales
- Comprova si està a terra amb `Physics2D.OverlapBoxAll()`
- Bloqueja pujada d'escales si aguanta objecte
- Actualitza paràmetres d'Animator: `Horizontal`, `VelocitatY`, `estaAterra`, `estaEscales`, `teObjecte`

**Restriccions**: No pot pujar escales amb objecte, els jugadors poden estar-se dempeus l'un sobre l'altre.

### AgafarObjecte.cs
**Responsabilitat**: Permet als jugadors agafar i deixar objectes.

- Detecta objectes propers amb `OnTriggerEnter2D`/`OnTriggerExit2D`
- Agafa objecte i l'adjunta al `puntAgafar` (GameObject fill del jugador)
- Desactiva física durant transport (`rb.isKinematic = true`)
- Crida `IntentarColocar()` quan es deixa l'objecte
- Posiciona objecte a ras de sòl amb `PosicionarARasDeSol()`

**Camps clau**: `puntAgafar`, `teObjecte`, `objecteActual`

### PuntColocacio.cs
**Responsabilitat**: Punt de col·locació d'objectes amb validació de color.

- Defineix ID correcte i distància de snap
- Comprova si color del focus coincideix (`ColorEsCorrecto()` amb tolerància 0.01f)
- Marca si està ocupat (`ocupat = true`)
- Pot tenir llum opcio (`Light2D`) per puzzles de color

**Camps clau**: `idCorrecte`, `ocupat`, `colorCorrecto`, `colorsDisponibles[]`

### ZonaObjectes.cs
**Responsabilitat**: Generació d'objectes aleatoris dels armaris amb sistema de bloqueig.

- Genera objectes de `objectesPossibles[]` quan jugador interactua
- Sistema de bloqueig amb clau: `estaBloquejar`, `spriteCadenat`, `idLlave`
- Detecta clau amb `OnTriggerStay2D()` i verifica `!estaAgafat`
- Desbloqueja amb animació (`estaBloquejat = false`), restaura sprite original
- Bloqueja generació si armari bloquejat i no desbloquejat

**Tipus armari**: `TipusArmari` enum (Attrezzo, Llums, Vestimenta)

### ControladorCrono.cs
**Responsabilitat**: Gestió del cronòmetre amb sistema d'avís crític.

- Compte enrere des de `tempsMaxim` (default 120s)
- Avís als últims 10s: text vermell + so tic-tac (una vegada)
- Flag `avisCritic` evita múltiples reproduccions
- Crida `GameManager.PerderPartida()` quan arriba a 0
- `EstablirCrono()` reinicia color i flag

**Camps clau**: `tempsMaxim`, `tempsAvís`, `colorCrític`, `soTicTac`

### BotoNivell.cs
**Responsabilitat**: Visualització d'estrelles i bloqueig de nivells.

- Mostra estrelles obtingudes (swap entre `estrellaPlena`/`estrellaVacia`)
- Comprova si nivell anterior té >= 1 estrella
- Bloqueja boto si no compleix requisit (`interactable = false`, alpha 0.3f)
- Carrega escena del nivell quan es clica

**Camps clau**: `nomNivell`, `nomNivellAnterior`, `estrelles[]`, `boto`

### ControladorFocus.cs
**Responsabilitat**: Gestió de llums col·locables per puzzles de color.

- Assigna color inicial aleatori (MAI el correcte)
- `ConfigurarDesdePunt()` crida per `ControladorObjecte` després de col·locar
- `EncenderLuz()` activa spotlight amb intensitat 1f
- `CambiarColor()` crida per `BotoColor` per canviar color
- `ComprovarVictoria()` després de cada canvi

**Camps clau**: `spotlight`, `colorsDisponibles[]`, `colorAsignat`

### GestorPausa.cs
**Responsabilitat**: Detecció de tecla Escape per pausar el joc.

- GameObject sempre actiu que detecta `Input.GetKeyDown(KeyCode.Escape)`
- Verifica que `ControladorPanellsInfo.panells` no estigui actiu (bloqueig)
- Troba `ControladorPausa` i crida `PausarJoc()` / `ReprendreJoc()`
- Ha d'estar en GameObject root de l'escena

**Important**: No es desactiva mai, controla pausa global.

### ControladorPausa.cs
**Responsabilitat**: Control de la UI del menú de pausa.

- Activa/desactiva panell de pausa
- Gestiona `Time.timeScale` (0 = pausa, 1 = normal)
- Botons: Reprendre, Tornar al Menu, Sortir

**Mètodes clau**: `PausarJoc()`, `ReprendreJoc()`

### ControladorEscena.cs
**Responsabilitat**: Gestió de transicions entre escenes.

- Carrega escenes amb nom o índex
- Opcional: reprodueix so durant transició (`SoCarregarEscena()`)
- Utilitza `SceneManager.LoadScene()`

**Mètodes clau**: `CarregarEscena()`, `CarregarEscenaPerIndex()`, `SortirJoc()`

### ControladorSo.cs (AudioManager)
**Responsabilitat**: Gestió global d'àudio amb Singleton.

- Reprodueix sons únics amb `ReproduirSoUncop(AudioClip)`
- Reprodueix sons en bucle amb `ReproduirSoEnBucle(AudioClip)`
- Atura so en bucle amb `AturarSo()`
- Components: `AudioSource` per efectes, `AudioSource` per música

**Accés**: `ControladorSo.Instance?.ReproduirSoUncop(clip)`

### ControladorPanellsInfo.cs
**Responsabilitat**: Mostra panells informatius al començar nivells.

- Gestiona array de panells que es mostren seqüencialment
- `MostrarPanellsInformatius()` activa primer panell
- `SeguentPanell()` / `PanellAnterior()` navegació
- Quan s'acaben tots, crida `GameManager.IniciarPartida()`

**Camps clau**: `panells[]`, `indexPanellActual`

### MostrarEstrelles.cs
**Responsabilitat**: Visualització d'estrelles a la pantalla de victòria.

- Llegeix `GameManager.Instance.numEstrelles`
- Swap sprites entre `estrellaPlena` i `estrellaVacia` segons puntuació
- Mostra 0 estrelles si partida no completada

**Camps clau**: `estrelles[]` (Image), `estrellaPlena`, `estrellaVacia`

### BotoColor.cs
**Responsabilitat**: Canvi de color de focus.

- Detecta jugador proper amb `OnTriggerEnter2D`
- En prémer tecla especial, crida `focus.CambiarColor()`
- Genera nou color aleatori dels disponibles

**Camps clau**: `focus` (ControladorFocus)

---

## Elements d'Art i Reutilització

### Eina de Desenvolupament

**Aseprite** s'ha utilitzat per crear tots els sprites del joc, permetent treballar amb pixel art i animacions de forma eficient.

### Especificacions Tècniques

**Mida de Tiles:**
- Dimensió base: **16x16 píxels**
- Pantalla del joc: **32 tiles × 16 tiles** (512×256 píxels)

### Sistema de Tematització per Nivells

Cada nivell utilitza una temàtica visual diferent que afecta a tots els elements de l'escenari:

| Nivell | Temàtica | Elements Visuals |
|--------|----------|------------------|
| Nivell 1 | **Medieval** | Castells, espasses |
| Nivell 2 | **Pirates** | Mapes, barrils, tresors |
| Nivell 3 | **Oest** | Cartells, bar oest|
| Nivell 4 | **Roma** | Columnes, toges|
| Nivell 5 | **Egipte** | Piràmides, sarcòfags, camells |

**Reutilització de Tilesets:**
- Cada nivell té el seu propi tileset de 16×16 píxels
- Es mantenen les mateixes dimensions i proporcions
- Només canvien colors, textures i estils visuals
- Facilita la creació de nous nivells amb temàtiques diferents

### Sistema de Plantilles per Personatges

**Jugadors i NPCs** comparteixen una arquitectura visual modular:

```
Plantilla Base (Corporal)
├─→ Capa 1: Cos/Silueta (invariable)
├─→ Capa 2: Cabell (intercanviable)
└─→ Capa 3: Vestimenta (intercanviable segons temàtica)
```

**Avantatges del Sistema:**
- **Consistència**: Tots els personatges mantenen les mateixes proporcions
- **Eficiència**: Es reutilitza la base corporal i animacions
- **Varietat**: Cabell i roba permeten personalització
- **Tematització**: La vestimenta s'adapta al nivell (túniques romanes, robes egípcies, etc.)

### Recomanacions per Afegir Nou Art

**Workflow recomanat amb Aseprite:**
```
1. Crear sprite → Exportar .png
2. Importar a Unity (Sprite Mode: Multiple si és spritesheet)
3. Configurar Pixels Per Unit: 16
4. Filter Mode: Point (no filter) per pixel art nítid
```

---

## Convencions de Codi

### Nomenclatura

**Variables i camps privats**: camelCase en català
```csharp
private bool estaAterra;
private float velocitatMoviment;
private int puntuacioActual;
```

**Mètodes públics i privats**: PascalCase en català
```csharp
public void ComprovarVictoria();
private void ReproduirSoUncop();
public void SumarPunts(int quantitat);
```

**Prefixos booleans**: `esta`, `es`, `te`
```csharp
public bool estaAgafat;
public bool estaAterra;
public bool teObjecte;
public bool esValid;
```

### Física

**Tags**: `Jugador1`, `Jugador2`, `Objecte`, `Terra`

**Sorting Layers**:
- Default: Objectes no col·locats
- Decoracions: Objectes col·locats
- Objectes amb dependències: `sortingOrder = 10`
- Objectes base: `sortingOrder = 0`


### Animacions

**Paràmetres estàndard** (MovimentsJugadors):
- `Horizontal` (float)
- `VelocitatY` (float)
- `estaAterra` (bool)
- `estaEscales` (bool)
- `teObjecte` (bool)

**Paràmetres armaris** (ZonaObjectes):
- `estaBloquejat` (bool)
- `TancarPortesArmari` (trigger)

---

## Llibreries Utilitzades

### Packages Unity

| Package | Versió | Ús |
|---------|--------|-----|
| **Universal RP** | 14.x | Render Pipeline 2D, sistema de llums |
| **TextMesh Pro** | 3.x | Text UI d'alta qualitat |
| **Localization** | 1.x | Sistema multiidioma (ca/es/en) |
| **Addressables** | 1.x | Gestió d'assets |
| **2D Sprite** | 1.x | Animacions 2D |
| **2D Tilemap Editor** | 1.x | Creació de nivells |

### Namespaces Principals

```csharp
using UnityEngine;
using UnityEngine.SceneManagement;  // Gestió escenes
using UnityEngine.Rendering.Universal;  // Light2D
using TMPro;  // TextMeshProUGUI
using System.Collections.Generic;  // Dictionary, HashSet
```

### Requisits del Sistema

- **Unity**: 2022.3.32f1 LTS
- **Versió .NET**: .NET Standard 2.1
- **Plataforma**: Windows, macOS, Linux
- **Resolució recomanada**: 1920x1080

### Dependències de Scripts

```
GameManager
├─→ ControladorPuntuacio
├─→ ControladorCrono
├─→ GestorDadesNivells
└─→ ControladorObjecte (static)

ControladorObjecte
├─→ PuntColocacio
├─→ ControladorFocus
└─→ GameManager

ZonaObjectes
└─→ ControladorObjecte

MovimentsJugadors
└─→ AgafarObjecte
    └─→ ControladorObjecte
```

---

**Versió del Joc**: 1.0  
**Data d'actualització**: Gener 2026  
**Repositori**: Unity 2022.3.32f1 LTS
