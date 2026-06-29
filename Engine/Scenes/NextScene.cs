using Raylib_cs;

// TODO: Tidy up the whole template, put on Github

/// <summary>
/// This scene is for showing how to switch scenes between them.
/// </summary>
public class NextScene : Scene {
    LdtkWorld? ldtkWorld;
    
    public override void Start() {        
        ldtkWorld = new LdtkWorld(Utils.DATA_PATH + "Levels/next_level.ldtk");
        ldtkWorld.LoadTilesetTextures();
        LoadCollisionLayer(ldtkWorld);
        factory.Load(this, ldtkWorld);
        
        base.Start();
    }

    public override void Update(float deltaTime) {
        base.Update(deltaTime);
        
        if(Raylib.IsKeyPressed(KeyboardKey.Minus)) {
            if(sceneManager != null)
                sceneManager.SwitchScene(0);
        }
    }

    public override void Draw() {
        if(ldtkWorld != null)
            ldtkWorld.DrawWorld();
        
        base.Draw();
    }
}