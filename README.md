# Raylib-Cs-LDtk
 
An **Actor-based game engine template** for C# built on top of [raylib-cs](https://github.com/raylib-cs/raylib-cs), with integrated support for the [LDtk](https://ldtk.io/) level editor and an optional ImGui-powered in-engine editor.
 
## Features
 
- **Actor-based architecture** — organize your game logic around reusable Actor entities managed through Scenes
- **LDtk integration** — load and use levels designed in the LDtk level editor directly in your game
- **Letterbox rendering** — all rendering targets a fixed internal resolution (`gameScreenWidth` × `gameScreenHeight`) and is scaled up to fill any window size, with black bars on the sides to preserve aspect ratio
- **Raylib camera support** — every Scene has its own 2D camera; world-space mouse coordinates are available via `Scene.worldMousePosition()`
- **ImGui editor** — an experimental in-engine entity editor powered by `rlImGui-cs` and `ImGui.NET`, toggled per-scene via a `debug` flag
- **Resizable window** — the window scales dynamically while maintaining the target internal resolution
- **.NET 9 / NativeAOT ready** — targets `net9.0` with unsafe blocks enabled; NativeAOT deployment on Windows is supported (requires `[STAThread]`)
 
## Project structure
 
```
Raylib-Cs-LDtk/
├── Engine/              # Core engine classes (Actors, Scenes, SceneManager, Utils, Editor, etc.)
├── Program.cs           # Entry point — window setup, game loop, rendering pipeline
├── RaylibCsTemplate_v2.csproj
└── imgui.ini            # ImGui layout configuration
```

## Videos:

https://github.com/justy41/Raylib-Cs-LDtk/blob/41c5082650a5de9709d0d548da416c19c18afe12/Media/RaylibCsLDtk.mp4

https://github.com/justy41/Raylib-Cs-LDtk/blob/41c5082650a5de9709d0d548da416c19c18afe12/Media/RaylibCsLDtk_ldtk.mp4

## Dependencies
 
| Package | Version |
|---|---|
| [Raylib-cs](https://www.nuget.org/packages/Raylib-cs) | 8.0.0 |
| [ImGui.NET](https://www.nuget.org/packages/ImGui.NET) | 1.91.6.1 |
 
`rlImGui-cs` is used to bridge ImGui with Raylib for the in-engine editor overlay.
 
## Getting started
 
### Prerequisites
 
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [LDtk](https://ldtk.io/) (for designing levels)
### Clone & Run
 
```bash
git clone https://github.com/justy41/Raylib-Cs-LDtk.git
cd Raylib-Cs-LDtk
dotnet run
```
 
## How it works
 
### Rendering pipeline
 
The game renders to a `RenderTexture2D` at the fixed internal resolution, then scales that texture to fill the screen using `DrawTexturePro`. This keeps pixel art crisp and the layout consistent across screen sizes.
 
```
[Game Logic] → [RenderTexture2D at gameScreenWidth×gameScreenHeight]
                        ↓
           [Scaled to screen with letterboxing]
                        ↓
              [Optional ImGui Editor Overlay]
```
 
### Scenes & Cameras
 
Each Scene holds its own Raylib `Camera2D`. To get the mouse position in world space (accounting for camera transform and scale) use:
 
```csharp
game.GetCurrentScene().worldMousePosition();
```
 
### Debug/Editor mode
 
The ImGui editor overlay is activated per-scene via the `debug` flag:
 
```csharp
myScene.debug = true;
```
 
> **Note:** The built-in entity editor is experimental. It's recommended to use the **Entity Editor in LDtk** for defining and configuring entities.
 
## Tips
 
- Check `Utils.cs` for helpful constants and utility methods used throughout the engine.
- `Utils.scale` is updated every frame and reflects the current letterbox scale factor — useful for UI positioning.
- For NativeAOT deployment on Windows, the `[STAThread]` attribute on `Main()` is required (see [raylib-cs #301](https://github.com/raylib-cs/raylib-cs/issues/301)).
 
## License
 
This project is licensed under the [MIT License](LICENSE).  
© 2026 Babiciu Iustin
