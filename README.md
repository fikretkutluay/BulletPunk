# 🔫 BulletPunk

**BulletPunk** is a fast-paced, top-down action game developed for **Sandwich Jam 2**. The game was built around the theme **"Losing Control,"** where the player's core mechanics are tied to their health and stability. Unlike traditional shooters, BulletPunk challenges players to survive as they progressively lose their ability to move and fight while under fire.

## 🎮 Jam Theme: "Losing Control"
In line with the jam theme, our character's control scheme is dynamic and fragile. As you take damage from enemies, you progressively **lose keyboard controls**. This mechanic turns every hit into a strategic crisis, forcing the player to adapt to a shrinking set of available inputs while trying to eliminate threats.

## 🕹 Core Features
* **Theme-Driven Controls:** A unique "Control Decay" system where keyboard inputs are disabled based on damage received.
* **Intense Top-Down Combat:** Fast-paced arena survival mechanics.
* **Universal Render Pipeline (URP):** Optimized 2D visuals with modern post-processing and lighting.
* **Interface-Based Interaction:** Robust health and damage systems using decoupled C# interfaces.

## 🛠 Technical Stack
* **Engine:** Unity 2022.3 (LTS)
* **Render Pipeline:** Universal Render Pipeline (URP)
* **Scripting:** C# (.NET Standard)
* **Input System:** Unity Input System (optimized for Windows)
* **Platform:** Windows PC (Build available)

## 📁 Project Structure
The repository follows a modular structure focused on the Jam's core mechanics:
```text
BulletPunk/
├── Assets/
│   ├── Scripts/
│   │   ├── _Core/      # Enums and core game constants
│   │   ├── Interfaces/ # IDamagable and IDebuffable systems
│   │   └── Managers/   # Input and Game state management
│   └── Settings/       # URP profiles and renderer settings

## 🚀 Key Technical Highlights
* Dynamic Input Decay
*The project leverages the Unity Input System to manage player controls. I implemented a custom logic that listens to damage events through the **IDamagable** interface. When certain damage thresholds are met, the **InputManager** dynamically disables specific action maps, directly simulating the "Losing Control" theme.

*Scalable Damage System
* By using **IDamagable** and **IDebuffable**, the combat system is completely decoupled. This allowed us to quickly iterate during the jam, adding new enemy types or environmental hazards that interact with the player's health and control state without breaking the core codebase.
