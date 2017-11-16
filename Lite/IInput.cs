using System;
using SFML.Window;

namespace Lite
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
        void Release();
        event Action<KeyEventArgs> KeyPressed;
    }
}