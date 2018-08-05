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

        public TextLevelParser(IInput input)
        {
            _input = input;
        }

        public (List<ITile>, Character, Vector2f, Func<Vector2i, Vector2f>) Parse(string levelName)
        {
            var allLevels = Directory.EnumerateFiles($"..\\..\\..\\levels/").Select(a => new FileInfo(a));
            var levelToLoad = allLevels.Single(a => a.Name.ToLower() == $"{levelName}.txt").FullName;
            var lines = File.ReadAllLines(levelToLoad);
            var rows = lines.Length;
            var cols = lines.Select(a => a.Length).Max();
            var ratio = Core.WindowUtil.GetPixelSize(new Vector2f(1.0f / cols, 1.0f / rows));
            var tileSize = (int)Math.Min(ratio.X, ratio.Y);
            var sv = new Vector2f(tileSize, tileSize);
            var totalWidth = cols * tileSize;
            var totalHeight = rows * tileSize;
            var halfWindowSize = (Vector2i)Core.WindowUtil.GetPixelSize(new Vector2f(.5f, .5f));
            var origin = new Vector2i(halfWindowSize.X - totalWidth / 2, halfWindowSize.Y - totalHeight / 2);
            var i = 0;
            var j = 0;
            var getScreenPos = new Func<Vector2i, Vector2f>(c => new Vector2f(origin.X + tileSize * c.X, origin.Y + tileSize * c.Y));
            var characterFound = false;

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
                                tile = new KeyTile(pv, sv, getScreenPos);
                                break;
                            case 'o':
                                tile = new EmptyTile(pv, sv, getScreenPos);
                                break;
                            case 'g':
                                tile = new GoalTile(pv, sv, getScreenPos);
                                break;
                            case 'c':
                                character = new Character(_input, new Vector2i(tileSize, tileSize), new List<RectWithIntPosition> { new RectWithIntPosition { Position = pv, Rect = new RectangleShape(new Vector2f(tileSize, tileSize)) { FillColor = Color.Cyan } } }, getScreenPos, vector2I => tiles.Any(a => a.X == vector2I.X && a.Y == vector2I.Y), BaseTile.OutlineThickness,
                                    pos => tiles.Where(a => a.GetType() == typeof(KeyTile) && (new Vector2i(a.X, a.Y) - pos).SquareMagnitude() == 1).Select(a => new Vector2i(a.X, a.Y)).ToList(), item =>
                                    {
                                        tiles.RemoveAll(a => a.X == item.X && a.Y == item.Y);
                                        tiles.Add(new EmptyTile(item, sv, getScreenPos));
                                    });
                                tile = new EmptyTile(pv, sv, getScreenPos);
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

            return (tiles, character, new Vector2f(tileSize, tileSize), getScreenPos);
        }
    }
}