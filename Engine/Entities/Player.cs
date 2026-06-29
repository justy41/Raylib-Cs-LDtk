using System.Numerics;
using Raylib_cs;

public class Player : Actor {
    SpriteRenderer renderer;
    Rectangle groundCheck;
    
    public float speed, jumpForce, gravity, velocityY;
    public string texturePath;
    public bool isGrounded;
    
    public Player(string name, string texturePath) {
        this.name = name;
        position = new Vector2(200, 200);
        rect = new Rectangle(position.X, position.Y, 16, 16);
        groundCheck = new Rectangle(position.X+1, position.Y+rect.Height, rect.Width-2, 5);
        
        speed = 100;
        jumpForce = 190;
        gravity = 340;
        velocityY = 0;
        
        renderer = new SpriteRenderer(texturePath, this, Color.Black, 0, 0, 0, 0, 16, 16);
    }

    public override void Start() {
        base.Start();
        
        renderer.Start();
    }
    public override void Update(float deltaTime) {
        base.Update(deltaTime);
        groundCheck.X = position.X+1; groundCheck.Y = position.Y+rect.Height;
        
        isGrounded = collideAt(scene!.solids, groundCheck) >= 0; // Probably pretty expensive because it loops all the solids in the scene
        
        velocityY += gravity * deltaTime;
        moveY(velocityY*deltaTime);
        
        if(Raylib.IsKeyDown(KeyboardKey.Right)) {
            moveX(speed*deltaTime);
        }
        else if(Raylib.IsKeyDown(KeyboardKey.Left)) {
            moveX(-speed*deltaTime);
        }
        
        if(Raylib.IsKeyPressed(KeyboardKey.Z) && isGrounded) {
            velocityY = -jumpForce;
        }
        
        if(Raylib.IsKeyReleased(KeyboardKey.Z)) {
            if(velocityY < 0) {
                velocityY /= 3;
            }
        }
    }

    public override void Draw() {
        base.Draw();
        
        renderer.Draw();
        Raylib.DrawRectangleLinesEx(groundCheck, 1, Color.Green);
    }

    public override void onCollideY(Solid solid) {
        base.onCollideY(solid);
        velocityY = 0;
    }
}