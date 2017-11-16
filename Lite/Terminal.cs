using System;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;
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

    public class Terminal : Drawable
    {
        private static readonly uint _characterSize = 16;
        private readonly float _closedYPos;
        private readonly RectangleShape _inputBackground;

        private readonly Color _inputBackgroundColor = new Color(30, 30, 30, 220);

        private readonly float _openingRate = 7f;

        private readonly RectangleShape _reportBackground;
        private readonly Color _reportBackgroundColor = new Color(10, 10, 10, 220);
        private List<string> _inputHistory = new List<string>();
        private float _currentOpenness;
        private static readonly Color InputTextColor = new Color(155, 255, 255);
        private static readonly Color ResponseTextColor = new Color(255, 155, 255);
        private float _lastInputTime;
        private OpenState _state = OpenState.Closed;
        private float _targetOpenness;

        public float xOffset = _characterSize * .2f;
        private readonly ICursorizedText _inputText;

        public void SetHighlightColor(Color color)
        {
            _inputText.SetHighlightColor(color);
        }

        private IWrappedTextRenderer reportText;
        public Terminal(RenderWindow window, Font font, IInput input, ICommandRunner commandRunner)
        {
            _closedYPos = window.Size.Y / 2f;
            _reportBackground = new RectangleShape(new Vector2f(window.Size.X, window.Size.Y / 2f))
            {
                FillColor = _reportBackgroundColor,

            };
            _inputBackground =
                new RectangleShape(new Vector2f(window.Size.X, font.GetLineSpacing(_characterSize) * 1.1f))
                {
                    FillColor = _inputBackgroundColor
                };


            reportText = new WrappedTextRenderer(() => _reportBackground.GetGlobalBounds(), () => (Vector2f)window.Size, font, _characterSize, new Dictionary<Tag, Color>
            {
                { Tag.Input, InputTextColor},
                { Tag.Response, ResponseTextColor}
            });
            _inputHistory = new List<string>();

            _inputText = new CursorizedText(new Text("", font, _characterSize) { Color = InputTextColor },
                _inputBackground.GetGlobalBounds, _characterSize,
                () =>
                {
                    var fraction = MathF.Sin(3 * (Core.TimeInfo.CurrentTime - _lastInputTime));
                    fraction *= fraction;
                    return Color.White.Lerp(new Color(255, 255, 255, 0), fraction);
                });

            var inputHistoryIndex = 0;
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

                if (_state == OpenState.Open && !input.IsControlDown)
                {
                    _lastInputTime = Core.TimeInfo.CurrentTime;
                    if (!toggled)
                        switch (args.Unicode)
                        {
                            case "\b":
                                {
                                    _inputText.Backspace();
                                }
                                break;
                            case "\r":
                                {
                                    var inputString = _inputText.ToString();
                                    if (string.IsNullOrWhiteSpace(inputString))
                                        break;
                                    _inputText.SetString("");
                                    _inputHistory.Add(inputString);
                                    inputHistoryIndex = _inputHistory.Count;
                                    reportText.AddLine(inputString, Tag.Input);
                                    commandRunner.RunCommand(inputString).ForEach(a => reportText.AddLine(a, Tag.Response));
                                }
                                break;
                            default:
                                _inputText.AddString(args.Unicode);
                                break;
                        }
                    else toggled = false;
                }
                if (toggled)
                    UpdateOpenness();
            };
            input.KeyPressed += args =>
            {
                if (_state != OpenState.Open) return;

                _lastInputTime = Core.TimeInfo.CurrentTime;
                switch (args.Code)
                {
                    case Keyboard.Key.Up:
                        if (_inputHistory.Any())
                        {
                            inputHistoryIndex--;
                            if (inputHistoryIndex < 0)
                                inputHistoryIndex = 0;
                            _inputText.SetString(_inputHistory[inputHistoryIndex]);
                        }
                        break;
                    case Keyboard.Key.Down:
                        if (_inputHistory.Any())
                        {
                            inputHistoryIndex++;
                            if (inputHistoryIndex >= _inputHistory.Count)
                                inputHistoryIndex = _inputHistory.Count - 1;
                            _inputText.SetString(_inputHistory[inputHistoryIndex]);
                        }
                        break;
                    case Keyboard.Key.Left:
                        _inputText.RecedeCursor(args.Control, args.Shift);
                        break;
                    case Keyboard.Key.Right:
                        _inputText.AdvanceCursor(args.Control, args.Shift);
                        break;
                    case Keyboard.Key.Delete:
                        _inputText.Delete();
                        break;
                    case Keyboard.Key.Home:
                        _inputText.Home(args.Shift);
                        break;
                    case Keyboard.Key.End:
                        _inputText.End(args.Shift);
                        break;
                    case Keyboard.Key.Z:
                        if (args.Control)
                            _inputText.Undo();
                        if (args.Control && args.Shift)
                            _inputText.Redo();
                        break;
                    case Keyboard.Key.A:
                        if (args.Control)
                            _inputText.SelectAll();
                        break;
                    case Keyboard.Key.C:
                        if (args.Control)
                        {
                            Clippy.PushStringToClipboard(_inputText.SelectedText);
                        }
                        break;
                    case Keyboard.Key.X:
                        if (args.Control)
                        {
                            Clippy.PushStringToClipboard(_inputText.SelectedText);
                            _inputText.Delete();
                        }
                        break;
                    case Keyboard.Key.V:
                        if (args.Control)
                            _inputText.AddString(Clippy.GetText());
                        break;
                }
            };

            input.MouseButtonDown += args => _inputText.HandleMouseDown(new Vector2f(args.X, args.Y), input.IsShiftDown);
            input.MouseButtonUp += args => _inputText.HandleMouseUp(new Vector2f(args.X, args.Y));
            input.MouseMoved += args => _inputText.HandleMouseMoved(new Vector2f(args.X, args.Y));
            input.MouseWheelScrolled += args =>
            {
                if (args.Delta > 0)
                    reportText.ScrollUp();
                else
                    reportText.ScrollDown();
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
                _inputBackground.Position = new Vector2f(0, (int)(bounds.Top + bounds.Height));
                _reportBackground.Draw(target, states);

                reportText.Draw(target, states);
                _inputBackground.Draw(target, states);
                _inputText.Draw(target, states);
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