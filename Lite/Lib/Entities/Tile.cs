using System;
using Lite.Lib.GameCore;
using Lite.Lib.Interface;
using SFML.Audio;
using SFML.Graphics;
using SFML.System;

namespace Lite.Lib.Entities
{
    public abstract class Tile : ITile
    {
        private readonly IBoard _board;
        protected readonly RectangleShape Rect;
        protected static Texture GoalTexture = new Texture("images/star.png");
        private bool _activated;
        private readonly Sprite _goalIndicator;
        private readonly bool _goal;
        protected static SoundBuffer ClickDownSb = new SoundBuffer(@"sounds/click_down.ogg");
        protected static SoundBuffer ClickUpSb = new SoundBuffer(@"sounds/click_up.ogg");
        public static Sound ClickDownSound = new Sound(ClickDownSb);
        public static Sound ClickUpSound = new Sound(ClickUpSb);
        private bool _disabled;

        protected Tile(Vector2i coord, Vector2i origin, int tileSize, IBoard board, IInput input, Action<ITile> registerLastClicked, bool goal)
        {
            _board = board;
            _goal = goal;
            Coord = coord;
            Rect = new RectangleShape(new Vector2f(tileSize, tileSize)) { Position = (Vector2f)((coord * tileSize) + origin), OutlineThickness = -2, OutlineColor = new Color(150, 150, 150, 200) };
            if (goal)
            {
                var targetSize = .5f * tileSize;
                _goalIndicator = new Sprite(GoalTexture);
                _goalIndicator.Scale *= targetSize/GoalTexture.Size.X;
                _goalIndicator.Position = Rect.Position + new Vector2f(targetSize/2, targetSize/2);
            }
            Activated = false;
            input.MouseButtonDown += args =>
            {
                if (!Rect.GetGlobalBounds().Contains(args.X, args.Y)) return;

                Activated = !Activated;
                registerLastClicked(this);
            };
        }

        public abstract void Undo();
        protected abstract void ActivationAction(bool activated);

        public void Draw(RenderTarget target, RenderStates states)
        {
            Rect.Draw(target, states);
            DrawMe(target, states);
            if (_goal)
                _goalIndicator.Draw(target, states);
        }

        protected abstract void DrawMe(RenderTarget target, RenderStates states);

        public bool Activated
        {
            get => _activated;
            set
            {
                if (_disabled)
                    return;
                _activated = value;
                if (Core.World.Initialized)
                {
                    if (_activated)
                    {
                        ClickDownSound.Play();
                    }
                    else
                    {
                        ClickUpSound.Play();
                    }
                }
                ActivationAction(value);
            }
        }

        public Vector2i Coord { get; }
        public int X => Coord.X;
        public int Y => Coord.Y;
        public bool Satisfied => _goal == Activated;
        public void Disable()
        {
            _disabled = true;
        }
    }
}