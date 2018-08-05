using System;
using System.Collections.Generic;
using Lite.Lib.Interface;
using SFML.Graphics;
using SFML.System;

namespace Lite
{
    public class Tile : ITile
    {
        public const int OutlineThickness = -2;
        private readonly Func<int, Vector2i, Vector2f> _getScreenPos;
        private readonly RectangleShape _rect;

        private static Dictionary<TileType, Tuple<Color, Color>> _colorLookup = new Dictionary<TileType, Tuple<Color, Color>>
        {
            [TileType.Walkable] = Tuple.Create(new Color(150, 150, 150), Color.White),
            [TileType.Key] = Tuple.Create(Color.Cyan, Color.White),
            [TileType.Goal] = Tuple.Create(Color.Black, Color.Cyan),
            [TileType.Unused] = Tuple.Create(Color.Black, Color.White)
        };

        public Tile(Vector2i position, Vector2f size, Func<int, Vector2i, Vector2f> getScreenPos, TileType type)
        {
            X = position.X;
            Y = position.Y;
            _getScreenPos = getScreenPos;
            var color = _colorLookup[type];
            _rect = new RectangleShape(size)
            {
                FillColor = color.Item1,
                OutlineColor = color.Item2,
                OutlineThickness = OutlineThickness,
                Position = _getScreenPos((int) size.X, new Vector2i(X, Y))
            };
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            _rect.Position = _getScreenPos((int) _rect.Size.X, new Vector2i(X, Y));
            _rect.Draw(target, states);
        }

        public TileType Type { get; set; }
        public int X { get; }
        public int Y { get; }
        public Vector2i PixelPosition => (Vector2i)_rect.Position;
    }
}