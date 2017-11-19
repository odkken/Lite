using System;
using System.Collections.Generic;
using System.Linq;
using Lite.Lib.Entities;
using SFML.Graphics;
using SFML.System;

namespace Lite.Lib.GameCore
{
    internal class World : Drawable, IWorld
    {
        private Entity[,] Map;

        public void Load(List<string> lines)
        {
            var y = 0;
            Map = new Entity[lines.Max(a => a.Length), lines.Count];
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
                            Map[x, y] = new Box();
                            break;
                        case 'c':
                            Map[x, y] = new Player();
                            break;
                        case ' ':
                            break;
                        default:
                            Core.Logger.Log($"Unknown tile type '{letter}'");
                            break;
                    }
                    x++;
                }
                y++;
            }
            ScreenFractionPerTile = 1f / Math.Max(Map.GetLength(0), Map.GetLength(1));
        }

        public Vector2i PlayerCoord { get; }
        public float ScreenFractionPerTile { get; set; }

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

        public TileState GetTileState(Vector2i tileLocation)
        {
            throw new NotImplementedException();
        }

        public Entity GetEntity(int entityId)
        {
            return _allEntities[entityId];
        }

        public void Update()
        {
            foreach (var allEntitiesValue in _allEntities.Values)
            {
                allEntitiesValue.Update();
            }
        }

        private Dictionary<int, Entity> _allEntities = new Dictionary<int, Entity>();
        public void RegisterEntity(Entity obj)
        {
            _allEntities.Add(obj.Id, obj);
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            foreach (var allEntitiesValue in _allEntities.Values)
            {
                allEntitiesValue.Draw(target, states);
            }
        }
    }
}