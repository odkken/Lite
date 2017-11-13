using System.Collections.Generic;
using System.Text;
using SFML.Graphics;
using SFML.System;

namespace Lite
{
    public interface ICursorizedText : Drawable
    {
        void AdvanceCursor(bool control, bool shift);
        void Delete();
        void SetString(string s);
        void SelectAll();
        void Backspace();
        void AddString(string text);
        void RecedeCursor(bool control, bool shift);
        void Home(bool shift);
        void End(bool shift);
        void Undo();
        void Redo();
        void HandleMouseDown(Vector2f position, bool shift);
        void HandleMouseUp(Vector2f position);
        void HandleMouseMoved(Vector2f position);
    }
}
