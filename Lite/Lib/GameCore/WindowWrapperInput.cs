using System;
using System.Collections.Generic;
using System.Numerics;
using SFML.System;
using SFML.Window;

namespace Lite.Lib.GameCore
{
    public class WindowWrapperInput : IInput
    {
        private Dictionary<Keyboard.Key, bool> _lastFramePressLookup = new Dictionary<Keyboard.Key, bool>();
        private Window _window;
        public event Action<TextEventArgs> TextEntered;
        public bool IsControlDown => Keyboard.IsKeyPressed(Keyboard.Key.LControl) || Keyboard.IsKeyPressed(Keyboard.Key.RControl);
        public bool IsShiftDown => Keyboard.IsKeyPressed(Keyboard.Key.LShift) || Keyboard.IsKeyPressed(Keyboard.Key.RShift);
        public event Action<MouseMoveEventArgs> MouseMoved;
        public event Action<MouseButtonEventArgs> MouseButtonDown;
        public event Action<MouseButtonEventArgs> MouseButtonUp;
        public event Action<MouseWheelEventArgs> MouseWheelScrolled;

        public void Update(float dt)
        {
            foreach (Keyboard.Key key in Enum.GetValues(typeof(Keyboard.Key)))
            {
                _lastFramePressLookup[key] = IsKeyDown(key);
            }
        }

        public void Consume()
        {

        }

        public void Consume(params Keyboard.Key[] keys)
        {

        }

        public void Release()
        {

        }

        public event Action<KeyEventArgs> KeyPressed;
        public bool IsKeyDown(Keyboard.Key key)
        {
            return Keyboard.IsKeyPressed(key);
        }

        public Vector2f GetMousePos()
        {
            var mpos = GetMousePosAbsolute();
            return new Vector2f((float) (mpos.X * 1.0 / _window.Size.X), (float) (mpos.Y * 1.0 / _window.Size.Y));
        }

        public bool WasKeyPressed(Keyboard.Key key)
        {
            return IsKeyDown(key) && !_lastFramePressLookup[key];
        }

        public Vector2i GetMousePosAbsolute()
        {
            return Mouse.GetPosition(_window);
        }

        public WindowWrapperInput(Window window)
        {
            _window = window;
            window.TextEntered += (sender, args) => TextEntered?.Invoke(args);
            window.KeyPressed += (sender, args) => KeyPressed?.Invoke(args);
            window.MouseButtonPressed += (sender, args) => MouseButtonDown?.Invoke(args);
            window.MouseButtonReleased += (sender, args) => MouseButtonUp?.Invoke(args);
            window.MouseMoved += (sender, args) => MouseMoved?.Invoke(args);
            window.MouseWheelMoved += (sender, args) => MouseWheelScrolled?.Invoke(args);
        }
    }
}