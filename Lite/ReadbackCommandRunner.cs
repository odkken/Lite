using System.Collections.Generic;

namespace Lite
{
    public class ReadbackCommandRunner : ICommandRunner
    {
        public List<string> RunCommand(string command)
        {
            return new List<string> { $"Running: {command}" };
        }
    }
}