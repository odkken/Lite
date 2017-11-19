using System;
using Lite.Lib.GameCore;
using SFML.Graphics;
using SFML.System;

namespace Lite.Lib.Entities
{
    internal class Wall : Entity
    {
        RectangleShape shape = new RectangleShape() { FillColor = Color.Cyan, OutlineColor = Color.White, OutlineThickness = 1 };

        public override void Draw(RenderTarget target, RenderStates states)
        {
            shape.Draw(target, states);
        }

        public override void Update()
        {
            shape.Size = Core.WindowUtil.GetPixelSize(new Vector2f(Core.World.ScreenFractionPerTile, Core.World.ScreenFractionPerTile));
            var worldPos = Position;
            shape.Position = new Vector2f(shape.Size.X * worldPos.X, shape.Size.Y * worldPos.Y);
        }
    }
}