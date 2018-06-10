using System;
using SFML.Window;

namespace Lite.Lib.GameCore
{
    public interface IInput
    {
        event Action<TextEventArgs> TextEntered;
        bool IsControlDown { get; }
        bool IsShiftDown { get; }
        event Action<MouseMoveEventArgs> MouseMoved;
        event Action<MouseButtonEventArgs> MouseButtonDown;
        event Action<MouseButtonEventArgs> MouseButtonUp;
        event Action<MouseWheelEventArgs> MouseWheelScrolled;
        void Consume();
        void Consume(params Keyboard.Key[] keys);
        void Release();
        event Action<KeyEventArgs> KeyPressed;
        bool IsKeyDown(Keyboard.Key key);
    }
}