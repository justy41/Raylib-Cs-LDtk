using Raylib_cs;
using rlImGui_cs;
using ImGuiNET;
using System.Numerics;
using System.Reflection;

public class Editor {
    Game game;
    // EntityFactory factory;
    public Actor? selectedEntity;
    
    bool selected;
    bool isEntityFactory;
    
    public Editor(Game game) {
        this.game = game;
        // factory = new EntityFactory(game);
        selectedEntity = null;
        selected = false;
    }
    
    public void Load() {
        rlImGui.Setup(true, true);
    }
    
    public void Update(float deltaTime) {
        // factory.Update(deltaTime);
        DeleteSelectedEntity();
    }
    
    public void Draw() {
        rlImGui.Begin();
            DrawMainMenu();
            DrawEntityManager();
            DrawEntityProperties();
        rlImGui.End();
    }
    
    void DrawField(string name, object? value, Type type, Action<object> setValue) {
        if(type == typeof(int)) {
            int v = (int)(value ?? 0);
            if(ImGui.DragInt(name, ref v)) setValue(v);
        }
        else if(type == typeof(float)) {
            float f = (float)(value ?? 0f);
            if(ImGui.DragFloat(name, ref f)) setValue(f);
        }
        else if(type == typeof(bool)) {
            bool b = (bool)(value ?? false);
            if(ImGui.Checkbox(name, ref b)) setValue(b);
        }
        else if(type == typeof(string)) {
            string s = (string)(value ?? "");
            if(ImGui.InputText(name, ref s, 256)) setValue(s);
        }
        else if(type == typeof(Vector2)) {
            Vector2 v = (Vector2)(value ?? Vector2.Zero);
            if(ImGui.DragFloat2(name, ref v)) setValue(v);
        }
        else if(type == typeof(Vector3)) {
            Vector3 v = (Vector3)(value ?? Vector3.Zero);
            if(ImGui.DragFloat3(name, ref v)) setValue(v);
        }
        else {
            ImGui.LabelText(name, value?.ToString() ?? "null");
        }
    }
    
    void DeleteSelectedEntity() {
        if(selectedEntity != null) {
            if(Raylib.IsKeyPressed(KeyboardKey.Delete)) {
                game.GetCurrentScene().destroyEntity(selectedEntity);
            }
        }
    }
    
    
    
    
    
    
    void DrawMainMenu() {
        if(ImGui.BeginMainMenuBar()) {
            if(ImGui.Button("Entity Factory")) {
                isEntityFactory = !isEntityFactory;
            }
        } ImGui.EndMainMenuBar();
        
        if(isEntityFactory) {
            // DrawEntityFactory();
        }
    }
    
    void DrawEntityManager() {
        Scene currentScene = game.GetCurrentScene();
        
        if(ImGui.Begin("Entity Manager")) {
                if(ImGui.CollapsingHeader("Actors")) {
                    foreach(Actor actor in currentScene.actors) {
                        if(actor != null) {
                            if((Raylib.IsMouseButtonPressed(0) && Raylib.CheckCollisionPointRec(currentScene.worldMousePosition, actor.rect) && actor.enabled) || ImGui.Selectable(actor.name+" ", ref actor.selected)) { // Remember to assign name to your entities!!! Also you need this long ass if statement to check for a mouse select or a hierarchy select
                                if(selectedEntity != null && selectedEntity != actor) {
                                    selectedEntity.selected = false;
                                }
                                if(Raylib.IsMouseButtonPressed(0) && Raylib.CheckCollisionPointRec(currentScene.worldMousePosition, actor.rect)) { // This double check is for the actor to always be selected when mouse clicked
                                    actor.selected = true;
                                }
                                selectedEntity = actor;
                            }
                        }
                    }
                }
                
                if(ImGui.CollapsingHeader("Solids")) {
                    foreach(Solid solid in currentScene.solids) {
                        if(solid != null) {
                            if(solid.name == null || solid.name == "") continue;
                            if(solid.name[0] == '_' && solid.name[1] == '_') continue;
                            
                            if((Raylib.IsMouseButtonPressed(0) && Raylib.CheckCollisionPointRec(currentScene.worldMousePosition, solid.rect) && solid.enabled) || ImGui.Selectable(solid.name+" ", ref solid.selected)) { // Remember to assign name to your entities!!! Also you need this long ass if statement to check for a mouse select or a hierarchy select
                                if(selectedEntity != null && selectedEntity != solid) {
                                    selectedEntity.selected = false;
                                }
                                if(Raylib.IsMouseButtonPressed(0) && Raylib.CheckCollisionPointRec(currentScene.worldMousePosition, solid.rect)) {
                                    solid.selected = true;
                                }
                                selectedEntity = solid;
                            }
                        }
                    }
                }
        } ImGui.End();
    }
    
    void DrawEntityProperties() {
        if(selectedEntity != null && selectedEntity.selected) {
            if(ImGui.Begin("Properties")) {
                Type type = selectedEntity.GetType();
                HashSet<string> fieldsToShow = new HashSet<string>{"name", "enabled", "position", "scale", "rotation", "rectSize"};
                HashSet<string> customFieldsToShow = new HashSet<string>{
                    "speed", "jumpForce", "gravity"
                };
                
                foreach(var field in type.GetFields(BindingFlags.Public | BindingFlags.Instance)) {
                    if(fieldsToShow.Contains(field.Name)) {
                        DrawField(field.Name, field.GetValue(selectedEntity), field.FieldType, v => field.SetValue(selectedEntity, v));
                    }
                }
                
                ImGui.Spacing();
                ImGui.Separator();
                ImGui.Spacing();
                
                foreach(var field in type.GetFields(BindingFlags.Public | BindingFlags.Instance)) {
                    if(customFieldsToShow.Contains(field.Name)) {
                        DrawField(field.Name, field.GetValue(selectedEntity), field.FieldType, v => field.SetValue(selectedEntity, v));
                    }
                }
            } ImGui.End();
        }
    }
    
    void DrawEntityFactory() {
        if(ImGui.Begin("Entity Factory", ref isEntityFactory)) {
            if(ImGui.TreeNode("Main Entities")) {
                if(ImGui.Selectable("Player"))
                    // factory.CreatePlayer();
                
                ImGui.TreePop();
            }
            
            if(ImGui.TreeNode("Platforms")) {
                if(ImGui.Selectable("Small Platform"))
                    // factory.CreateSmallPlatform();
                if(ImGui.Selectable("Platform"))
                    // factory.CreatePlatform();
                
                ImGui.TreePop();
            }
        } ImGui.End();
    }
}