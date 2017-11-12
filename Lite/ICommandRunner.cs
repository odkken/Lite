using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace Lite
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