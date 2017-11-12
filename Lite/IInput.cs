using System;
using SFML.Window;

namespace Lite
{
    public interface IInput
    {
        event Action<TextEventArgs> TextEntered;
        void Consume();
        void Release();
        event Action<KeyEventArgs> KeyPressed;
    }
}