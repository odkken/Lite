using System;
using SFML.Window;

namespace Lite
{
    public class BlockableInput : IInput
    {
        private bool _blocked;
        private event Action<KeyEventArgs> BlockableKeyPressed;
        public event Action<KeyEventArgs> KeyPressed;
        public event Action<TextEventArgs> TextEntered;
        public event Action<TextEventArgs> BlockableTextEntered;

        public BlockableInput(IInput input)
        {
            input.TextEntered += OnTextEntered;
            input.KeyPressed += OnKeyPressed;
        }
        public BlockableInput(BlockableInput blockableInput)
        {
            blockableInput.BlockableTextEntered += OnTextEntered;
            blockableInput.BlockableKeyPressed += OnKeyPressed;
        }

        private void OnTextEntered(TextEventArgs args)
        {
            var initialBlocked = _blocked;
            TextEntered?.Invoke(args);

            if (!_blocked && initialBlocked == _blocked)
                BlockableTextEntered?.Invoke(args);
        }

        private void OnKeyPressed(KeyEventArgs args)
        {
            var initialBlocked = _blocked;
            KeyPressed?.Invoke(args);

            if (!_blocked && initialBlocked == _blocked)
                BlockableKeyPressed?.Invoke(args);
        }

        public void Consume()
        {
            _blocked = true;
        }

        public void Release()
        {
            _blocked = false;
        }

    }
}