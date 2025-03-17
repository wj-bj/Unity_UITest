# UITest
The project is to construct UI for testing the development UI system with Unity 6
## Video Demonstration
* The video demonstration is available to view [here](https://youtu.be/5JS3QZU3JSY).
## Introduction
<p align="center">
    <img src="pics/flow.png" height="586" width="812">
</p>

This project consisof three UI panel: the UI for Main screen, Setting enviroment and Level complete. Each UI panel is linked by pressing the specific button which is indicated on the figure.
**Home Screen UI** – The main interface containing navigation elements with bar animation via scripts.

**Settings Popup UI** – A modular settings panel designed for easy expansion supporint localization function.

**Level Completed UI** – A visually dynamic completion screen with animations and effects

The implemented functions are included as the following
* Button pressing animation implemented by scripts.
* UI pop-up animation, transition and blur effect.
* UI management system for ease of maintenance across different screens.
* Localization structured for multi-language adaptation 
* Some vfx, animation and shader for dynamic effect.

## Technique
Features in the project are list as follows:

**Cellular Automata**: Generate density map and decoration map, which are required for 3D Terrain and the vegetation on it.

**Marching Cubes** : Generate triangular meshed according to the voxel density.

**Force-based steering**: Controll the movement of Game objects, such as seek, flee, path following, avoid collision.

**Behavior Tree** : Controll AI behavior

**Utility agent**: Controll AI logic

**Compute Shader**: Put the most time-consuming calculation on GPU side, such as mesh generation, fish animation

**Command Buffer**: To postprocess the render from camera, adding special effect like screen-based depth and distortion to simulate the undersea effect.

## Setup
* This project was developed on Unity version `Unity 6`.
* The scene file is at `Assets/Scenes/UITest.unity`

## Project hierarchy
```commandline
📦Assets
 ┣ 📂Scenes                         // Main Scene
 ┃ ┗ 📜UITest.unity                   
 ┣ 📂Scripts                        // Main scripts for UI and Utility
 ┃ ┣ 📂Core                         // some UI eventtrigger
 ┃ ┣ 📂Localization                 // text localization system
 ┃ ┣ 📂UI                           // UI management, UIPanel and transition animation related scripts
 ┃ ┣ 📂Utility                      // scripts for using in editor to test 
 ┣ 📂Art                            // source from art side, include ui texture, video
 ┣ 📂AssetBoundles                  // assets created in Unity editor, such as prefab, anmiation, etc.
 ┣ 📂Shaders                        // shaders for ui, some special effects
 ┣ 📂Resource                       // resource files, like json file localization
 ┣ 📂Plugins                        // helpfuf asset store sources.
 ┗ ┗ 📂Dotween                      // animation system
 ```

## Future Improvements
* There is no sound system playing the sfx for the UI events
* Currently, window animations are implemented using inheritance from a base WindowUI, which includes foreground and background animations along with feedback. This could be refactored into a component-based system (IComponent), making it easier to maintain and extend.
* All UI elements are instantiated and hidden in the scene by default. Instead, an asynchronous system should be implemented to dynamically instantiate them as needed.
* A GameEvent system is currently missing and should be introduced to improve event handling and communication between UI elements.
* There is no event disregister in the implmented as there is no formal Management to create and destroy UI in order.
* There is no event deregistration in the implementation, as there is no formal management system to properly create and destroy UI elements in right order
There is a loading UI and a gradient UI for the transition and background system, but they are not funtional, only added here as a demonstration of expandability.

## Reference
* Dotween for GameObject animation