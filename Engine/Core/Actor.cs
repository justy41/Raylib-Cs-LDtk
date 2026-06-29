using System.Numerics;
using Raylib_cs;

/// <summary>
/// Class that represents a moving object. Sort of like a Kinematic body in other game engines.
/// <para>This is the class you want to inherit from when making a new object (ex. like Player or Enemy).</para>
/// <para>The object should be moved with the <tt>moveX()</tt> or <tt>moveY()</tt> functions.</para>
/// <para>Contains basic attributes like position, scale, rectangle (bounding box AABB style) and the a separate vector for its size.</para>
/// 
/// <para>Inspired by the Entity model from Celeste by EXOK Games.</para>
/// </summary>
public class Actor {
    
    public Scene? scene;
    public Vector2 position = new Vector2(0, 0);
    public Vector2 scale = new Vector2(1, 1);
    public Vector2 rectSize = new Vector2(0, 0);
    public Rectangle rect;
    
    public int touchedSolid;
    public int sameNameCount;
    public float rotation;
    public float xRemainder;
    public float yRemainder;
    public string name = "";
    public bool enabled = true;
    public bool selected = false;
    
    public virtual void Start() {
        rect.X = position.X;
        rect.Y = position.Y;
        rectSize = new Vector2(rect.Width, rect.Height);
    }
    
    public virtual void Update(float deltaTime) {
        if(selected) {
            rect.X = (int)position.X;
            rect.Y = (int)position.Y;
            rect.Width = rectSize.X;
            rect.Height = rectSize.Y;
        }
    }
    
    public virtual void LateUpdate(float deltaTime) {
        // Stuff...
    }
    
    public virtual void Draw() {
        
    }
    
    // ======================== ACTOR LOGIC ======================== //
    public void moveX(float amount) {
        if(scene == null) return;
        
        xRemainder += amount;
        int move = (int)MathF.Round(xRemainder);
        if(move != 0) {
            xRemainder -= move;
            int sign = MathF.Sign(move);
            while(move != 0) {
                touchedSolid = collideAt(scene.solids, new Rectangle(rect.X + sign, rect.Y, rect.Width, rect.Height));
                if(touchedSolid < 0 || !scene.solids[touchedSolid].collidable) {
                    position.X += sign;
                    move -= sign;
                }
                else if(scene.solids[touchedSolid].collidable) {
                    onCollideX(scene.solids[touchedSolid]);
                    break;
                }
                
                rect.X = (int)position.X;
            }
        }
    }
    
    public void moveY(float amount) {
        if(scene == null) return;
        
        yRemainder += amount;
        int move = (int)MathF.Round(yRemainder);
        if(move != 0) {
            yRemainder -= move;
            int sign = MathF.Sign(move);
            while(move != 0) {
                touchedSolid = collideAt(scene.solids, new Rectangle(rect.X, rect.Y + sign, rect.Width, rect.Height));
                if(touchedSolid < 0 || !scene.solids[touchedSolid].collidable) {
                    position.Y += sign;
                    move -= sign;
                }
                else if(scene.solids[touchedSolid].collidable){
                    onCollideY(scene.solids[touchedSolid]);
                    break;
                }
                
                rect.Y = (int)position.Y;
            }
        }
    }
    
    public bool collideAt(List<Solid> allSolids, Vector2 position) {
        foreach(var sol in allSolids) {
            if(!sol.enabled) continue;
            if(Raylib.CheckCollisionPointRec(position, sol.rect)) {
                return true;
            }
        }
        
        return false;
    }
    
    /// <summary>
    /// Returns the index of the Solid this Actor is colliding with.
    /// If the this Actor isn't colliding with anything, return -1.
    /// </summary>
    /// <param name="allSolids"></param>
    /// <param name="r"></param>
    /// <returns></returns>
    public int collideAt(List<Solid> allSolids, Rectangle r) {
        for(int i = 0; i<allSolids.Count; i++) {
            if(!allSolids[i].enabled) continue;
            if(Raylib.CheckCollisionRecs(allSolids[i].rect, r)) {
                return i;
            }
        }
        
        return -1;
    }
    
    public virtual bool isRiding(Solid solid) {
        if(touchedSolid >= 0 && (rect.Y + rect.Height) <= solid.rect.Y) {
            return true;
        }
        
        return false;
    }
    
    public virtual void squish() {
        
    }
    
    public virtual void onCollideX(Solid solid) {
        
    }
    
    public virtual void onCollideY(Solid solid) {
        
    }
    
    public Game? GetGame() {
        if(scene != null && scene.sceneManager != null)
            return scene.sceneManager.game;
        return null;
    }
}