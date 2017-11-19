using System;
using System.ComponentModel.Design.Serialization;
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
            Core.Input.KeyPressed += OnInputOnKeyPressed;
            shape = new CircleShape { FillColor = Color.White, Radius = 50 };
        }

        private void OnInputOnKeyPressed(KeyEventArgs args)
        {
            switch (args.Code)
            {
                case Keyboard.Key.W:
                case Keyboard.Key.Up:
                    EnterDoor();
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
        }

        private void EnterDoor()
        {
            var tileType = Core.World.GetTileAt(Position);
            if (tileType is Door)
                GameWorld.LoadNextLevel();
        }


        protected override void DestroyMe()
        {
            Core.Input.KeyPressed -= OnInputOnKeyPressed;
        }

        ~Player()
        {
            Logger.Log("Destroyed Player", Category.SuperLowDebug);
        }


        void Move(Vector2i delta)
        {
            var tileType = Core.World.GetTileAt(Position + delta);
            switch (tileType)
            {
                case Wall wall:
                    break;
                case Empty empty:
                    {
                        var contents = empty.Contents;
                        if (empty.Contents is Box)
                        {
                            var destinationPushTile = Core.World.GetTileAt(Position + delta * 2);
                            if (destinationPushTile is Empty && (destinationPushTile as Empty).Contents == null)
                            {
                                empty.Contents.Position += delta;
                                TryToMoveTo(Position + delta);
                            }
                        }
                        else
                        {
                            TryToMoveTo(Position + delta);
                        }
                    }
                    break;
                default:
                    throw new Exception();
            }
        }

        void TryToMoveTo(Vector2i destination)
        {
            var tile = Core.World.GetTileAt(destination);
            var beneath = Core.World.GetTileAt(destination + new Vector2i(0, 1));
            if ((tile as Empty)?.Contents == null)
            {
                //this block is available to move to
                if ((beneath as Empty)?.Contents != null || beneath is Wall)
                {
                    Position = destination;
                }
            }

        }

        private CircleShape shape;

        public bool HoldingBlock { get; set; }

        protected override void DrawMe(RenderTarget target, RenderStates states)
        {
            shape.Draw(target, states);
        }

        protected override void UpdateMe()
        {
            shape.Position = (Vector2f)Position * shape.Radius * 2;
        }
    }
}