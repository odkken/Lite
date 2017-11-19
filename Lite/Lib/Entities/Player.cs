using System;
using System.Data;
using Lite.Lib.GameCore;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Lite.Lib.Entities
{
    public class Player : Entity
    {
        public Player()
        {
            Core.Input.KeyPressed += args =>
            {
                switch (args.Code)
                {
                    case Keyboard.Key.W:
                    case Keyboard.Key.Up:
                        Move(new Vector2i(0, -1));
                        break;
                    case Keyboard.Key.A:
                    case Keyboard.Key.Left:
                        Move(new Vector2i(-1, 0));
                        break;
                    case Keyboard.Key.D:
                    case Keyboard.Key.Right:
                        Move(new Vector2i(1, 0));
                        break;
                    case Keyboard.Key.S:
                    case Keyboard.Key.Down:
                        Move(new Vector2i(0, 1));
                        break;
                }
            };
            shape = new CircleShape(){FillColor = Color.White};
        }

        void Move(Vector2i delta)
        {
            var tileState = Core.World.GetTileState(Position + delta);
            switch (tileState)
            {
                case TileState.Walkable:
                    //Position += delta;
                    break;
                case TileState.Pit:
                    //if(HoldingBlock)
                    break;
                case TileState.Wall:
                    break;
                case TileState.Door:
                    break;
                case TileState.Block:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private CircleShape shape;

        public bool HoldingBlock { get; set; }
        public override void Draw(RenderTarget target, RenderStates states)
        {
            //shape.Radius = 
        }

        public override void Update()
        {
            
        }
    }
}