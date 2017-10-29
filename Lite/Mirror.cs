using System;
using SFML.Graphics;
using SFML.System;

namespace Lite
{
    public class Mirror : Drawable
    {
        private readonly Beam _physicalRepresentation;

        public Mirror(Beam physicalRepresentation)
        {
            _physicalRepresentation = physicalRepresentation;
        }

        public Mirror(Vector2f position)
        {
            _physicalRepresentation = new Beam(5, position, new Vector2f(0, 2000), Color.White);
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            _physicalRepresentation.Draw(target, states);
        }

        public bool TryGetIntersection(MathUtil.LineInfo info, out Vector2f intersection)
        {
            var myInfo = MathUtil.GetLineInfo(_physicalRepresentation.Origin, _physicalRepresentation.Representation);
            var det = myInfo.A * info.B - myInfo.B * info.A;
            if (Math.Abs(det) <= double.Epsilon * 10)
            {
                intersection = new Vector2f();
                return false;
            }

            intersection = new Vector2f((info.B * myInfo.C - info.C * myInfo.B) / det, (myInfo.A * info.C - info.A * myInfo.C) / det);
            return true;
        }
    }
}