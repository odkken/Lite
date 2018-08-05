using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Lite.Lib;
using Lite.Lib.GameCore;
using Lite.Lib.Interface;
using SFML.Graphics;
using SFML.System;

namespace Lite
{
    public class TextLevelParser : ILevelParser
    {
        private readonly IInput _input;
        private readonly ITileFactory _tileFactory;
        private Func<int, Vector2i, Vector2i, Vector2f> _getScreenPos;

        public TextLevelParser(IInput input, ITileFactory tileFactory, Func<int, Vector2i, Vector2i, Vector2f> getScreenPos)
        {
            _input = input;
            _tileFactory = tileFactory;
            _getScreenPos = getScreenPos;
        }

        public (List<ITile>, Character, Vector2i, Func<Vector2i, Vector2f>) Parse(string levelName)
        {
            var allLevels = Directory.EnumerateFiles($"..\\..\\..\\levels/").Select(a => new FileInfo(a));
            var levelToLoad = allLevels.Single(a => a.Name.ToLower() == $"{levelName}.txt").FullName;
            var lines = File.ReadAllLines(levelToLoad);
            var rows = lines.Length;
            var cols = lines.Select(a => a.Length).Max();
            var boardSize = new Vector2i(cols, rows);
            var i = 0;
            var j = 0;
            var characterFound = false;
            var tileSize = _tileFactory.GetTileSize(new Vector2i(rows, cols));
            var tiles = new List<ITile>();
            Character character = null;
            try
            {
                for (; i < lines.Length; i++)
                {
                    j = 0;
                    for (; j < lines[i].Length; j++)
                    {
                        ITile tile;
                        var pv = new Vector2i(j, i);
                        switch (lines[i][j])
                        {
                            case 'x':
                                continue;
                            case 's':
                                tile = _tileFactory.CreateTile(pv.X, pv.Y, tileSize, boardSize, TileType.Key);
                                break;
                            case 'o':
                                tile = _tileFactory.CreateTile(pv.X, pv.Y, tileSize, boardSize, TileType.Walkable);
                                break;
                            case 'g':
                                tile = _tileFactory.CreateTile(pv.X, pv.Y, tileSize, boardSize, TileType.Goal);
                                break;
                            case 'c':
                                character = new Character(_input, new Vector2i(tileSize, tileSize),
                                    new List<RectWithIntPosition>
                                    {
                                        new RectWithIntPosition
                                        {
                                            Position = pv,
                                            Rect = new RectangleShape(new Vector2f(tileSize, tileSize))
                                            {
                                                FillColor = Color.Cyan
                                            }
                                        }
                                    }, vector2I => _getScreenPos(tileSize, vector2I, boardSize), vector2I => tiles.Any(a => a.X == vector2I.X && a.Y == vector2I.Y), -1,
                                    pos => tiles.Where(a => a.Type == TileType.Key && (new Vector2i(a.X, a.Y) - pos).SquareMagnitude() == 1).Select(a => new Vector2i(a.X, a.Y)).ToList(), item =>
                                    {
                                        tiles.RemoveAll(a => a.X == item.X && a.Y == item.Y);
                                        tiles.Add(_tileFactory.CreateTile(pv.X, pv.Y, tileSize, boardSize, TileType.Walkable));
                                    });
                                tile = _tileFactory.CreateTile(pv.X, pv.Y, tileSize, boardSize, TileType.Walkable);
                                characterFound = true;
                                break;
                            default: throw new Exception($"unknown character '{lines[i][j]}'");
                        }
                        tiles.Add(tile);
                    }
                }

                if (!characterFound) throw new Exception("No character spawn was found.");
            }
            catch (Exception e)
            {
                Core.Logger.Log($"Error parsing level {levelName} (i,j) = {i},{j}:\n{e.Message}\n{e.StackTrace}", Category.Error);
                tiles.Clear();
            }

            return (tiles, character, new Vector2i(tileSize, tileSize), vector2I => _getScreenPos(tileSize, vector2I, boardSize));
        }
    }
}