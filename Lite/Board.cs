﻿//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Numerics;
//using Lite.Lib;
//using Lite.Lib.GameCore;
//using Lite.Lib.Interface;
//using SFML.Graphics;
//using SFML.System;
//using SFML.Window;

//namespace Lite
//{
//    public class Board : IBoard, Drawable
//    {
//        private readonly List<ITile> _tiles;
//        public Board(int level, IInput input, Action<Character> spawnCharacter)
//        {
//            _tiles = new List<ITile>();
//            var allLevels = Directory.EnumerateFiles($"..\\..\\..\\levels/").Select(a => new FileInfo(a));
//            var levelToLoad = "";
//            if (level == -1)
//                levelToLoad = allLevels.OrderBy(a => a.LastWriteTime).Last().FullName;
//            else
//                levelToLoad = allLevels.Single(a => a.FullName.ToLower().EndsWith($"{level}.txt")).FullName;

//            var lines = File.ReadAllLines(levelToLoad);
//            var rows = lines.Length;
//            var cols = lines.Select(a => a.Length).Max();
//            var ratio = Core.WindowUtil.GetPixelSize(new Vector2f(1.0f / cols, 1.0f / rows));
//            var tileSize = (int)Math.Min(ratio.X, ratio.Y);
//            var sv = new Vector2f(tileSize, tileSize);
//            var totalWidth = cols * tileSize;
//            var totalHeight = rows * tileSize;
//            var halfWindowSize = (Vector2i)Core.WindowUtil.GetPixelSize(new Vector2f(.5f, .5f));
//            var origin = new Vector2i(halfWindowSize.X - totalWidth / 2, halfWindowSize.Y - totalHeight / 2);
//            var i = 0;
//            var j = 0;
//            var getScreenPos = new Func<Vector2i, Vector2f>(c => new Vector2f(origin.X + tileSize * c.X, origin.Y + tileSize * c.Y));
//            var characterFound = false;
//            try
//            {
//                for (; i < lines.Length; i++)
//                {
//                    j = 0;
//                    for (; j < lines[i].Length; j++)
//                    {
//                        ITile tile;
//                        var pv = new Vector2i(j, i);
//                        switch (lines[i][j])
//                        {
//                            case 'x':
//                                continue;
//                            case 's':
//                                tile = new KeyTile(pv, sv, getScreenPos);
//                                break;
//                            case 'o':
//                                tile = new EmptyTile(pv, sv, getScreenPos);
//                                break;
//                            case 'g':
//                                tile = new GoalTile(pv, sv, getScreenPos);
//                                break;
//                            case 'c':
//                                spawnCharacter(new Character(input, pv, sv, getScreenPos, vector2I => _tiles.Any(a => a.X == vector2I.X && a.Y == vector2I.Y), BaseTile.OutlineThickness,
//                                    pos => _tiles.Where(a => a.GetType() == typeof(KeyTile) && (new Vector2i(a.X, a.Y) - pos).SquareMagnitude() == 1).Select(a => new Vector2i(a.X, a.Y)).ToList(), item =>
//                                    {
//                                        _tiles.RemoveAll(a => a.X == item.X && a.Y == item.Y);
//                                        _tiles.Add(new EmptyTile(item, sv, getScreenPos));
//                                    }));
//                                tile = new EmptyTile(pv, sv, getScreenPos);
//                                characterFound = true;
//                                break;
//                            default: throw new Exception($"unknown character '{lines[i][j]}'");
//                        }
//                        _tiles.Add(tile);
//                    }
//                }

//                if (!characterFound) throw new Exception("No character spawn was found.");
//            }
//            catch (Exception e)
//            {
//                Core.Logger.Log($"Error parsing level {level} (i,j) = {i},{j}:\n{e.Message}\n{e.StackTrace}", Category.Error);
//                _tiles.Clear();
//            }

//        }

//        public event Action<int> OnSolved;


//        public ITile GetTile(Vector2i coord)
//        {
//            return GetTile(coord.X, coord.Y);
//        }


//        public ITile GetTile(int xCoord, int yCoord)
//        {
//            return _tiles.Single(a => a.X == xCoord && a.Y == yCoord);
//        }

//        public void Draw(RenderTarget target, RenderStates states)
//        {
//            _tiles.ForEach(a => a.Draw(target, states));
//        }

//        private bool _solved;
//        private bool _disabled;

//        public void Update()
//        {
//        }

//        public ITile GetTile(Vector2 mousePos)
//        {
//            var x = (int)Math.Floor(_tiles.Max(a => a.X) * mousePos.X);
//            var y = (int)Math.Floor(_tiles.Max(a => a.Y) * mousePos.Y);
//            return _tiles.SingleOrDefault(a => a.X == x && a.Y == y);
//        }
//    }
//}