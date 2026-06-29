using Raylib_cs;

// TODO: Put on Github

/// <summary>
/// <para>An example of how a scene should be generally made.
/// Starts with a LdtkWorld object that needs to be loaded in Start(). You can use <tt>Utils.DATA_PATH</tt> (set it up first).</para>
/// <para><tt>LoadTilesetTextures()</tt>.</para>
/// <para><tt>LoadCollisionLayer()</tt>.</para>
/// <para>Call <tt>factory.Load(this, ldtkWorld)</tt>.</para>
/// <para>-</para>
/// Call LdtkWorld.DrawWorld() to draw the whole world with all levels.
/// </summary>
public class TemplateScene : Scene {
    LdtkWorld? ldtkWorld;
    
    public override void Start() {        
        ldtkWorld = new LdtkWorld(Utils.DATA_PATH + "Levels/template_level.ldtk");
        ldtkWorld.LoadTilesetTextures();
        LoadCollisionLayer(ldtkWorld);
        factory.Load(this, ldtkWorld);
        
        base.Start();
    }

    public override void Update(float deltaTime) {
        base.Update(deltaTime);
        
        if(Raylib.IsKeyPressed(KeyboardKey.Minus)) {
            if(sceneManager != null)
                sceneManager.SwitchScene(1);
        }
    }

    public override void Draw() {
        if(ldtkWorld != null)
            ldtkWorld.DrawWorld();
        
        base.Draw();
    }
}