using System;
using System.Collections.Generic;
using System.Linq;
using Lite.Lib.GameCore;
using SFML.Graphics;
using SFML.System;

namespace Lite.Lib
{
    class EditPanel : Drawable
    {
        private readonly Func<TileType, Color> _colorGet;

        class EditPane
        {
            private bool _isSelected;
            public RectangleShape Shape { get; set; }

            public bool IsSelected
            {
                get => _isSelected;
                set
                {
                    _isSelected = value;
                    Shape.OutlineColor = IsSelected ? Color.White : Color.Black;
                }
            }

            public TileType TileType { get; set; }
        }

        private List<EditPane> _icons = new List<EditPane>();

        public TileType SelectedTileType => _icons.Single(a => a.IsSelected).TileType;

        public EditPanel(IInput input, Func<bool> isEditing, Func<TileType, Color> colorGet)
        {
            _colorGet = colorGet;
            input.KeyPressed += args =>
            {
                if (!isEditing())
                    return;
                var num = (int)args.Code;
                if (num < 27 || num >= 27 + _icons.Count) return;
                num -= 27;
                _icons.ForEach(a => a.IsSelected = false);
                _icons[num].IsSelected = true;
            };
            AddIcon(TileType.Walkable);
            AddIcon(TileType.Goal);
            AddIcon(TileType.Key);
            AddIcon(TileType.Unused);
            AddIcon(TileType.CharacterSpawn);
        }

        void AddIcon(TileType type)
        {
            if (_icons.Any(a => a.TileType == type))
            {
                Core.Logger.Log($"Already have type {type}");
                return;
            }

            _icons.Add(new EditPane
            {
                Shape = new RectangleShape(new Vector2f(20, 20))
                {
                    Position = new Vector2f(0, 20 * _icons.Count),
                    FillColor = _colorGet(type),
                    OutlineThickness = -2
                },
                IsSelected = !_icons.Any(),
                TileType = type,
            });
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            foreach (var editPane in _icons)
            {
                editPane.Shape.Draw(target, states);
            }
        }
    }
}