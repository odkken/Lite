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
        private readonly Font _font;
        private readonly uint _charSize;

        public WrappedTextRenderer(Func<FloatRect> getBounds, Font font, uint charSize, Dictionary<Tag, Color> colorLookup)
        {
            _getBounds = getBounds;
            _font = font;
            _charSize = charSize;
            _colorLookup = colorLookup;
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            AlignTexts();
            _texts.ForEach(a => a.Text.Draw(target, states));
        }

        void AlignTexts()
        {
            var spacing = _font.GetLineSpacing(_charSize);
            var bounds = _getBounds();
            for (int i = 0; i < _texts.Count; i++)
            {
                var text = _texts[i].Text;
                if (i == 0)
                    text.Position = new Vector2f((int)bounds.Left, (int)(bounds.Top + bounds.Height - spacing * _texts[i].NumLines - spacing * .5f));
                else
                    text.Position = new Vector2f((int)bounds.Left, (int)(_texts[i - 1].Text.Position.Y - spacing * _texts[i].NumLines));
            }
        }

        class WrappedTextItem
        {
            public Text Text { get; set; }
            public Tag Tag { get; set; }
            public int NumLines { get; set; }
        }

        private readonly List<Tuple<string, Tag>> _receivedLines = new List<Tuple<string, Tag>>();
        private List<WrappedTextItem> _texts = new List<WrappedTextItem>();
        public void AddLine(string line, Tag tag)
        {
            var bounds = _getBounds();
            var roughCharacterLimit = (int)(2 * bounds.Width * bounds.Height / (_charSize * _charSize));
            _receivedLines.Add(Tuple.Create(line, tag));
            _texts.Clear();
            for (var index = _receivedLines.Count - 1; index >= 0; index--)
            {
                var currentText = _receivedLines[index].Item1;
                var currentTag = _receivedLines[index].Item2;
                if (currentText.Length > roughCharacterLimit)
                    currentText = currentText.Substring(currentText.Length - roughCharacterLimit);

                Text text;
                if (_texts.Any() && _texts.Last().Tag == currentTag)
                {
                    text = _texts.Last().Text;
                }
                else
                {
                    text = new Text("", _font, _charSize) { Color = _colorLookup[currentTag] };
                    _texts.Add(new WrappedTextItem
                    {
                        NumLines = 1,
                        Tag = currentTag,
                        Text = text
                    });
                }
                var currentDisplayString = text.DisplayedString;
                if (!string.IsNullOrWhiteSpace(currentDisplayString))
                {
                    text.DisplayedString = currentText + "\n" + currentDisplayString;
                    _texts.Last().NumLines++;
                }
                else
                {
                    text.DisplayedString = currentText;
                }
                var lastSplitIndex = 0;
                var shadowString = text.DisplayedString;
                while (text.GetLocalBounds().Width > bounds.Width)
                {
                    for (; lastSplitIndex < shadowString.Length; lastSplitIndex++)
                    {
                        if (text.FindCharacterPos((uint)lastSplitIndex).X - text.Position.X > bounds.Width)
                        {
                            lastSplitIndex--;
                            shadowString = shadowString.Insert(lastSplitIndex, "\n");
                            text.DisplayedString = shadowString;
                            _texts.Last().NumLines++;
                            break;
                        }
                    }
                }
                AlignTexts();
                if (text.GetGlobalBounds().Top < bounds.Top)
                {
                    _receivedLines.RemoveRange(0, index);
                    break;
                }
            }
        }

        private Dictionary<Tag, Color> _colorLookup;
    }
}