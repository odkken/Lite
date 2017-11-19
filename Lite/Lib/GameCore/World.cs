using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Lite.Lib.Entities;
using Lite.Lib.Terminal;
using SFML.Graphics;
using SFML.System;

namespace Lite.Lib.GameCore
{
    internal class World : Drawable, IWorld
    {
        private Tile[,] Map;
        private Text[,] coords;
        public bool DrawCoordText = false;
        readonly Func<int, List<string>> _getLinesForLevel;
        public void Load(int level)
        {
            var lines = _getLinesForLevel(level);
            _currentLevel = level;
            foreach (var keyValuePair in _allEntities)
            {
                keyValuePair.Value.Destroy();
            }
            _allEntities.Clear();
            var y = 0;
            Map = new Tile[lines.Max(a => a.Length), lines.Count];
            coords = new Text[Map.GetLength(0), Map.GetLength(1)];
            foreach (var line in lines)
            {
                Core.Logger.Log(line);
                var x = 0;
                foreach (var letter in line)
                {
                    switch (letter)
                    {
                        case 'x':
                            Map[x, y] = new Wall();
                            break;
                        case 'd':
                            Map[x, y] = new Door();
                            break;
                        case 'b':
                            {
                                var tile = new Empty();
                                tile.SetPosition(new Vector2i(x, y));
                                Map[x, y] = tile;
                                new Box().Position = new Vector2i(x, y);
                            }
                            break;
                        case 'c':
                            {
                                var tile = new Empty();
                                tile.SetPosition(new Vector2i(x, y));
                                Map[x, y] = tile;
                                new Player().Position = new Vector2i(x, y);
                            }
                            break;
                        case ' ':
                            Map[x, y] = new Empty();
                            break;
                        default:
                            Core.Logger.Log($"Unknown tile type '{letter}'");
                            break;
                    }
                    Map[x, y].SetPosition(new Vector2i(x, y));
                    x++;
                }
                y++;
            }


            for (var i = 0; i < Map.GetLength(0); i++)
            {
                for (var j = 0; j < Map.GetLength(1); j++)

                {
                    var position = new Vector2i(i, j);
                    coords[i, j] = new Text($"{i},{j}", Core.Text.DefaultFont, 20) { Position = Tile.TileSize * ((Vector2f)position + new Vector2f(0.5f, 0.5f)) };
                    if (Map[i, j] == null)
                    {
                        Map[i, j] = new Empty();
                        Map[i, j].SetPosition(position);
                    }
                }
            }
        }

        public Vector2i PlayerCoord { get; }
        public List<string> Entities => _allEntities.Values.OrderBy(a => a.GetType().ToString()).Select(a => a.ToString()).ToList();

        public Vector2i GetCoordOfEntity(Entity entity)
        {
            for (int x = 0; x < Map.GetLength(0); x++)
            {
                for (int y = 0; y < Map.GetLength(1); y++)
                {
                    if (Map[x, y]?.Id == entity.Id)
                        return new Vector2i(x, y);
                }
            }
            throw new Exception("entity not found");
        }

        public Entity GetEntity(int entityId)
        {
            return _allEntities[entityId];
        }

        public void LoadNextLevel()
        {
            Load(_currentLevel + 1);
        }

        public Tile GetTileAt(Vector2i position)
        {
            return Map[position.X, position.Y];
        }

        public void Update()
        {
            foreach (var allEntitiesValue in _allEntities.Values)
            {
                allEntitiesValue.Update();
            }
        }

        private Dictionary<int, Entity> _allEntities = new Dictionary<int, Entity>();
        private int _currentLevel;

        public World(Func<int, List<string>> getLinesForLevel)
        {
            _getLinesForLevel = getLinesForLevel;
        }

        public void RegisterEntity(Entity obj)
        {
            _allEntities.Add(obj.Id, obj);
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            var undrawnEntities = _allEntities.ToDictionary(a => a.Key, a => a.Value);
            foreach (var tile in Map)
            {
                tile.Draw(target, states);
                undrawnEntities.Remove(tile.Id);
            }
            if (DrawCoordText)
                foreach (var coord in coords)
                {
                    coord.Draw(target, states);
                }
            foreach (var undrawnEntity in undrawnEntities)
            {
                undrawnEntity.Value.Draw(target, states);
            }
        }

        public Vector2i SetPosition(Entity e, Vector2i destination)
        {
            if (e is Tile)
                return destination;

            var tile = Map[e.Position.X, e.Position.Y];
            (tile as Empty)?.RemoveEntity(e);
            (Map[destination.X, destination.Y] as Empty).StoreEntity(e);
            return destination;
        }
    }
}