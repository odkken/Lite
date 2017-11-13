using System;
using SFML.Window;

namespace Lite
{
    public class BlockableInput : IInput
    {
        private bool _blocked;

        private readonly Func<bool> _isControlDown;
        public bool IsControlDown => _isControlDown();

        private readonly Func<bool> _isShiftDown;
        public bool IsShiftDown => _isShiftDown();
        public event Action<MouseMoveEventArgs> MouseMoved;
        public event Action<MouseMoveEventArgs> BlockableMouseMoved;
        public event Action<MouseButtonEventArgs> MouseButtonDown;
        public event Action<MouseButtonEventArgs> BlockableMouseButtonDown;
        public event Action<MouseButtonEventArgs> MouseButtonUp;
        public event Action<MouseButtonEventArgs> BlockableMouseButtonUp;
        private event Action<KeyEventArgs> BlockableKeyPressed;
        public event Action<KeyEventArgs> KeyPressed;
        public event Action<TextEventArgs> TextEntered;
        public event Action<TextEventArgs> BlockableTextEntered;

        public BlockableInput(IInput input)
        {
            input.TextEntered += OnTextEntered;
            input.KeyPressed += OnKeyPressed;
            input.MouseButtonDown += OnMouseDown;
            input.MouseButtonUp += OnMouseUp;
            input.MouseMoved += OnMouseMoved;
            _isControlDown = () => input.IsControlDown;
            _isShiftDown = () => input.IsShiftDown;
        }

        private void OnMouseMoved(MouseMoveEventArgs args)
        {
            MouseMoved?.Invoke(args);
            if (!_blocked)
                BlockableMouseMoved?.Invoke(args);
        }

        public BlockableInput(BlockableInput blockableInput)
        {
            blockableInput.BlockableTextEntered += OnTextEntered;
            blockableInput.BlockableKeyPressed += OnKeyPressed;
            blockableInput.BlockableMouseButtonDown += OnMouseDown;
            blockableInput.BlockableMouseButtonUp += OnMouseUp;
            blockableInput.BlockableMouseMoved += OnMouseMoved;
            _isControlDown = () => blockableInput.BlockedIsControlDown;
            _isShiftDown = () => blockableInput.BlockedIsShiftDown;
        }

        public bool BlockedIsShiftDown => !_blocked && _isShiftDown();

        public bool BlockedIsControlDown => !_blocked && _isControlDown();

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

        private void OnMouseDown(MouseButtonEventArgs args)
        {
            MouseButtonDown?.Invoke(args);
            if (!_blocked)
                BlockableMouseButtonDown?.Invoke(args);
        }

        private void OnMouseUp(MouseButtonEventArgs args)
        {
            MouseButtonUp?.Invoke(args);
            if (!_blocked)
                BlockableMouseButtonUp?.Invoke(args);
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