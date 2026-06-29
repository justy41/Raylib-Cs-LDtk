using Raylib_cs;

/// <summary>
/// The center of data. You can <b>access everything</b> through here!
/// </summary>
public class Game {
    public SceneManager sceneManager;
    public Dictionary<string, Texture2D> textures;
    
    public TemplateScene templateScene;
    public NextScene nextScene;
    
    public Game() {
        templateScene = new TemplateScene();
        nextScene = new NextScene();
        
        sceneManager = new SceneManager(this);
        sceneManager.AddScene(templateScene);
        sceneManager.AddScene(nextScene);
    }
    
    public void Load() {
        textures = new Dictionary<string, Texture2D> {
            {"pixel.png", Raylib.LoadTexture(Utils.ASSETS_PATH + "pixel.png")},
            {"round_square.png", Raylib.LoadTexture(Utils.ASSETS_PATH + "round_square.png")},
            {"platforms.png", Raylib.LoadTexture(Utils.ASSETS_PATH + "platforms.png")}
        };
        
        sceneManager.GetCurrentScene().Start();
    }
    
    public void Update(float deltaTime) {
        sceneManager.Update(deltaTime);
        sceneManager.GetCurrentScene().beforeUpdate(deltaTime);
        sceneManager.GetCurrentScene().Update(deltaTime);
        sceneManager.GetCurrentScene().afterUpdate(deltaTime);
        
        sceneManager.GetCurrentScene().processDeletion();
    }
    
    public void Draw() {
        sceneManager.GetCurrentScene().Draw();
    }
    
    public Scene GetCurrentScene() {
        return sceneManager.GetCurrentScene();
    }
}