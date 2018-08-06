using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Lite.Lib;
using Lite.Lib.Interface;

namespace Lite
{
    public class TextLevelSaver : ILevelSaver
    {
        private readonly Func<TileType, char> _charRepresentation;
        private string _levelsDirectory;

        public TextLevelSaver(Func<TileType, char> charRepresentation, string levelsDirectory)
        {
            _charRepresentation = charRepresentation;
            _levelsDirectory = levelsDirectory;
        }

        public void SaveLevel(IEnumerable<ITile> tiles, string name)
        {
            var cols = tiles.Max(a => a.X) + 1;
            var rows = tiles.Max(a => a.Y) + 1;
            var lines = new List<string>();
            for (int row = 0; row < rows; row++)
            {
                var line = "";
                for (int col = 0; col < cols; col++)
                {
                    var tile = tiles.SingleOrDefault(a => a.X == col && a.Y == row);
                    var type = tile?.Type ?? TileType.Unused;
                    line += _charRepresentation(type);
                }
                lines.Add(line);
            }
            File.WriteAllLines(Path.Combine(_levelsDirectory, name + ".lev"), lines);
        }
    }
}