﻿using System;
using System.Collections.Generic;
using System.Linq;
using Lite.Lib.GameCore;
using Lite.Lib.Interface;
using SFML.Graphics;
using SFML.System;

namespace Lite.Lib
{
    public class Board : IBoard
    {
        private List<ITile> _tiles = new List<ITile>();
        private Character _character;
        private readonly ILevelParser _levelParser;

        public Board(ILevelParser levelParser)
        {
            _levelParser = levelParser;
        }

        public bool LoadLevel(string levelName)
        {
            try
            {
                (_tiles, _character, TileSize, GetScreenPos) = _levelParser.Parse(levelName);
            }
            catch (Exception e)
            {
                Core.Logger.Log(e.Message, Category.Error);
                return false;
            }

            var minX = _tiles.Min(a => a.PixelPosition.X);
            var minY = _tiles.Min(a => a.PixelPosition.Y);
            var maxX = _tiles.Max(a => a.PixelPosition.X);
            var maxY = _tiles.Max(a => a.PixelPosition.Y);
            ScreenOffset = new Vector2i(minX, minY);
            PixelSize = new Vector2i(maxX - minX + TileSize.X, maxY - minY + TileSize.Y);
            Size = new Vector2i(_tiles.Max(a => a.X) + 1, _tiles.Max(a => a.Y) + 1);
            return true;
        }

        public Vector2i Size { get; set; }
        public IEnumerable<ITile> Tiles => _tiles;

        public Vector2i ScreenOffset { get; private set; }
        public Vector2i PixelSize { get; private set; }

        public Func<Vector2i, Vector2f> GetScreenPos { get; set; }

        public Vector2i TileSize { get; private set; }

        public void Draw(RenderTarget target, RenderStates states)
        {
            foreach (var tile in _tiles)
            {
                tile.Draw(target, states);
            }
            _character?.Draw(target, states);
        }

        public ITile GetTile(int xCoord, int yCoord)
        {
            return _tiles.SingleOrDefault(a => a.X == xCoord && a.Y == yCoord);
        }

        public (int, int, ITile) GetTileFromScreenCoord(Vector2f fractionalScreenCoord)
        {
            var xCoord = (int)(fractionalScreenCoord.X * (1 + _tiles.Max(a => a.X)));
            var yCoord = (int)(fractionalScreenCoord.Y * (1 + _tiles.Max(a => a.Y)));
            return (xCoord, yCoord, GetTile(xCoord, yCoord));
        }

        public void Update(float dt)
        {
            _character?.Update(dt);
        }

        public void SetTile(int i, int i1, ITile tile)
        {
            var existing = _tiles.SingleOrDefault(a => a.X == i && a.Y == i1);
            if (existing != null)
                _tiles.Remove(existing);
            _tiles.Add(tile);
        }
    }
}