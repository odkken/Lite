using System;
using System.Linq;
using Lite.Lib.GameCore;
using SFML.Graphics;
using SFML.System;

namespace Lite.Lib.Entities
{
    public abstract class Entity : Drawable
    {
        private static int _guid;
        private static Action<Entity> _entityRegistration;

        public static void SetActions(Action<Entity> entityRegistration, Func<Entity, Vector2i, Vector2i> setPositionFunc)
        {
            if (_entityRegistration != null)
                return;
            _entityRegistration = entityRegistration;
            _setPositionFunc = setPositionFunc;
        }

        public int Id { get; }

        protected Entity()
        {
            Id = _guid++;
            _entityRegistration(this);
        }

        private bool alive = true;
        private Vector2i _position;

        public void Destroy()
        {
            alive = false;
            DestroyMe();
        }

        protected IWorld GameWorld => Core.World;

        protected abstract void DestroyMe();

        public override string ToString()
        {
            var props = this.GetType().GetProperties();
            return string.Join(";", props.Select(a => $"[{a.Name} = {a.GetValue(this)}]"));
        }

        protected ILogger Logger => Core.Logger;
        ~Entity()
        {
            Logger.Log("Destroying " + this, Category.SuperLowDebug);
            Program.LogGcFrame();
        }

        public virtual Vector2i Position
        {
            get => _position;
            set => _position = _setPositionFunc(this, value);
        }

        private static Func<Entity, Vector2i, Vector2i> _setPositionFunc;

        protected abstract void DrawMe(RenderTarget target, RenderStates states);
        protected abstract void UpdateMe();

        public void Update()
        {
            if (!alive)
                return;
            UpdateMe();
        }
        public void Draw(RenderTarget target, RenderStates states)
        {
            if (!alive)
                return;
            DrawMe(target, states);
        }
    }
}