using System;
using SFML.Window;

namespace Lite.Lib.GameCore
{
    public class WindowWrapperInput : IInput
    {
        public event Action<TextEventArgs> TextEntered;
        public bool IsControlDown => Keyboard.IsKeyPressed(Keyboard.Key.LControl) || Keyboard.IsKeyPressed(Keyboard.Key.RControl);
        public bool IsShiftDown => Keyboard.IsKeyPressed(Keyboard.Key.LShift) || Keyboard.IsKeyPressed(Keyboard.Key.RShift);
        public event Action<MouseMoveEventArgs> MouseMoved;
        public event Action<MouseButtonEventArgs> MouseButtonDown;
        public event Action<MouseButtonEventArgs> MouseButtonUp;
        public event Action<MouseWheelEventArgs> MouseWheelScrolled;

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

        public WindowWrapperInput(Window window)
        {
            window.TextEntered += (sender, args) => TextEntered?.Invoke(args);
            window.KeyPressed += (sender, args) => KeyPressed?.Invoke(args);
            window.MouseButtonPressed += (sender, args) => MouseButtonDown?.Invoke(args);
            window.MouseButtonReleased += (sender, args) => MouseButtonUp?.Invoke(args);
            window.MouseMoved += (sender, args) => MouseMoved?.Invoke(args);
            window.MouseWheelMoved += (sender, args) => MouseWheelScrolled?.Invoke(args);
        }
    }
}