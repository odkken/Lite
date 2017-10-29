using System;
using Lite;
using SFML.Graphics;
using SFML.System;

namespace Lite
{
    public class Beam : Drawable
    {
        public Vector2f Representation { get; }
        public Vector2f Origin { get; }
        private readonly RectangleShape _rect;
        public Tuple<Vector2f, Vector2f> Endpoints { get; }

        public Beam(float width, Vector2f origin, Vector2f representation, Color color)
        {
            Representation = representation;
            Origin = origin;
            _rect = new RectangleShape(new Vector2f(width, representation.Magnitude()))
            {
                Position = origin,
                FillColor = color,
                Rotation = -360 * MathF.Atan2(representation.X, representation.Y) / (2 * MathF.PI)
            };
            Endpoints = new Tuple<Vector2f, Vector2f>(origin, origin + representation);
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            _rect.Draw(target, states);
        }
    }
}