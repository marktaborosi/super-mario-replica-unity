# <img src="images/Mario_Small_Idle.png" width="24" alt="Mario Idle Icon"> Super Mario Bros – Unity Recreation (Educational Project)

A non-commercial, educational Unity project recreating core gameplay elements
from **Super Mario Bros. (1985)**.  
This project was developed as part of the **CubixEDU 2025 Unity Game Development Course**,
with the sole purpose of learning, practicing, and understanding 2D platformer mechanics.

Created by **Táborosi Márk**, 2025.

[![Author](https://img.shields.io/badge/author-@marktaborosi-blue.svg)](https://github.com/marktaborosi)
![Version](https://img.shields.io/badge/version-1.0-brightgreen.svg)
![Unity](https://img.shields.io/badge/Unity-6000.2.14f1-black?logo=unity)
![Platforms](https://img.shields.io/badge/Platforms-WebGL%20%7C%20Windows-green.svg)
[![Software License](https://img.shields.io/badge/license-Custom%20Non--Commercial-orange.svg)](LICENSE)
[![Repo](https://img.shields.io/badge/GitHub-super--mario--replica--unity-lightgrey?logo=github)](https://github.com/marktaborosi/super-mario-replica-unity)
![Repo Size](https://img.shields.io/github/repo-size/marktaborosi/super-mario-replica-unity.svg)
![Last Update](https://img.shields.io/github/last-commit/marktaborosi/super-mario-replica-unity.svg)



---

## 🚀 Project Overview

This Unity project is a faithful fan recreation of the original  
**Super Mario Bros** (1985) gameplay loop and mechanics, including:

- 🕹️ Classic 2D platformer movement
- 🍄 Enemy interactions (Goomba, Koopa behavior)
- ⭐ Item pickups
- 🏗️ Level structure and transitions
- 🚩 Flagpole sequence and level completion cinematic
- 🧱 Collision systems and physics behaviour
- 🎮 Scene and state management
- 🧩 Custom scripts implementing original Mario-like control and game feel

The project was built entirely for **educational and learning purposes**,  
following and expanding upon content taught in the **CubixEDU 2025 Unity course**.

This is *not* a remaster, remake, port, or commercial product—  
merely a personal study project.

---

## 🗂️ Scene Hierarchy Example

The repository also includes a visual example of the **Unity scene hierarchy** used to structure a level.  
This illustrates how tiles, blocks, enemies, triggers, background elements and player-related objects
are organized inside a typical 1-1 style scene.

<p align="center">
  <img src="images/scene-hierarchy.png" alt="Scene Hierarchy Example" width="420">
</p>

## 📸 Screenshot

<p align="center">
  <img src="images/screenshot01.png" alt="Gameplay Screenshot" width="640">
</p>

## 🎯 Purpose of the Project

The goal of this project was to:

- Practice Unity 2D workflow
- Learn component-based architecture
- Understand animation states
- Implement flagpole and level-complete sequences
- Improve C# scripting and coroutine usage
- Recreate iconic gameplay mechanics as a learning challenge

This project serves as a technical practice exercise and educational study.

---

## 📁 Repository Structure

The repository includes the essential Unity project folders:
- `Assets/`
- `ProjectSettings/`
- `Packages/`

Additionally, the repo contains **two build outputs**:

- 🌐 `Build/Web/` — WebGL playable build
- 🖥️ `Build/Bin/` — Windows standalone build

Unity-generated folders such as:

- `Library/`
- `Temp/`
- `Obj/`
- `Logs/`
- `UserSettings/`


are intentionally excluded.

---

## ⚠️ Legal Disclaimer

This project is a **fan-made, educational recreation** of  
**Super Mario Bros. (1985)**.

- All characters, sprites, audio, and intellectual property belong to  
  **Nintendo Co., Ltd.**
- This project is **not affiliated with, endorsed by, or associated with Nintendo**.
- The project is strictly **non-commercial**, intended only for learning.

Any Nintendo-originating visual or audio assets included in the project remain  
their exclusive property.

All other components, scripts, and Unity logic were created by  
**Táborosi Márk**.

---

## 📦 Third-Party Assets

This project uses the following **paid Unity Asset Store** tool:

- **vHierarchy 2** by *Kubacho Labs*  
  (Used exclusively for enhancing the Unity Editor hierarchy.)

The asset itself **is NOT included** in this repository,  
as redistribution of paid Asset Store content is prohibited.

---

## ▶️ How to Run the Project

### Opening the project in Unity:
1. Clone or download the repository
2. Open **Unity Hub**
3. Click **Add project**
4. Select the root project folder
5. Open using **Unity 6000.2.14f1**
6. Press ▶ **Play** in the editor

### Running the builds:
- 🖥️ Run `Build/Bin/*.exe` for the Windows build
- 🌐 Run `Build/Web/` on a Web Server

WebGL builds must be served via an HTTP server. Opening the index file directly from the file system will not work.
For web server you can use python, node.js, php, whatever you prefer...

---

## 🖥️ Display & Aspect Ratio Support

The project supports a wide range of display resolutions and aspect ratios.  
It has been primarily tested on:

- ✔️ **16:9** (Full HD, QHD, 4K UHD)
- ✔️ **16:10**
- ✔️ **WXGA / HD+** formats
- ✔️ **Full HD (1920×1080)**
- ✔️ **QHD (2560×1440)**
- ✔️ **4K UHD (3840×2160)**

Unity's **Free Aspect** mode is fully supported as well —  
the side-scrolling camera and clamped player movement do not depend on a fixed aspect ratio,  
so gameplay remains stable across standard and high-resolution displays.

---

## ⚡ Performance & Framerate

The game is configured to run at a **target framerate of 100 FPS**  
(as set in `GameManager.Start()` via `Application.targetFrameRate = 100`).

This ensures:

- Smooth camera scrolling
- Responsive player input
- Stable physics behaviour at modern refresh rates

Unity will internally cap the framerate if the platform or browser cannot reach 100 FPS,
but the gameplay remains consistent even at lower framerates.

## 🎮 Gameplay Scope & Limitations

This project intentionally focuses on a **small, self-contained slice** of the original game:

- Contains **only the first level** (World 1-1)
- Game state (world, stage, lives, coins) exists **only in memory**
- There is **no persistence layer**: no save/load, no profile system
- There is **no high-score table** or any stored stats between sessions

Every time you start the game, it always begins from the default initial state.

The goal is to demonstrate core gameplay, control and system design in a compact form,
not to recreate the full commercial feature set of Super Mario Bros.

---

## 🧠 Implementation Overview

Under the hood, the project is built around a few simple core ideas:

- **Global singletons for shared state**  
  `GameManager`, `SoundManager` and `UI` are persistent singletons:
  - `GameManager` controls world/stage, lives, coins, level loading, resets and pause
  - `SoundManager` subscribes to game events and plays music/SFX for player, blocks, pipes, flagpole, enemies, etc.
  - `UI` shows coins, lives and world–stage, and reacts to pause events

- **Custom player controller and animation layer**  
  The player uses a hand-rolled movement system (`PlayerMovement`) based on its own velocity and gravity
  instead of Unity physics forces.  
  It handles acceleration/deceleration, running, jumping and fall behaviour, as well as simple collision responses.
  `PlayerController` manages small/big state, hits, death and star power, while `PlayerSpriteRenderer` and
  `AnimatedSprite` map those states to sprites and animations (idle, run, jump, slide, flagpole, cinematic).

- **Level interaction & game flow objects**  
  A small set of components encapsulate classic Mario interactions:
  - `BlockHit`, `BlockItem`, `BlockCoin` and `BlockBreakEffect` handle question blocks, breakable bricks,
    popping coins, spawned items and debris that can even damage enemies.
  - `PowerUp` defines what happens when the player collects a coin, 1-up, mushroom or star.
  - `Pipe` animates going into and coming out of pipes (including underground/overworld transitions and camera height changes).
  - `FlagPole` runs the end-of-level sequence (slide down, walk into the castle, then end/quit the game).
  - `DeathBarrier` catches anything that falls off the level and restarts the stage if it’s the player.

- **Simple, reusable enemy and movement logic**  
  A base `Enemy` class plus `EntityMovement` provide shared behaviour for enemies.  
  Specific enemies like `Goomba` and `Koopa` build on top of this to implement stomping, flattening, shell state and
  moving shells as hazards, closely mimicking the original game’s behaviour while keeping the code readable.

- **Juice & feedback through lightweight animation scripts**  
  Small scripts like `CoinAnimation`, `AnimatedSprite` and `DeathAnimation` add simple but effective
  visual feedback: pulsing coins, sprite-based run cycles, jump-and-fall death arcs and block debris explosions.

Overall, the architecture aims to be **clean and educational**:  
global managers for shared state, events for decoupling, and many small focused components
for movement, interaction, audio and visual feedback.

---

## 🔮 Future Improvements

This project intentionally focuses on a single, self-contained level (1-1),  
but it was built with modular systems that allow extending the game further.  
Possible future additions include:

### 🎮 Gameplay & Content
- Additional levels (1-2, 1-3, 1-4, and full world progression)
- Underground, castle and sky themes
- More enemy types (Hammer Bro, Piranha Plant, Lakitu, etc.)
- Improved collision system and movement refinements

### 💾 Systems & Persistence
- Save/load support for:
  - player progress
  - high scores
  - collected coins
  - continue from last stage
- Configurable input bindings or settings menu

### 📱 Platform Support
- Mobile touch controls (virtual joystick + jump/run buttons)
- Gamepad support (Xbox / PlayStation controllers)
- WebGL performance tuning for mobile browsers

### 🧩 Tools & Editor
- Level editor improvements (prefab palettes, snapping, gizmos)
- More reusable scene templates for level-building

These features are **not implemented in this version**,  
but the codebase is structured to make such extensions possible in future iterations.

---

## 🧩 Unity Version

This project was created using:

**Unity 6000.2.14f1**

Using other Unity versions may produce import warnings or unexpected behaviour.

---

## 🙋 Author

**Mark Taborosi**  
Cubix Institue of Technology - Unity Game Development — 2025  
Email: <a href="mailto:mark.taborosi@gmail.com">mark.taborosi@gmail.com</a>

---

## 📝 License

This repository uses a **custom non-commercial, no-redistribution license**  
designed for educational fan projects.

Please refer to the `LICENSE` file for full details.



