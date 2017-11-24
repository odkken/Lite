using System;
using Lite.Lib.GameCore;
using Lite.Lib.Interface;
using SFML.Graphics;
using SFML.System;

namespace Lite.Lib.Entities
{
    class BasicTile : Tile
    {
        public BasicTile(Vector2i coord, Vector2i origin, IBoard board, IInput input, Action<ITile> registerLastClicked, bool goal, int tileSize) : base(coord, origin, tileSize, board, input, registerLastClicked, goal)
        {
        }

        public override void Undo()
        {
            Activated = !Activated;
        }

        protected override void ActivationAction(bool activated)
        {
            if (activated)
            {
                Rect.FillColor = new Color(0, 255, 0);
            }
            else
            {
                Rect.FillColor = new Color(50, 50, 50, 50);
            }

        }

        protected override void DrawMe(RenderTarget target, RenderStates states)
        {
            //
        }
    }
}