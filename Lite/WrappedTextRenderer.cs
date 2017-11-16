using System;
using System.Collections.Generic;
using System.Linq;
using SFML.Graphics;
using SFML.System;

namespace Lite
{

    public class WrappedTextRenderer : IWrappedTextRenderer
    {
        private readonly Func<FloatRect> _getBounds;
        private readonly Func<Vector2f> _getWindowDimensions;
        private readonly Font _font;
        private readonly uint _charSize;

        public WrappedTextRenderer(Func<FloatRect> getBounds, Func<Vector2f> getWindowDimensions, Font font, uint charSize, Dictionary<Tag, Color> colorLookup)
        {
            _getBounds = getBounds;
            _getWindowDimensions = getWindowDimensions;
            _font = font;
            _charSize = charSize;
            _colorLookup = colorLookup;
            _textViewport = new View(new FloatRect(_textOrigin, new Vector2f(getBounds().Width, getBounds().Height)));
        }

        private readonly View _textViewport;

        public void Draw(RenderTarget target, RenderStates states)
        {
            var bounds = _getBounds();
            var windowSize = _getWindowDimensions();
            _textViewport.Viewport = new FloatRect(bounds.Left / windowSize.X, bounds.Top / windowSize.Y, bounds.Width / windowSize.X, bounds.Height / windowSize.Y);
            target.SetView(_textViewport);
            _textsToDraw.ForEach(a => a.Text.Draw(target, states));
            target.SetView(target.DefaultView);
        }

        class WrappedTextItem
        {
            public Text Text { get; set; }
            public Tag Tag { get; set; }
            public int NumLines { get; set; }
        }

        readonly Vector2f _textOrigin = new Vector2f(200000, 200000);
        private readonly List<Tuple<string, Tag>> _receivedLines = new List<Tuple<string, Tag>>();
        private readonly List<WrappedTextItem> _texts = new List<WrappedTextItem>();

        public void AddLine(string line, Tag tag)
        {
            _receivedLines.Add(Tuple.Create(line, tag));

            var shadowString = line;
            var newText = new Text(shadowString, _font, _charSize) { Color = _colorLookup[tag] };
            var lastSplitIndex = 0;

            var bounds = _getBounds();
            while (newText.GetLocalBounds().Width > bounds.Width)
            {
                for (; lastSplitIndex < shadowString.Length; lastSplitIndex++)
                {
                    if (newText.FindCharacterPos((uint)lastSplitIndex).X - newText.Position.X > bounds.Width)
                    {
                        lastSplitIndex--;
                        shadowString = shadowString.Insert(lastSplitIndex, "\n ");
                        newText.DisplayedString = shadowString;
                        break;
                    }
                }
            }

            var spacing = _font.GetLineSpacing(_charSize);
            if (_texts.Any())
            {
                var lastText = _texts.Last();
                newText.Position = new Vector2f(0, lastText.Text.Position.Y + lastText.NumLines * spacing);
            }
            else
                newText.Position = _textOrigin;

            _texts.Add(new WrappedTextItem
            {
                NumLines = shadowString.Count(a => a == '\n') + 1,
                Tag = tag,
                Text = newText
            });
            MoveViewportToBottom();
        }

        private readonly int scrollAmount = 50;
        public void ScrollUp()
        {
            MoveViewport(new Vector2f(0, -scrollAmount));
            ClampViewportPos();
        }

        FloatRect CreateViewRect(Vector2f origin)
        {
            var bounds = _getBounds();
            return new FloatRect(origin, new Vector2f(bounds.Width, bounds.Height));
        }

        public void ScrollDown()
        {
            MoveViewport(new Vector2f(0, scrollAmount));
            ClampViewportPos();
        }

        void MoveViewportToBottom()
        {
            var bottomOfText = 0;
            if (_texts.Any())
            {
                var lowestText = _texts.Last().Text.GetGlobalBounds();
                bottomOfText = (int)(lowestText.Top + lowestText.Height);
            }
            var bounds = _getBounds();
            var bottom = new Vector2f(bounds.Left, bottomOfText);
            MoveViewportTo(bottom - new Vector2f(0, bounds.Height));
        }

        void MoveViewport(Vector2f delta)
        {
            _textViewport.Move(delta);
            CalculateDrawnText();
        }

        void MoveViewportTo(Vector2f position)
        {
            _textViewport.Reset(CreateViewRect(position));
            CalculateDrawnText();
        }

        void CalculateDrawnText()
        {
            _textsToDraw = _texts.Where(a => a.Text.GetGlobalBounds().Intersects(GetViewportTargetRegion())).ToList();
        }

        void ClampViewportPos()
        {
            if (!_texts.Any())
            {
                MoveViewportToBottom();
            }
            else
            {
                var firstTextBounds = _texts.First().Text.GetGlobalBounds();
                var viewportRegion = GetViewportTargetRegion();
                if (viewportRegion.Top < firstTextBounds.Top)
                {
                    _textViewport.Move(new Vector2f(0, firstTextBounds.Top - viewportRegion.Top));
                }
                viewportRegion = GetViewportTargetRegion();
                var lastTextBounds = _texts.Last().Text.GetGlobalBounds();
                if (viewportRegion.Bottom().Y > lastTextBounds.Bottom().Y)
                {
                    _textViewport.Move(new Vector2f(0, lastTextBounds.Bottom().Y - viewportRegion.Bottom().Y));
                }
            }
            CalculateDrawnText();
        }

        FloatRect GetViewportTargetRegion()
        {
            var center = _textViewport.Center;
            var size = _textViewport.Size;
            return new FloatRect(new Vector2f(center.X - size.X / 2, center.Y - size.Y / 2), size);
        }

        private readonly Dictionary<Tag, Color> _colorLookup;
        private List<WrappedTextItem> _textsToDraw = new List<WrappedTextItem>();
    }
}