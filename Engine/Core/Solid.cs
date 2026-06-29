using Raylib_cs;

/// <summary>
/// Class that represents a static Actor.
/// <para>Should be moved only with the respective <tt>moveSolid()</tt> function, <b>NOT</b> the <tt>moveX()</tt> and <tt>moveY()</tt>.</para>
/// </summary>
public class Solid : Actor {
    
    public bool collidable = true;
    
    // ======================== SOLID LOGIC ======================== //
    public void moveSolid(float amountX, float amountY) {
        if(scene == null) return;
        
        xRemainder += amountX;
        yRemainder += amountY;
        int moveX = (int)MathF.Round(xRemainder);
        int moveY = (int)MathF.Round(yRemainder);
        if(moveX != 0 || moveY != 0) {
            List<Actor> riding = getAllRidingActors();
            
            collidable = false;
            
            if(moveX != 0) {
                xRemainder -= moveX;
                position.X += moveX;
                rect.X = (int)position.X;
                if(moveX > 0) {
                    foreach(Actor actor in scene.actors) {
                        if(overlapCheck(actor)) {
                            actor.moveX(rect.X + rect.Width - actor.rect.X);
                        }
                        else if(riding.Contains(actor)) {
                            actor.moveX(moveX);
                        }
                    }
                }
                else {
                    foreach(Actor actor in scene.actors) {
                        if(overlapCheck(actor)) {
                            actor.moveX(rect.X - actor.rect.X + actor.rect.Width);
                        }
                        else if(riding.Contains(actor)) {
                            actor.moveX(moveX);
                        }
                    }
                }
            }
            
            if(moveY != 0) {
                yRemainder -= moveY;
                position.Y += moveY;
                rect.Y = (int)position.Y;
                if(moveY > 0) {
                    foreach(Actor actor in scene.actors) {
                        if(overlapCheck(actor)) {
                            actor.moveY(rect.Y + rect.Height - actor.rect.Y);
                        }
                        else if(riding.Contains(actor)) {
                            actor.moveY(moveY);
                        }
                    }
                }
                else {
                    foreach(Actor actor in scene.actors) {
                        if(overlapCheck(actor)) {
                            actor.moveY(rect.Y - actor.rect.Y + actor.rect.Height);
                        }
                        else if(riding.Contains(actor)) {
                            actor.moveX(moveY);
                        }
                    }
                }
            }
            
            collidable = true;
        }
    }
    
    public bool overlapCheck(Actor actor) {
        return Raylib.CheckCollisionRecs(rect, actor.rect);
    }
    
    public List<Actor> getAllRidingActors() {
        if(scene == null) return new List<Actor>();
        
        List<Actor> result = new List<Actor>();
        foreach(Actor actor in scene.actors) {
            if(actor.isRiding(this)) {
                result.Add(actor);
            }
        }
        
        return result;
    }
}