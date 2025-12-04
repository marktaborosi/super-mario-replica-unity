# Contributing Guidelines

Thank you for considering contributing to the **Super Mario Bros – Unity Recreation** project!  
This project is primarily an educational exercise, but well-structured contributions are welcome.

---

## 🎮 Project Goals

- Faithfully recreate key gameplay elements from *Super Mario Bros (1985)*
- Focus on clarity, readability and educational value
- Use clean, modular, component-based Unity architecture
- Keep the project strictly **non-commercial**

---

## 🧱 How to Contribute

### 1. Fork the Repository
Create your own fork and work in a feature branch:

`git checkout -b feature/your-feature-name`


### 2. Follow Code Style Guidelines

#### **C# / Unity coding conventions**
- Use PascalCase for classes, public fields and methods
- Use camelCase for private fields
- Prefer `[SerializeField] private` over public fields
- Avoid large monolithic scripts — keep components focused
- Use Unity events or clean messaging instead of direct cross-references
- Avoid magic numbers; use constants or ScriptableObjects when possible

#### **Scene / Prefab guidelines**
- Scene hierarchy should remain clean and organized
- Use prefabs for repeatable objects (enemies, blocks, items)
- Avoid editing `ProjectSettings` unnecessarily
- Do not include temporary test scenes in commits

#### **Folders**
Use this structure:

- Assets/
- Scripts/
- Sprites/
- Audio/
- Prefabs/
- Scenes/
- UI/


---

## 🔄 Pull Request Process

1. Ensure your branch is up to date with `main`
2. Provide a clear PR title and description
3. Include screenshots or short GIFs for gameplay changes
4. Reference related issue numbers (if any)
5. Ensure the project opens cleanly in **Unity 6000.2.14f1**
6. Remove any debug logs, unused assets or leftover test code
7. Submit the PR

The maintainer will review your changes as soon as possible.

---

## 🧪 Testing

Before submitting:
- Play through World 1-1 at least once
- Test both:
    - **Windows build**
    - **WebGL build** (via HTTP server)

Ensure no new:
- console errors
- warnings
- broken animations
- missing references

---

## ⚠️ Restrictions

To keep the project educational and legally compliant:

❌ **Do NOT submit:**
- Original Nintendo assets not already included
- Complete game ports or copyrighted levels
- Commercial monetization features
- Online services or multiplayer
- Paid Unity Asset Store files

✔ **Allowed:**
- Custom scripts
- Custom sprites you created
- Code improvements
- Educational examples
- New modular systems (collision, items, UI, etc.)

---

## 🙏 Thanks

Your contribution helps expand this educational Unity project and keeps it a high-quality reference for new developers.  
Thank you for taking the time to follow these guidelines!
