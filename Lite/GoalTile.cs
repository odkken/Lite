using System;
using Lite.Lib.Interface;
using SFML.Graphics;
using SFML.System;

namespace Lite
{
    public abstract class BaseTile : ITile
    {
        public const int OutlineThickness = -2;
        private readonly Func<Vector2i, Vector2f> _getScreenPos;
        private readonly RectangleShape _rect;

        protected BaseTile(Vector2i position, Vector2f size, Func<Vector2i, Vector2f> getScreenPos, Color color, Color outlineColor)
        {
            X = position.X;
            Y = position.Y;
            _getScreenPos = getScreenPos;
            _rect = new RectangleShape(size) { FillColor = color, OutlineColor = outlineColor, OutlineThickness = OutlineThickness };
            _rect.Position = _getScreenPos(new Vector2i(X, Y));
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            _rect.Position = _getScreenPos(new Vector2i(X, Y));
            _rect.Draw(target, states);
        }

        public int X { get; }
        public int Y { get; }
        public Vector2i PixelPosition => (Vector2i) _rect.Position;

        public void SetColor(Color color)
        {
            _rect.FillColor = color;
        }
    }
    public class GoalTile : BaseTile
    {
        public GoalTile(Vector2i position, Vector2f size, Func<Vector2i, Vector2f> getScreenPos) : base(position, size, getScreenPos, Color.Black, Color.Cyan)
        {
        }
    }
}