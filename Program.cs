using System.Numerics;
using Raylib_cs;
using rlImGui_cs;
using ImGuiNET;

namespace HelloWorld;

/*
    The starting point of the application.
    The raylib window is configured and the Game is started, updated and drawn here together with important values like scale or deltaTime.
    
    The games resolution is currently set up in a letter box format (with black bars on the side).
    It works by drawing everything to a RenderTexture2D and then that texture is scaled up to always keep a specified resolution based on the scale value.
    
    Also raylib cameras are supported, every Scene has a camera.
    World mouse coordinates can be obtained by using the specific Scene function worldMousePosition().
    
    The Editor was experimental. The Entity editor in LDtk should be used instead!
    Check out Utils.cs !!!
*/

internal static class Program
{
    // STAThread is required if you deploy using NativeAOT on Windows - See https://github.com/raylib-cs/raylib-cs/issues/301
    [System.STAThread]
    public static void Main() {
        // SETUP ==============================================================================================================
        Raylib.SetConfigFlags(ConfigFlags.ResizableWindow);
        Raylib.InitWindow(Utils.screenWidth, Utils.screenHeight, "Game Name :3");

        float deltaTime = 0;
        float scale = 0;
        RenderTexture2D target = Raylib.LoadRenderTexture(Utils.gameScreenWidth, Utils.gameScreenHeight);
        Raylib.SetTextureFilter(target.Texture, TextureFilter.Point);
        // ====================================================================================================================
        
        Game game = new Game();
        game.Load();
        
        Editor editor = new Editor(game);
        editor.Load();
        
        while (!Raylib.WindowShouldClose()) {
            deltaTime = Raylib.GetFrameTime();
            scale = MathF.Min((float)Raylib.GetScreenWidth()/Utils.gameScreenWidth, (float)Raylib.GetScreenHeight()/Utils.gameScreenHeight);
            Utils.scale = scale;
            
            game.Update(deltaTime);
            
            if(game.GetCurrentScene().debug)
                editor.Update(deltaTime);
            else {
                if(editor.selectedEntity != null) {}
                    // editor.selectedEntity.selected = false;
            }
            
            
            
            // DRAWING
            
            
            
            
            Raylib.BeginTextureMode(target);
            Raylib.BeginMode2D(game.sceneManager.GetCurrentScene().camera);
                Raylib.ClearBackground(Color.White);
                game.Draw();
            Raylib.EndMode2D();
            Raylib.EndTextureMode();
            
            
            
            
            // DRAWING
            
            
            
            
            Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.Black);

                // Draw render texture to screen, properly scaled
                Raylib.DrawTexturePro(target.Texture, new Rectangle(0.0f, 0.0f, (float)target.Texture.Width, (float)-target.Texture.Height),
                            new Rectangle((Raylib.GetScreenWidth() - ((float)Utils.gameScreenWidth*scale))*0.5f, (Raylib.GetScreenHeight() - ((float)Utils.gameScreenHeight*scale))*0.5f,
                            (float)Utils.gameScreenWidth*scale, (float)Utils.gameScreenHeight*scale), new Vector2(0, 0), 0.0f, Color.White);
                
                if(game.GetCurrentScene().debug)
                    editor.Draw();
            Raylib.EndDrawing();
        }

        rlImGui.Shutdown();
        Raylib.CloseWindow();
    }
}