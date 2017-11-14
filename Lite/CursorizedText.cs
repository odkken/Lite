using System;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;
using SFML.Graphics;
using SFML.System;

namespace Lite
{
    public class CursorizedText : ICursorizedText
    {
        private readonly uint _characterSize;
        private readonly RectangleShape _cursor;
        private readonly Func<FloatRect> _getBounds;
        private readonly Func<Color> _getCursorColor;
        private readonly RectangleShape _selectionRect;

        private readonly Text _text;
        private int _cursorIndex;

        private bool _selectionActive;
        private int _selectionOrigin;

        private RectangleShape line;
        private List<Action> _history = new List<Action>();

        public CursorizedText(Text text, Func<FloatRect> getBounds, uint characterSize, Func<Color> getCursorColor)
        {
            _text = text;
            _getBounds = getBounds;
            _characterSize = characterSize;
            _getCursorColor = getCursorColor;
            _cursor = new RectangleShape();
            _selectionRect = new RectangleShape { FillColor = new Color(100, 100, 100, 100) };
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            var cursorScale = .75f;
            var bounds = _getBounds();
            var padding = _characterSize * .2f;
            _text.Position = new Vector2f(bounds.Left + padding, bounds.Top);
            var textBounds = _text.GetGlobalBounds();
            if (textBounds.Left + textBounds.Width > bounds.Width - padding)
                _text.Position -= new Vector2f(textBounds.Left + textBounds.Width - (bounds.Width) + padding, 0);
            _text.Draw(target, states);
            var cursorHeight = bounds.Height * cursorScale;
            if (_selectionActive)
            {
                var leftmostSpot = _text.FindCharacterPos((uint)Math.Min(_cursorIndex, _selectionOrigin)).X;
                var rightmostSpot = _text.FindCharacterPos((uint)Math.Max(_cursorIndex, _selectionOrigin)).X;
                _selectionRect.Size = new Vector2f(rightmostSpot - leftmostSpot, cursorHeight);
                _selectionRect.Position = new Vector2f(+_text.Position.X + leftmostSpot,
                    bounds.Top + bounds.Height * (1 - cursorScale) / 2f);
                _selectionRect.Draw(target, states);
            }
            _cursor.Size = new Vector2f(1, cursorHeight);
            _cursor.Position = new Vector2f(_text.FindCharacterPos((uint)_cursorIndex).X + _text.Position.X,
                bounds.Top + bounds.Height * (1 - cursorScale) / 2f);
            _cursor.FillColor = _getCursorColor();
            _cursor.Draw(target, states);
            line?.Draw(target, states);
        }

        public void AdvanceCursor(bool control, bool shift)
        {
            HandleSelection(shift);
            if (control)
                _cursorIndex = FindFollowingWordEnd();
            else
                _cursorIndex++;
            ClampCursor();
        }

        public void RecedeCursor(bool control, bool shift)
        {
            HandleSelection(shift);
            if (control)
                _cursorIndex = FindPrecedingWordEnd();
            else
                _cursorIndex--;
            ClampCursor();
        }

        public void Home(bool shift)
        {
            HandleSelection(shift);
            _cursorIndex = 0;
        }

        public void End(bool shift)
        {
            HandleSelection(shift);
            _cursorIndex = _text.DisplayedString.Length;
        }

        private int _historyIndex = 0;
        private bool _mouseClickedOnUs;

        public void Undo()
        {
            //if(_historyIndex == -1)
            //_history[_historyIndex].Invoke();
        }

        void AddCommandToHistory(Action command)
        {
            _history.Add(command);
            _historyIndex = _history.Count - 1;
        }

        public void Redo()
        {
            //throw new NotImplementedException();
        }

        public void HandleMouseDown(Vector2f position, bool shift)
        {
            var inputRegionBounds = _getBounds();
            if (position.Y < inputRegionBounds.Top || position.Y > inputRegionBounds.Top + inputRegionBounds.Height)
            {
                _mouseClickedOnUs = false;
                return;
            }
            _mouseClickedOnUs = true;
            HandleSelection(shift);
            MoveCursorToClosestCharacter(position);
            if (!shift)
                _selectionOrigin = _cursorIndex;

        }

