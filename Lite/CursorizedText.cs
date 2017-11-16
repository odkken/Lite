﻿using System;
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
            DisplayString = "";
            _getBounds = getBounds;
            _characterSize = characterSize;
            _getCursorColor = getCursorColor;
            _cursor = new RectangleShape();
            _selectionRect = new RectangleShape { FillColor = new Color(100, 100, 100, 100) };
            AlignTextAndCursor();
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            _text.Position = new Vector2f(_text.Position.X, _getBounds().Top);
            if (_selectionActive)
                _selectionRect.Draw(target, states);
            _cursor.FillColor = _getCursorColor();
            _text.Draw(target, states);
            _cursor.Draw(target, states);
            line?.Draw(target, states);
        }

        void AlignTextAndCursor()
        {
            var cursorScale = .75f;
            var bounds = _getBounds();
            var padding = _characterSize * .2f;
            var leftJustification = (int)(bounds.Left + padding);
            var rightJustification = (int)(bounds.Left + bounds.Width - padding);
            var cursorHeight = bounds.Height * cursorScale;
            _cursor.Position = new Vector2f(_text.FindCharacterPos((uint)_cursorIndex).X + _text.Position.X, bounds.Top + bounds.Height * (1 - cursorScale) / 2f);
            if (_cursor.Position.X < leftJustification)
            {
                var shift = leftJustification - (int)_text.FindCharacterPos((uint)_cursorIndex).X + 1;
                _text.Position = new Vector2f(shift, _text.Position.Y);
                _cursor.Position = new Vector2f(leftJustification, _cursor.Position.Y);
            }
            else if (_cursor.Position.X > rightJustification)
            {
                var shift = (int)_text.FindCharacterPos((uint)_cursorIndex).X - rightJustification;
                _text.Position = new Vector2f(-shift, _text.Position.Y);
                _cursor.Position = new Vector2f(rightJustification, _cursor.Position.Y);
            }
            if (_selectionActive)
            {
                var leftmostSpot = _text.FindCharacterPos((uint)Math.Min(_cursorIndex, _selectionOrigin)).X;
                var rightmostSpot = _text.FindCharacterPos((uint)Math.Max(_cursorIndex, _selectionOrigin)).X;
                _selectionRect.Size = new Vector2f(rightmostSpot - leftmostSpot, cursorHeight);
                _selectionRect.Position = new Vector2f(+_text.Position.X + leftmostSpot,
                    bounds.Top + bounds.Height * (1 - cursorScale) / 2f);
            }
            _cursor.Size = new Vector2f(1, cursorHeight);
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
            AlignTextAndCursor();
        }

        public void End(bool shift)
        {
            HandleSelection(shift);
            _cursorIndex = DisplayString.Length;
            AlignTextAndCursor();
        }

        private int _historyIndex = 0;
        private bool _mouseClickedOnUs;
        private string _displayString;

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
            var indeces = Enumerable.Range(0, DisplayString.Length + 1).ToDictionary(a => a, a => _text.Position + _text.FindCharacterPos((uint)a));
            var closest = indeces.MinBy(a => (a.Value - position).SquareMagnitude());
            _cursorIndex = closest.Key;
            AlignTextAndCursor();
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
                return DisplayString.Substring(leftMost, rightMost - leftMost);
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
                if (_cursorIndex == DisplayString.Length) return;
                DisplayString = DisplayString.Remove(_cursorIndex, 1);
            }
            ClampCursor();
        }

        public void SetString(string s)
        {
            _selectionActive = false;
            DisplayString = s;
            _cursorIndex = DisplayString.Length;
            AlignTextAndCursor();
        }

        public void SelectAll()
        {
            _selectionActive = true;
            _selectionOrigin = DisplayString.Length;
            _cursorIndex = 0;
            AlignTextAndCursor();
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
                DisplayString = DisplayString.Remove(_cursorIndex - 1, 1);
                _cursorIndex--;
            }
            ClampCursor();
        }

        public void AddString(string text)
        {
            if (_selectionActive)
                DeleteSelected();
            DisplayString = DisplayString.Insert(_cursorIndex, text);
            DisplayString = DisplayString.Substring(0, Math.Min(2500, DisplayString.Length));
            _cursorIndex += text.Length;
            ClampCursor();
        }

        public override string ToString()
        {
            return DisplayString;
        }

        private int FindFollowingWordEnd()
        {
            if (_cursorIndex == DisplayString.Length)
                return _cursorIndex;
            var isOnWhitespace = char.IsWhiteSpace(DisplayString[_cursorIndex]);
            var foundIndex = DisplayString.Substring(_cursorIndex).ToList()
                .FindIndex(a => char.IsWhiteSpace(a) != isOnWhitespace);
            if (foundIndex == -1)
                return DisplayString.Length;
            return _cursorIndex + foundIndex;
        }

        private void ClampCursor()
        {
            _cursorIndex = Math.Clamp(_cursorIndex, 0, DisplayString.Length);
            AlignTextAndCursor();
        }

        private int FindPrecedingWordEnd()
        {
            if (_cursorIndex == 0)
                return _cursorIndex;
            var isOnWhitespace = char.IsWhiteSpace(DisplayString[_cursorIndex - 1]);
            return DisplayString.Substring(0, _cursorIndex).ToList()
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
            DisplayString = DisplayString.Remove(leftmost, Math.Min(range, DisplayString.Length - leftmost));
            _cursorIndex = leftmost;
            _selectionActive = false;
            AlignTextAndCursor();
        }

        private string DisplayString
        {
            get => _displayString;
            set
            {
                _displayString = value;
                _text.DisplayedString = value.Replace("\n", "");
            }
        }
    }
}