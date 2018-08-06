using System.Collections.Generic;
using Lite.Lib.Interface;

namespace Lite.Lib
{
    internal interface ILevelSaver
    {
        void SaveLevel(IEnumerable<ITile> tiles, string name);
    }
}