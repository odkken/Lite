using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Lite.Lib.Entities;
using Lite.Lib.GameCore;
using Lite.Lib.Interface;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Lite
{
    public class Board : IBoard, Drawable
    {

        private readonly int _level;
        private readonly List<ITile> _tiles;
        private readonly Stack<ITile> _lastClickeds = new Stack<ITile>();
        public Board(int level, IInput input)
        {
            _level = level;
            _tiles = new List<ITile>();
            var lines = File.ReadAllLines($"levels/{level}.txt");
            var tileSize = 200;
            var rows = lines.Length;
            var cols = lines[0].Length;
            var totalWidth = cols * tileSize;
            var totalHeight = rows * tileSize;
            var halfWindowSize = (Vector2i)Core.WindowUtil.GetPixelSize(new Vector2f(.5f, .5f));
            var origin = new Vector2i(halfWindowSize.X - totalWidth / 2, halfWindowSize.Y - totalHeight / 2);
            int i = 0;
            int j = 0;
            try
            {
                for (; i < lines.Length; i++)
                {
                    for (; j < lines.First().Length; j++)
                    {
                        ITile tile;
                        switch (lines[i][j])
                        {
                            case 'a':
                                tile = new BasicTile(new Vector2i(j, i), origin, this, input, SetLastClicked, false, tileSize) { Activated = true };
                                break;
                            case 'g':
                                tile = new BasicTile(new Vector2i(j, i), origin, this, input, SetLastClicked, true, tileSize);
                                break;
                            case 'x':
                                tile = new BasicTile(new Vector2i(j, i), origin, this, input, SetLastClicked, false, tileSize);
                                break;
                            default: throw new Exception($"unknown character '{lines[i][j]}'");
                        }
                        _tiles.Add(tile);
                    }
                }
            }
            catch (Exception e)
            {
                Core.Logger.Log($"Error parsing level {level} (i,j) = {i},{j}:\n{e.Message}\n{e.StackTrace}", Category.Error);
                _tiles.Clear();
            }

            input.KeyPressed += args =>
                {
                    if (args.Code == Keyboard.Key.Z)
                        Undo();
                };
        }

        public event Action<int> OnSolved;

        public void SetLastClicked(ITile tile)
        {
            _lastClickeds.Push(tile);
        }

        public void Undo()
        {
            if (_lastClickeds.Any())
                _lastClickeds.Pop().Undo();
        }

        public ITile GetTile(Vector2i coord)
        {
            return GetTile(coord.X, coord.Y);
        }

        public void Disable()
        {
            _disabled = true;
            _tiles.ForEach(a=> a.Disable());
        }

        public ITile GetTile(int xCoord, int yCoord)
        {
            return _tiles.Single(a => a.X == xCoord && a.Y == yCoord);
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            _tiles.ForEach(a => a.Draw(target, states));
        }

        private bool _solved;
        private bool _disabled;

        public void Update()
        {
            if(_disabled)
                return;
            if (!_solved && _tiles.Any() && _tiles.All(a => a.Satisfied))
            {
                _solved = true;
                OnSolved?.Invoke(_level);
            }

        }
    }
}