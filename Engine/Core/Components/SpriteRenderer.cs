using System.Numerics;
using Raylib_cs;

public class SpriteRenderer {
    Actor? actor;
    public Color tint;
    public Rectangle sourceRect;
    public int offsetX, offsetY;
    public string textureName;
    
    public SpriteRenderer(Actor actor) {
        textureName = "";
        this.actor = actor;
        tint = Color.White;
        sourceRect = new Rectangle(0, 0, 0, 0);
        offsetX = 0;
        offsetY = 0;
    }
    
    public SpriteRenderer(string textureName, Actor actor, Color tint, int offsetX = 0, int offsetY = 0, int srcX = 0, int srcY = 0, int srcWidth = 0, int srcHeight = 0) {
        this.textureName = textureName;
        this.actor = actor;
        this.tint = tint;
        sourceRect = new Rectangle(srcX, srcY, srcWidth, srcHeight);
        this.offsetX = offsetX;
        this.offsetY = offsetY;
    }
    
    public void Start() {
        
    }

    public void Draw() {
        if(actor != null) {
            if(actor.GetGame() != null) {
                Texture2D tex = tex = actor.GetGame().textures[textureName];
                
                if(sourceRect.Width == 0 || sourceRect.Height == 0) {
                    Raylib.DrawTexturePro(
                        tex,
                        new Rectangle(0, 0, tex.Width, tex.Height),
                        new Rectangle(actor.position.X+offsetX, actor.position.Y+offsetY, tex.Width*actor.scale.X, tex.Height*actor.scale.Y),
                        new Vector2(0, 0),
                        actor.rotation,
                        tint
                    );
                }
                else {
                    Raylib.DrawTexturePro(
                        tex,
                        sourceRect,
                        new Rectangle(actor.position.X+offsetX, actor.position.Y+offsetY, sourceRect.Width*actor.scale.X, sourceRect.Height*actor.scale.Y),
                        new Vector2(0, 0),
                        actor.rotation,
                        tint
                    );
                }
            }
        }
    }
}