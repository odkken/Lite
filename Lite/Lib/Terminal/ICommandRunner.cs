﻿using System;
using System.Collections.Generic;

namespace Lite.Lib.Terminal
{
    public interface ICommandRunner
    {
        List<string> RunCommand(string command);
    }

    public class CommandData
    {
        public Func<string[], List<string>> Invoke { get; set; }
        public string Name { get; set; }
    }
}