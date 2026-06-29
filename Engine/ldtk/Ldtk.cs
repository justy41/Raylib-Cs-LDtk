using System.Numerics;
using QuickType;
using Raylib_cs;

/// <summary>
/// Represents a value for collisions (Example: 0 - air, 1 - solid, 2 - oneway platform).
/// The coordinates are treated just as if they were tiles in the world.
/// </summary>
public class IntGridTile {
    public float X, Y, Width, Height;
    public int Value;
    
    /// <summary>
    /// Represents a value for collisions (Example: 0 - air, 1 - solid, 2 - oneway platform).
    /// The coordinates are treated just as if they were tiles in the world.
    /// </summary>
    public IntGridTile(float x, float y, float width, float height, int value = 0) {
        X = x;
        Y = y;
        Width = width;
        Height = height;
        Value = value;
    }
}

/// <summary>
/// Class that contains usefull functions to Load, Get or Draw levels, layers and entities from a .ldtk file.
/// </summary>
public class LdtkWorld {
    Coordinate coord;
    public string filepath;
    public Dictionary<long, Texture2D> tilesetTextures;
    
    public LdtkWorld(string filepath) {
        string jsonString = File.ReadAllText(filepath);
        coord = Coordinate.FromJson(jsonString);
        this.filepath = filepath;
        tilesetTextures = new();
    }
    
    public List<Level> GetLevels() {
        return coord.Levels.ToList();
    }
    
    public Level GetLevel(string name) {
        foreach(var level in GetLevels()) {
            if(level.Identifier == name)
                return level;
        }
        
        throw new ArgumentException("Couldn't find level with name " + name);
    }
    
    public LayerInstance GetLayer(string name, string levelName) {
        foreach(var layer in GetLevel(levelName).LayerInstances) {
            if(layer.Identifier == name)
                return layer;
        }
        
        throw new ArgumentException("Couldn't get layer with name " + name + " in level with name " + levelName);
    }
    
    /// <summary>
    /// Returns a list of tiles with any kind of IntGrid value. These tiles represent the collisions from an IntGrid layer.
    /// </summary>
    public List<IntGridTile> GetLayersIntGrid(string collisionLayerName) {
        List<IntGridTile> tiles = new();
        foreach(Level level in GetLevels()) {
            var collisionLayer = GetLayer(collisionLayerName, level.Identifier);
            for(int i = 0; i<collisionLayer.IntGridCsv.Length-1; i++) {
                if(collisionLayer.IntGridCsv[i] != 0) {
                    int x = (i%(int)collisionLayer.CWid) * (int)collisionLayer.GridSize;
                    int y = (i/(int)collisionLayer.CWid) * (int)collisionLayer.GridSize;
                    
                    x += (int)level.WorldX;
                    y += (int)level.WorldY;
                    
                    tiles.Add(new IntGridTile(x, y, collisionLayer.GridSize, collisionLayer.GridSize, (int)collisionLayer.IntGridCsv[i]));
                }
            }
        }
        
        return tiles;
    }
    
    /// <summary>
    /// Gets a list of entities from the specified layers.
    /// </summary>
    public List<EntityInstance> GetEntities(string[] entityLayerNames) {
        List<EntityInstance> result = new();
        foreach(var level in GetLevels()) {
            foreach(string name in entityLayerNames) {
                result.AddRange(GetLayer(name, level.Identifier).EntityInstances.ToList());
            }
        }
        
        return result;
    }
    
    /// <summary>
    /// Gets an entity with a specific name in specified entity layers.
    /// </summary>
    public EntityInstance? GetEntity(string entityName, string[] entityLayerNames) {
        foreach(var level in GetLevels()) {
            foreach(string name in entityLayerNames) {
                foreach(var instance in GetLayer(name, level.Identifier).EntityInstances) {
                    if(entityName == instance.Identifier) {
                        return instance;
                    }
                }
            }
        }
        
        return null;
    }
    
