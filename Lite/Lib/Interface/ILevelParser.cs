using System;
using System.Collections.Generic;
using SFML.System;

namespace Lite.Lib.Interface
{
    public interface ILevelParser
    {
        (List<ITile>, Character, Vector2i, Func<Vector2i, Vector2f>) Parse(string levelName);
    }
}