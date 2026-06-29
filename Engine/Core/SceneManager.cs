using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text.Json;
using Raylib_cs;

/// <summary>
/// Class that contains <tt>Actors</tt> and <tt>Solids</tt> together with a 2D raylib camera, a scene manager and the entitiy factory.
/// </summary>
public class Scene {
    public int index;
    Rectangle previousBounds;
    public bool debug;
    public Dictionary<string, Actor> entities;
    public List<string> entitiesToDestroy;
    public List<Actor> actors;
    public List<Solid> solids;
    public Vector2 mousePosition;
    public Vector2 worldMousePosition;
    public Camera2D camera;
    public SceneManager? sceneManager;
    public EntityFactory factory;
    
    Actor? selectedEntity;
    float offsetX = 0;
    float offsetY = 0;
    
    public Scene() {
        index = -1;
        previousBounds = new Rectangle(0, 0, 0, 0);
        debug = false;
        
        entities = new Dictionary<string, Actor>();
        entitiesToDestroy = new List<string>();
        
        actors = new List<Actor>();
        solids = new List<Solid>();
        
        camera = new Camera2D(new Vector2(0, 0), new Vector2(0, 0), 0, 1);
        sceneManager = null;
        
        factory = new EntityFactory();
    }
    
    public virtual void Start() {
        foreach(Actor e in entities.Values) {
            e.Start();
        }
    }
    
    public virtual void beforeUpdate(float deltaTime) {
        // Stuff...
    }
    
    public virtual void Update(float deltaTime) {
        if(Raylib.IsKeyPressed(KeyboardKey.Grave)) {
            debug = !debug;
        }
        
        mousePosition = Raylib.GetMousePosition();
        offsetX = (Raylib.GetScreenWidth()-Utils.gameScreenWidth * Utils.scale)*0.5f;
        offsetY = (Raylib.GetScreenHeight()-Utils.gameScreenHeight * Utils.scale)*0.5f;
        worldMousePosition = new Vector2((mousePosition.X-offsetX)/Utils.scale, (mousePosition.Y-offsetY)/Utils.scale);
        worldMousePosition = Raylib.GetScreenToWorld2D(worldMousePosition, camera);
        
        factory.Update(deltaTime);
        foreach(Actor e in entities.Values) {
            if(e.enabled) {
                e.Update(deltaTime);
            }
        }
    }
    
    public virtual void afterUpdate(float deltaTime) {
        foreach(Actor e in entities.Values) {
            if(e.enabled) {
                e.LateUpdate(deltaTime);
            }
        }
    }
    
    public virtual void Draw() {
        foreach(Actor e in entities.Values) {
            if(e.enabled) {
                e.Draw();
                if(debug) {
                    Raylib.DrawRectangleLinesEx(e.rect, 1, Color.Red);
                }
            }
        }
        
        if(debug) {
            Raylib.DrawCircleLines(
                (int)worldMousePosition.X,
                (int)worldMousePosition.Y,
                5,
                Color.Blue
            );
        }
    }
    
    public void AddActor(Actor actor) {
        actor.scene = this;
        if(entities.Count > 0) {
            if(entities.ContainsKey(actor.name)) {
                entities[actor.name].sameNameCount++;
                actor.name += entities[actor.name].sameNameCount.ToString();
            }
        }
        
        entities.Add(actor.name, actor);
        actors.Add(actor);
    }
    
    public void AddSolid(Solid solid) {
        solid.scene = this;
        if(entities.Count > 0) {
            if(entities.ContainsKey(solid.name)) {
                entities[solid.name].sameNameCount++;
                solid.name += entities[solid.name].sameNameCount.ToString();
            }
        }
        
        entities.Add(solid.name, solid);
        solids.Add(solid);
    }
    
    public Actor? GetActorWithName(string name) {
        if(entities.ContainsKey(name)) {
            return entities[name];
        }
        
        return null;
    }
    
    public Solid? GetSolidWithName(string name) {
        if(entities.ContainsKey(name)) {
            return (Solid)entities[name];
        }
        
        return null;
    }
    
    public void destroyEntity(string name) {
        if(entities.ContainsKey(name)) {
            entitiesToDestroy.Add(name);
        }
    }
    
    public void destroyEntity(Actor entity) {
        if(entities.ContainsKey(entity.name)) {
            entitiesToDestroy.Add(entity.name);
        }
    }
    
    public void processDeletion() {
        foreach(string name in entitiesToDestroy) {
            entities.Remove(name);
            actors.RemoveAll(e => e.name == name);
            solids.RemoveAll(e => e.name == name);
        }
        
        entitiesToDestroy.Clear();
    }
    
    public void LoadCollisionLayer(LdtkWorld ldtkWorld) {
        foreach(var tile in ldtkWorld.GetLayersIntGridWithValue("Collisions", 1)) {
            Solid sol = new Solid();
            sol.name = "__collisionLayerTile";
            sol.position = new Vector2(tile.X, tile.Y);
            sol.rect = new Rectangle(tile.X, tile.Y, tile.Width, tile.Height);
            sol.rotation = 0f;
            sol.scale = new Vector2(1, 1);
            AddSolid(sol);
            //Console.WriteLine(sol.name);
        }
    }
    
    public void OnLoad() {
        entities.Clear();
        entitiesToDestroy.Clear();
        actors.Clear();
        solids.Clear();
    }
}

public class SceneManager {
    int nextScene;
    int currentSceneIndex;
    int newSceneIndex;
    bool canSwitch;
    public Dictionary<int, Scene> scenes;
    public Game game;

    public SceneManager(Game game) {
        nextScene = 0;
        currentSceneIndex = 0;
        canSwitch = false;
        scenes = new();
        this.game = game;
    }

    public void AddScene(Scene scene) {
        scene.sceneManager = this;
        scene.index = nextScene;
        scenes.Add(nextScene, scene);
        nextScene++;
    }

    public Scene GetCurrentScene() {
        if(scenes.TryGetValue(currentSceneIndex, out Scene? scene)) {
            return scene;
        }
        
        throw new ArgumentException("Couldn't get the current scene!");
    }

    public void SwitchScene(int newSceneIndex) {
        this.newSceneIndex = newSceneIndex;
        canSwitch = true;
    }

    public void Update(float delta_time) {
        if(canSwitch) {
            GetCurrentScene().OnLoad();

            currentSceneIndex = newSceneIndex;
            GetCurrentScene().Start();
            canSwitch = false;
        }
    }
}