    /// <summary>
    /// Returns a list of tiles with the IntGrid value specified. These tiles represent the collisions from an IntGrid layer with a chosen value.
    /// Should be used for generating a Solid, a Oneway platform or other custom examples like that.
    /// </summary>
    public List<IntGridTile> GetLayersIntGridWithValue(string collisionLayerName, int valueToGet) {
        List<IntGridTile> tiles = new();
        foreach(Level level in GetLevels()) {
            var collisionLayer = GetLayer(collisionLayerName, level.Identifier);
            for(int i = 0; i<collisionLayer.IntGridCsv.Length-1; i++) {
                if(collisionLayer.IntGridCsv[i] == valueToGet) {
                    int x = (i%(int)collisionLayer.CWid) * (int)collisionLayer.GridSize;
                    int y = (i/(int)collisionLayer.CWid) * (int)collisionLayer.GridSize;
                    
                    x += (int)level.WorldX;
                    y += (int)level.WorldY;
                    
                    tiles.Add(new IntGridTile(x, y, collisionLayer.GridSize, collisionLayer.GridSize, valueToGet));
                }
            }
        }
        
        return tiles;
    }
    
    /// <summary>
    /// Load all tileset textures to be used by the level.
    /// </summary>
    public void LoadTilesetTextures() {
        foreach(var level in GetLevels()) {
            foreach(var layer in level.LayerInstances) {
                if(layer.TilesetDefUid != null && !tilesetTextures.ContainsKey((long)layer.TilesetDefUid)) {
                    Texture2D t = Raylib.LoadTexture(RemoveLastPathItem(filepath) + layer.TilesetRelPath);
                    tilesetTextures.Add((long)layer.TilesetDefUid, t);
                }
            }
        }
    }
    
    public void UnLoadTextures() {
        foreach(var t in tilesetTextures.Values) {
            Raylib.UnloadTexture(t);
        }
    }
    
    /// <summary>
    /// Draw the whole LDtk world.
    /// </summary>
    public void DrawWorld() {
        foreach(Level level in GetLevels()) {
            DrawLevel(level.Identifier);
        }
    }
    
    /// <summary>
    /// Draw LDtk levels with specified names.
    /// </summary>
    public void DrawLevels(string[] levelNames) {
        foreach(string name in levelNames) {
            DrawLevel(GetLevel(name).Identifier);
        }
    }
    
    /// <summary>
    /// Draw LDtk level with a specific name.
    /// </summary>
    public void DrawLevel(string levelName) {
        Level level = GetLevel(levelName);
        foreach(var layer in level.LayerInstances) {
            DrawLayer(layer.Identifier, level.Identifier);
        }
    }
    
    /// <summary>
    /// Draw a LDtk layer with a specific name from a specific level.
    /// </summary>
    public void DrawLayer(string name, string levelName) {
        Level level = GetLevel(levelName);
        LayerInstance layer = GetLayer(name, levelName);
        if(layer.Visible) {
            foreach(var tile in layer.AutoLayerTiles) {
                Rectangle srcRect = new Rectangle(tile.Src[0], tile.Src[1], layer.GridSize, layer.GridSize);
                Rectangle destRect = new Rectangle(tile.Px[0] + level.WorldX + layer.PxOffsetX, tile.Px[1] + level.WorldY + layer.PxOffsetY, layer.GridSize, layer.GridSize);
                
                if(tile.F == 1) srcRect.Width = -srcRect.Width;
                if(tile.F == 2) srcRect.Height = -srcRect.Height;
                
                var tint = Raylib.ColorAlpha(Color.White, (float)tile.A);
                if(layer.TilesetDefUid != null && tilesetTextures.ContainsKey((long)layer.TilesetDefUid)) {
                    Raylib.DrawTexturePro(tilesetTextures[(long)layer.TilesetDefUid], srcRect, destRect, Vector2.Zero, 0f, tint);
                }
            }
        }
    }
    
    string RemoveLastPathItem(string path) {
        int removePos = 0;
        for(int i = path.Length-1; i>=0; i--) {
            if(path[i] == '/') {
                removePos = i;
                break;
            }
        }
        
        return path.Remove(removePos+1);
    }
}