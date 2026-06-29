using System.Numerics;
using QuickType;

/// <summary>
/// Class that talks to LdtkWorld.cs and Scene.cs to instantiate object types (like Player or Platform objects etc...)
/// <para>The design pattern consists of making a function that creates the object you want.</para>
/// <para>Then call that function in one of the two <tt>switch</tt> statemens in the <tt>Load()</tt> function.</para>
/// </summary>
public class EntityFactory {
    public Scene scene;
    public LdtkWorld world;
    
    public EntityFactory() {
        
    }
    
    public void Load(Scene scene, LdtkWorld world) {
        this.scene = scene;
        this.world = world;
        
        List<EntityInstance> entities = world.GetEntities(["Actors", "Solids"]);
        foreach(EntityInstance instance in entities) {
            if(instance.Tags.Contains("Actor")) {
                switch(instance.Identifier) {
                    case "Player": CreatePlayer(instance); break;
                }
            }
            else if(instance.Tags.Contains("Solid")) {
                switch(instance.Identifier) {
                    case "Platform": CreatePlatform(instance); break;
                }
            }
        }
    }
    
    public void Update(float deltaTime) {
        
    }
    
    public void CreatePlayer(EntityInstance instance) {
        string? name = instance.FieldInstances[0].Value?.ToString(); if(name == null) return;
        string? texturePath = instance.FieldInstances[1].Value?.ToString(); if(texturePath == null) return;
        
        Player player = new Player(name, texturePath);
        player.position.X = instance.Px[0];
        player.position.Y = instance.Px[1];
        scene.AddActor(player);
    }
    
    public void CreatePlatform(EntityInstance instance) {
        string? name = instance.FieldInstances[0].Value.ToString(); if(name == null) return;
        string? texturePath = instance.FieldInstances[1].Value.ToString(); if(texturePath == null) return;
        
        if(instance.Width <= 16) { // Small platform
            Platform platform = new Platform(name, texturePath, 0, 0, 16, 5);
            platform.position = new Vector2(instance.Px[0], instance.Px[1]);
            scene.AddSolid(platform);
        }
        else {
            Platform platform = new Platform(name, texturePath, 0, 5, 34, 5);
            platform.position = new Vector2(instance.Px[0], instance.Px[1]);
            scene.AddSolid(platform);
        }
    }
}