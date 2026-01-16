# Break a Leg

**Autora:** Carla López Campos

---

## Índex
1. [Descripció del Projecte](#descripció-del-projecte)
2. [Descarregar i Jugar](#descarregar-i-jugar)
3. [Arquitectura](#arquitectura)
4. [Patrons de Disseny](#patrons-de-disseny)
5. [Navegació d'Interfícies](#navegació-dinterfícies)
6. [Scripts Controladors](#scripts-controladors)
7. [Elements d'Art i Reutilització](#elements-dart-i-reutilització)
8. [Convencions de Codi](#convencions-de-codi)
9. [Llibreries Utilitzades](#llibreries-utilitzades)

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

## Descarregar i Jugar

Pots descarregar els executables del joc per jugar sense necessitat d'instal·lar Unity:

### Windows
📥 [Descarregar per a Windows](https://github.com/carlalopez16/Videojoc2D/releases/download/v1.0/BreakALeg_Windows.zip)

**Requisits**:
- Windows 10 o superior
- Descomprimeix el fitxer .zip i executa `BreakALeg-Windows-v1.0.0.exe`

### macOS
📥 [Descarregar per a macOS](https://github.com/carlalopez16/Videojoc2D/releases/download/v1.0/BreakALeg_Mac.zip)

**Requisits**:
- macOS 10.13 o superior
- Descomprimeix el fitxer .zip i obre `BreakALeg-Mac-v1.0.0.app`

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

## Patrons de Disseny

El projecte implementa diversos patrons de disseny per facilitar el manteniment, extensibilitat i separació de responsabilitats.

### 1. Patró State (Estat)

#### Estat del Jugador
**Propòsit**: Gestionar els diferents estats de moviment del jugador (caminar, saltar, pujar escales).

**Implementació**:
```
EstatJugador (Abstract Base)
├─→ EstatCaminant: Moviment horitzontal a terra
├─→ EstatSaltant: Moviment aeri (salt/caiguda)
└─→ EstatPujantBaixantEscales: Moviment vertical en escales
```

**Avantatges**:
- Cada estat gestiona la seva pròpia lògica (input, física, transicions)
- Facilita afegir nous estats sense modificar MovimentsJugadors
- Separació clara entre comportaments (salt vs caiguda amb `aplicarForcaSalt`)

**Classes clau**:
- `EstatJugador.cs`: Classe base abstracta amb `ProcessarInput()`, `ActualitzarFisica()`, `ComprovarTransicions()`
- `EstatCaminant.cs`: Gestiona moviment horitzontal i detecció de caigudes/salts
- `EstatSaltant.cs`: Controla moviment aeri, permet enganxar-se a escales en mig del salt
- `EstatPujantBaixantEscales.cs`: Gestió de moviment vertical, bloqueja si jugador porta objecte

#### Estat de la Partida
**Propòsit**: Gestionar els estats globals del joc (jugant, pausada, finalitzada).

**Implementació**:
```
EstatPartida (Abstract Base)
├─→ EstatJugant: Partida en curs (Time.timeScale = 1)
├─→ EstatPausada: Joc pausat (Time.timeScale = 0)
└─→ EstatFinalitzada: Victòria o derrota
```

**Avantatges**:
- Centralitza la gestió de `Time.timeScale`
- Evita comprovacions booleanes disperses pel codi
- Només permet comprovar victòria en estat Jugant

**Classes clau**:
- `EstatPartida.cs`: Classe base amb mètodes `Pausar()`, `Reprendre()`, `FinalitzarAmbVictoria()`, `FinalitzarAmbDerrota()`
- `GameManager.cs`: Manté `estatActual` i delega operacions als estats
- `ControladorPausa.cs`: Crida `GameManager.Instance.Pausar()/Reprendre()`

### 2. Patró Strategy (Estratègia)

#### Validació d'Escena
**Propòsit**: Permetre diferents estratègies de validació de victòria segons el tipus d'objectes.

**Implementació**:
```
IValidacioEstrategia (Interface)
├─→ ValidacioAttrezzo: Valida objectes sense focus
├─→ ValidacioLlums: Valida focus amb color correcte
└─→ ValidacioVestimenta: Valida que NPCs estan vestits
```

**Avantatges**:
- Facilita afegir noves regles de victòria sense modificar GameManager
- Cada estratègia té responsabilitat única i clara
- Permet combinar múltiples validacions

**Ús a GameManager.cs**:
```csharp
foreach (IValidacioEstrategia estrategia in estrategiesValidacio)
{
    if (!estrategia.Validar()) return; // Victòria falla
}
// Totes les validacions passen → Victòria!
```

#### Sistema de Puntuació amb 3 Estrelles
**Propòsit**: Permetre diferents algoritmes de càlcul d'estrelles.

**Implementació**:
```
ISistemaPuntuacio (Interface)
└─→ Puntuacio3Estrelles: Calcula 0-3 estrelles segons llindars
```

**Avantatges**:
- Facilita implementar nous sistemes de càlcul (percentatges, objectius específics, etc.)
- Separa lògica de càlcul d'estrelles del controlador de puntuació
- Configuració independent de llindars per estratègia

**Ús a ControladorPuntuacio.cs**:
```csharp
estrategiaEstrelles = new Puntuacio3Estrelles(9600, 12600);
numEstrelles = estrategiaEstrelles.CalcularEstrelles(puntuacioFinal, true);
```

### 3. Patró Observer (Observador)

#### Sistema de Puntuació Reactiu
**Propòsit**: Desacoblar el model de puntuació de la UI, actualitzant automàticament la interfície quan canvia la puntuació.

**Implementació**:
```
ModelPuntuacio (Subject)
    ├─→ SubscriureObservador()
    ├─→ DesubscriureObservador()
    └─→ NotificarObservadors()
         ↓
IObservadorPuntuacio (Observer Interface)
    └─→ ActualitzarPuntuacio()
         ↑
ControladorPuntuacio (Concrete Observer)
```

**Avantatges**:
- La UI s'actualitza automàticament quan canvia la puntuació
- Separació clara entre lògica de domini (ModelPuntuacio) i presentació (ControladorPuntuacio)
- Permet afegir múltiples observadors (UI, logs, estadístiques) sense modificar el model

**Flux d'execució**:
1. `ModelPuntuacio.SumarPunts()` → modifica puntuació interna
2. `NotificarObservadors()` → crida `ActualitzarPuntuacio()` de tots els observadors
3. `ControladorPuntuacio.ActualitzarPuntuacio()` → actualitza `puntuacioUI.text`

**Classes clau**:
- `IObservadorPuntuacio.cs`: Interfície amb `ActualitzarPuntuacio(int novaPuntuacio)`
- `ModelPuntuacio.cs`: Model de domini que gestiona puntuació i notifica canvis
- `ControladorPuntuacio.cs`: Implementa IObservadorPuntuacio, subscriu-se al model

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
**Responsabilitat**: Gestor principal del joc amb patró Singleton i patró State per gestionar estats de la partida.

- Persisteix entre escenes amb `DontDestroyOnLoad`
- Gestiona transicions d'escenes amb fade
- Utilitza **patró State** per gestionar estat de la partida (Jugant/Pausada/Finalitzada)
- Utilitza **patró Strategy** per validar condicions de victòria amb múltiples estratègies
- Calcula puntuació final i estrelles
- Inicialitza nivells

**Mètodes clau**: 
- `ComprovarVictoria()`: Valida amb totes les estratègies (ValidacioAttrezzo, ValidacioLlums, ValidacioVestimenta)
- `CanviarEstat()`: Gestiona transicions d'estat amb OnEnter/OnExit
- `Pausar()/Reprendre()`: Delega al estatActual
- `ProcessarVictoria()/ProcessarDerrota()`: Cridats per EstatFinalitzada
- `InicialitzarNivell()`: Inicialitza estat a EstatJugant

### ControladorObjecte.cs
**Responsabilitat**: Gestió de col·locació d'objectes amb sistema de dependències.

- Valida dependències abans de permetre col·locació
- Reprodueix so d'error si falta dependència
- Assigna `sortingOrder = 10` per objecteamb patró Observer per actualitzar UI automàticament i patró Strategy per càlcul d'estrelles.

- Implementa **IObservadorPuntuacio** per rebre notificacions de canvis de puntuació
- Utilitza **ModelPuntuacio** (Subject) per gestionar la lògica de domini
- Utilitza **patró Strategy** amb ISistemaPuntuacio per càlcul d'estrelles flexible
- UI s'actualitza automàticament quan ModelPuntuacio notifica canvis
- Reset selectiu: només reinicia en escenes que comencen amb "Nivell"

**Mètodes clau**: 
- `ActualitzarPuntuacio(int novaPuntuacio)`: Observer, actualitza UI automàticament
- `SumarPunts(int punts)`: Delega a ModelPuntuacio, que notifica canvis
- `CalcularEstrelles()`: Utilitza estratègia Puntuacio3Estrelles
- `CanviarEstrategiaEstrelles()`: Permet canviar algoritme de càlcul en temps d'execució
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
- Persisteix amb `DontDestroyOnLoad(transform.root.gameObject)` amb patró State.

- Utilitza **patró State** per gestionar comportaments de moviment (Caminant/Saltant/PujantEscales)
- Delega processat d'input i física a `estatActual`
- Detecta input segons tag (`Jugador1`: A/D/W/S/E, `Jugador2`: Fletxes/RightShift)
- Estats gestionen transicions automàticament (caiguda, salt, escales)
- Comprova si està a terra amb `Physics2D.OverlapBoxAll()`
- Bloqueja pujada d'escales si aguanta objecte
- Actualitza paràmetres d'Animator: `Horizontal`, `VelocitatY`, `estaAterra`, `estaEscales`, `teObjecte`

**Estats disponibles**:
- `EstatCaminant`: Moviment horitzontal a terra, detecta inici de salt o caiguda
- `EstatSaltant`: Moviment aeri, permet enganxar-se a escales en mig del salt
- `EstatPujantBaixantEscales`: Moviment vertical, bloquejat si porta objecte
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
 amb integració al patró State de GameManager.

- Activa/desactiva panell de pausa
- Delega gestió d'estat a `GameManager.Instance.Pausar()/Reprendre()`
- GameManager gestiona `Time.timeScale` a través del patró State
- Botons: Reprendre, Tornar al Menu, Sortir

**Mètodes clau**: 
- `PausarJoc()`: Crida `GameManager.Pausar()` (canvia a EstatPausada)
- `ReprendreJoc()`: Crida `GameManager.Reprendre()` (torna a EstatJugant)/ `ReprendreJoc()`
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
EstatPartida (State)
│   ├─→ EstatJugant
│   ├─→ EstatPausada
│   └─→ EstatFinalitzada
├─→ IValidacioEstrategia (Strategy)
│   ├─→ ValidacioAttrezzo
│   ├─→ ValidacioLlums
│   └─→ ValidacioVestimenta
├─→ ControladorPuntuacio
├─→ ControladorCrono
└─→ GestorDadesNivells

ControladorPuntuacio (Observer)
├─→ ModelPuntuacio (Subject)
├─→ IObservadorPuntuacio (Interface)
└─→ ISistemaPuntuacio (Strategy)
    └─→ Puntuacio3Estrelles

MovimentsJugadors
├─→ EstatJugador (State)
│   ├─→ EstatCaminant
│   ├─→ EstatSaltant
│   └─→ EstatPujantBaixantEscales
└─→ AgafarObjecte
    └─→ ControladorObjecte

ControladorObjecte
├─→ PuntColocacio
├─→ ControladorFocus
└─→ GameManager

ZonaObjectes
roladorObjecte
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
