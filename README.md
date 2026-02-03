# 🔫 BulletPunk

**BulletPunk** is a fast-paced, top-down action game developed with **Unity**. Built on a modular architecture, it focuses on intense combat mechanics, fluid character movement, and scalable game systems. The project serves as a robust foundation for arcade-style shooters, emphasizing clean code and performance.

## 🎮 Core Features
* **Dynamic Combat System:** Implementation of health and damage systems through robust interfaces.
* **Advanced Input Handling:** Integrated with the **Unity Input System**, supporting seamless cross-platform controls (Keyboard/Mouse & Gamepad).
* **Modular Architecture:** Designed with scalability in mind, using decoupled systems for managers and core gameplay logic.
* **High-Fidelity Visuals:** Developed using the **Universal Render Pipeline (URP)** for optimized 2D/3D performance and modern post-processing effects.

## 🛠 Technical Stack
* **Engine:** Unity 2022+ (LTS)
* **Render Pipeline:** Universal Render Pipeline (URP)
* **Scripting:** C# (.NET Standard)
* **Input:** Unity Input System package

## 📁 Project Structure
The repository is organized following clean-code principles to ensure maintainability:
```text
BulletPunk/
├── Assets/
│   ├── Scripts/
│   │   ├── _Core/      # Enums, constants, and base systems
│   │   ├── Interfaces/ # IDamagable, IDebuffable, etc.
│   │   └── Managers/   # InputManager and core game controllers
│   ├── Settings/       # URP profiles and Scene templates
│   └── Scenes/         # Main gameplay and testing environments
