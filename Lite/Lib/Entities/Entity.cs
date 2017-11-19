using System;
using Lite.Lib.GameCore;
using SFML.Graphics;
using SFML.System;

namespace Lite.Lib.Entities
{
    public abstract class Entity : Drawable
    {
        private static int _guid;
        private static Action<Entity> _entityRegistration;

        public static void SetRegistrationAction(Action<Entity> entityRegistration)
        {
            if (_entityRegistration != null)
                return;
            _entityRegistration = entityRegistration;
        }

        public int Id { get; }

        protected Entity()
        {
            Id = _guid++;
            _entityRegistration(this);
        }

        public Vector2i Position => Core.World.GetCoordOfEntity(this);
        public abstract void Draw(RenderTarget target, RenderStates states);
        public abstract void Update();
    }
}