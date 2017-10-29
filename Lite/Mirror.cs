using System;
using SFML.Graphics;
using SFML.System;

namespace Lite
{
    public class Mirror : Drawable
    {
        private readonly Beam _physicalRepresentation;

        public Mirror(Vector2f position)
        {
            _physicalRepresentation = new Beam(5, position, new Vector2f(22, 60), Color.White);
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            _physicalRepresentation.Draw(target, states);
        }

        public bool TryGetIntersection(Vector2f origin, Vector2f direction, out Vector2f intersection)
        {
            var info = MathUtil.GetLineInfo(origin, direction);
            var myInfo = MathUtil.GetLineInfo(_physicalRepresentation.Origin, _physicalRepresentation.Representation);
            var det = myInfo.A * info.B - myInfo.B * info.A;
            if (Math.Abs(det) <= double.Epsilon * 10)
            {
                intersection = new Vector2f();
                return false;
            }

            intersection = new Vector2f((info.B * myInfo.C - info.C * myInfo.B) / det, (myInfo.A * info.C - info.A * myInfo.C) / det);
            var inMyDirection = direction.UnNormalizedDot(intersection - origin) > 0;

            var endpoints = _physicalRepresentation.Endpoints;
            var inMyBounds = MathF.Min(endpoints.Item1.X, endpoints.Item2.X) <= intersection.X &&
                             MathF.Max(endpoints.Item1.X, endpoints.Item2.X) >= intersection.X && MathF.Min(endpoints.Item1.Y, endpoints.Item2.Y) <= intersection.Y &&
                             MathF.Max(endpoints.Item1.Y, endpoints.Item2.Y) >= intersection.Y;
            return inMyBounds && inMyDirection;
        }
    }
}