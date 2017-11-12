using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Lite
{
    public class CommandExtractor
    {
        public List<CommandData> GetAllStaticCommands(Assembly ass)
        {
            var commands = new List<CommandData>();
            foreach (var type in ass.GetTypes())
            {
                var typesCommands = type.GetMethods().Where(a => a.CustomAttributes.Any(b => b.AttributeType == typeof(CommandAttribute)));
                foreach (var method in typesCommands)
                {
                    if (!method.IsStatic)
                    {
                        Console.WriteLine($"Can't register command {method.Name} because it isn't static.");
                        continue;
                    }
                    commands.Add(new CommandData
                    {
                        Name = method.Name,
                        Invoke = strings =>
                        {
                            var parms = method.GetParameters();
                            if (parms.Length != strings.Length)
                                return new List<string>
                                {
                                    $"{method.Name} argument mismatch.  Expected <{string.Join(",", parms.Select(a=> a.ParameterType))}>"
                                };
                            var typedArgs = new List<object>();
                            var parameterErrors = new List<string>();
                            for (int i = 0; i < parms.Length; i++)
                            {
                                try
                                {
                                    typedArgs.Add(Convert.ChangeType(strings[i], parms[i].ParameterType));
                                }
                                catch (Exception e)
                                {
                                    parameterErrors.Add($"Error parsing {strings[i]} to type {parms[i].ParameterType}:");
                                    parameterErrors.Add(e.Message);
                                }
                            }
                            if (parameterErrors.Any())
                                return parameterErrors;

                            var returnVal = method.Invoke(null, typedArgs.ToArray());

                            if (returnVal is IEnumerable<object>)
                                return (returnVal as IEnumerable<object>).Select(a => a.ToString()).ToList();
                            return new List<string> { returnVal.ToString() };
                        }
                    });
                }
            }
            return commands;
        }
    }
}