        void MoveCursorToClosestCharacter(Vector2f position)
        {
            var indeces = Enumerable.Range(0, _text.DisplayedString.Length + 1).ToDictionary(a => a, a => _text.Position + _text.FindCharacterPos((uint)a));
            var closest = indeces.MinBy(a => (a.Value - position).SquareMagnitude());
            _cursorIndex = closest.Key;
        }

        public void HandleMouseUp(Vector2f position)
        {
            _mouseClickedOnUs = false;
        }

        public void HandleMouseMoved(Vector2f position)
        {
            if (!_mouseClickedOnUs) return;

            var oldPos = _cursorIndex;
            MoveCursorToClosestCharacter(position);
            if (oldPos != _cursorIndex)
                _selectionActive = true;
        }

        public void SetHighlightColor(Color color)
        {
            _selectionRect.FillColor = color;
        }

        public string SelectedText
        {
            get
            {
                var leftMost = Math.Min(_cursorIndex, _selectionOrigin);
                var rightMost = Math.Max(_cursorIndex, _selectionOrigin);
                return _text.DisplayedString.Substring(leftMost, rightMost - leftMost);
            }
        }

        public void Delete()
        {
            if (_selectionActive)
            {
                DeleteSelected();
            }
            else
            {
                if (_cursorIndex == _text.DisplayedString.Length) return;
                _text.DisplayedString = _text.DisplayedString.Remove(_cursorIndex, 1);
            }
            ClampCursor();
        }

        public void SetString(string s)
        {
            _selectionActive = false;
            _text.DisplayedString = s;
            _cursorIndex = _text.DisplayedString.Length;
        }

        public void SelectAll()
        {
            _selectionActive = true;
            _selectionOrigin = 0;
            _cursorIndex = _text.DisplayedString.Length;
        }

        public void Backspace()
        {
            if (_selectionActive)
            {
                DeleteSelected();
            }
            else
            {
                if (_cursorIndex == 0) return;
                _text.DisplayedString = _text.DisplayedString.Remove(_cursorIndex - 1, 1);
                _cursorIndex--;
            }
            ClampCursor();
        }

        public void AddString(string text)
        {
            if (_selectionActive)
                DeleteSelected();
            _text.DisplayedString = _text.DisplayedString.Insert(_cursorIndex, text);
            _cursorIndex += text.Length;
            ClampCursor();
        }

        public override string ToString()
        {
            return _text.DisplayedString;
        }

        private int FindFollowingWordEnd()
        {
            if (_cursorIndex == _text.DisplayedString.Length)
                return _cursorIndex;
            var isOnWhitespace = char.IsWhiteSpace(_text.DisplayedString[_cursorIndex]);
            var foundIndex = _text.DisplayedString.Substring(_cursorIndex).ToList()
                .FindIndex(a => char.IsWhiteSpace(a) != isOnWhitespace);
            if (foundIndex == -1)
                return _text.DisplayedString.Length;
            return _cursorIndex + foundIndex;
        }

        private void ClampCursor()
        {
            _cursorIndex = Math.Clamp(_cursorIndex, 0, _text.DisplayedString.Length);
        }

        private int FindPrecedingWordEnd()
        {
            if (_cursorIndex == 0)
                return _cursorIndex;
            var isOnWhitespace = char.IsWhiteSpace(_text.DisplayedString[_cursorIndex - 1]);
            return _text.DisplayedString.Substring(0, _cursorIndex).ToList()
                .FindLastIndex(a => char.IsWhiteSpace(a) != isOnWhitespace) + 1;
        }

        private void HandleSelection(bool shift)
        {
            if (shift)
            {
                if (!_selectionActive)
                    _selectionOrigin = _cursorIndex;
                _selectionActive = true;
            }
            else
            {
                _selectionActive = false;
            }
        }

        private void DeleteSelected()
        {
            var leftmost = Math.Min(_cursorIndex, _selectionOrigin);
            var range = Math.Abs(_cursorIndex - _selectionOrigin);
            _text.DisplayedString = _text.DisplayedString.Remove(leftmost, Math.Min(range, _text.DisplayedString.Length - leftmost));
            _cursorIndex = leftmost;
            _selectionActive = false;
        }
    }
}