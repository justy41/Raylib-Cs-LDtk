using Raylib_cs;

public class Platform : Solid {
    SpriteRenderer renderer;
    public string textureName;
    
    public Platform(string name, string textureName, int srcX, int srcY, int srcWidth, int srcHeight) {
        this.name = name;
        this.textureName = textureName;
        renderer = new SpriteRenderer(textureName, this, Color.White, 0, 0, srcX, srcY, srcWidth, srcHeight);
        rect.Width = srcWidth;
        rect.Height = srcHeight;
    }

    public override void Start() {
        base.Start();
        
        renderer.Start();
    }

    public override void Draw() {
        base.Draw();
        
        renderer.Draw();
    }
}