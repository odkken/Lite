#r @"bin\Debug\netcoreapp2.0\Lite.dll"
#r @"sfml\sfmlnet-window-2.dll"
#r @"sfml\sfmlnet-audio-2.dll"
#r @"sfml\sfmlnet-graphics-2.dll"
open System
open System.IO

Lite.Program.Main()
Environment.CurrentDirectory <- Path.Combine(Environment.CurrentDirectory, @"bin\Debug\netcoreapp2.0\")
Environment.CurrentDirectory