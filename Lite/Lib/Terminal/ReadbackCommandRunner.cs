using System.Collections.Generic;

namespace Lite.Lib.Terminal
{
    public class ReadbackCommandRunner : ICommandRunner
    {
        public List<string> RunCommand(string command)
        {
            return new List<string> { $"Running: {command}" };
        }
    }
}