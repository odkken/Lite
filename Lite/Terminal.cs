using System;
using System.Collections.Generic;
using System.Linq;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Lite
{
    public enum OpenState
    {
        Closed,
        Open
    }

    public class EmphasizedText : Drawable
    {
        private readonly Text _emphasis;
        private readonly Func<uint> _getCharacterSize;

        public EmphasizedText(Text text, Func<uint> getCharacterSize)
        {
            Text = text;
            _getCharacterSize = getCharacterSize;
            _emphasis = new Text(text) { Color = new Color(0, 0, 0, (byte)(255 * .75)) };
        }

        public Text Text { get; }

        public void Draw(RenderTarget target, RenderStates states)
        {
            var offset = _getCharacterSize() * .05f;
            if (offset < 2)
                offset = 2;
            _emphasis.Position = Text.Position + new Vector2f(offset, offset);
            _emphasis.DisplayedString = Text.DisplayedString;
            _emphasis.Draw(target, states);
            Text.Draw(target, states);
        }
    }

    public class Terminal : Drawable
    {
        private static readonly uint _characterSize = 26;
        private readonly float _closedYPos;
        private readonly RectangleShape _cursor;
        private readonly RectangleShape _inputBackground;

        private readonly Color _inputBackgroundColor = new Color(42, 74, 127);

        private readonly List<string> _inputHistory;
        private readonly List<string> _reportLines;
        private readonly EmphasizedText _inputText;
        private readonly float _openingRate = 7f;

        private readonly RectangleShape _reportBackground;
        private readonly Color _reportBackgroundColor = new Color(53, 86, 140);
        private readonly Text _reportText;
        private float _currentOpenness;
        private Color _inputTextColor = new Color(255, 252, 188);
        private float _lastInputTime;
        private float _maxOpenness = .7f;
        private OpenState _state = OpenState.Closed;
        private float _targetOpenness;

        public float xOffset = _characterSize * .2f;

        public Terminal(RenderWindow window, Font font, IInput input, ICommandRunner commandRunner)
        {
            _closedYPos = window.Size.Y / 2f;
            _reportBackground = new RectangleShape(new Vector2f(window.Size.X, window.Size.Y / 2f))
            {
                FillColor = _reportBackgroundColor
            };
            _inputBackground =
                new RectangleShape(new Vector2f(window.Size.X, font.GetLineSpacing(_characterSize) * 1.1f))
                {
                    FillColor = _inputBackgroundColor
                };
            _cursor = new RectangleShape();
            var actualInputText = new Text
            {
                Font = font,
                CharacterSize = _characterSize,
                Color = _inputTextColor,
                DisplayedString = "Hi Buddy Waz Sup.<><>"
            };
            _reportLines = new List<string>
            {
                "DO U HAVE ANY COUNDOM",
                "ok, delete your archives and remember",
                "Mrs. Kayla Marie Armstrong............",
                "i love you more, then anything"
            };
            _inputHistory = new List<string>();
            _inputText = new EmphasizedText(actualInputText, () => _characterSize);
            _reportText = new Text { Font = font, CharacterSize = _characterSize };

            var toggled = false;
            input.TextEntered += args =>
            {
                if (args.Unicode == "`")
                    switch (_state)
                    {
                        case OpenState.Closed:
                            _state = OpenState.Open;
                            input.Consume();
                            toggled = true;
                            break;
                        case OpenState.Open:
                            _state = OpenState.Closed;
                            input.Release();
                            toggled = true;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                if (_state == OpenState.Open)
                {
                    _lastInputTime = Core.TimeInfo.CurrentTime;
                    if (!toggled)
                        switch (args.Unicode)
                        {
                            case "\b":
                                if (actualInputText.DisplayedString.Length != 0)
                                    actualInputText.DisplayedString =
                                        actualInputText.DisplayedString.Remove(
                                            actualInputText.DisplayedString.Length - 1);
                                break;
                            case "\r":
                                if (actualInputText.DisplayedString.Length != 0)
                                {
                                    _inputHistory.Add(actualInputText.DisplayedString);
                                    _reportLines.Add(">" + actualInputText.DisplayedString);
                                    commandRunner.RunCommand(actualInputText.DisplayedString).ForEach(_reportLines.Add);
                                    actualInputText.DisplayedString = "";
                                }
                                break;
                            default:
                                actualInputText.DisplayedString += args.Unicode;
                                break;
                        }
                    else toggled = false;
                }
                if (toggled)
                    UpdateOpenness();
            };

            input.KeyPressed += args =>
            {
                if (args.Code == Keyboard.Key.Up)
                {
                    if (_inputHistory.Count > 0)
                        _inputText.Text.DisplayedString = _inputHistory.Last();
                }
            };

        }

        public bool IsOpen => _currentOpenness > 0;

        public void Draw(RenderTarget target, RenderStates states)
        {
            UpdateOpenness();
            if (IsOpen)
            {
                _reportBackground.Position = new Vector2f(0, _closedYPos * (_currentOpenness - 1));
                var bounds = _reportBackground.GetGlobalBounds();
                _inputBackground.Position = new Vector2f(0, bounds.Top + bounds.Height);
                _reportBackground.Draw(target, states);

                _reportText.DisplayedString = "";
                var lines = _reportLines.ToList();
                for (var i = lines.Count - 1; i >= 0; i--)
                {
                    _reportText.DisplayedString = lines[i] + "\n" + _reportText.DisplayedString;
                    if (_reportText.GetLocalBounds().Height > _reportBackground.GetLocalBounds().Height)
                        break;
                }
                _reportText.Position = new Vector2f(xOffset,
                    bounds.Top + bounds.Height - _reportText.GetGlobalBounds().Height + 2 * xOffset);
                _reportText.Draw(target, states);
                _inputBackground.Draw(target, states);
                _inputText.Text.Position = _inputBackground.Position + new Vector2f(xOffset, 0);
                _inputText.Draw(target, states);
                var inputRectBounds = _inputBackground.GetGlobalBounds();
                var inputBounds = _inputText.Text.GetGlobalBounds();
                var cursorScale = .75f;
                var cursorHeight = inputRectBounds.Height * cursorScale;
                _cursor.Size = new Vector2f(2, cursorHeight);
                _cursor.Position = new Vector2f(inputBounds.Width + inputBounds.Left,
                    inputRectBounds.Top + inputRectBounds.Height * (1 - cursorScale) / 2f);

                var fraction = MathF.Sin(2 * (Core.TimeInfo.CurrentTime - _lastInputTime));
                fraction *= fraction;
                _cursor.FillColor = _inputTextColor.Lerp(_inputBackgroundColor, fraction);
                _cursor.Draw(target, states);
            }
        }

        private void UpdateOpenness()
        {
            _targetOpenness = _state == OpenState.Closed ? 0 : 1;
            var dt = Core.TimeInfo.CurrentDt;
            var dOpen = dt * _openingRate;

            if (_currentOpenness < _targetOpenness)
            {
                _currentOpenness += dOpen;
                if (_currentOpenness > _targetOpenness)
                    _currentOpenness = _targetOpenness;
            }
            else if (_currentOpenness > _targetOpenness)
            {
                _currentOpenness -= dOpen;
                if (_currentOpenness < _targetOpenness)
                    _currentOpenness = _targetOpenness;
            }
        }
    }
}