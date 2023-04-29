// See https://aka.ms/new-console-template for more information
using System.Reflection;
using CSharpToTypeScript;

if (args.Length < 3)
{
    Console.WriteLine("Usage: csharptotypescript <dll/exe file name> <class name> <output file name>\n" +
        "\tFor example: csharptotypescript \"myassembly.dll\" \"MyNamespace.MyClass\" \"ClientApp\\Models\\MyClass.ts\"");
    return;
}

var assembly = Assembly.LoadFrom(args[0]);
if (assembly == null)
{
    Console.WriteLine("Unable to load assembly \"" + args[0] + "\".");
    return;
}

var typeToExport = assembly.GetType(args[1]);
if (typeToExport == null)
{
    Console.WriteLine("The type \"" + args[1] + "\" is not found in assembly \"" + args[0] + "\".");
    return;
}

var ts = TypeScript.Definitions()
    .WithReference(args[2])
    .For(typeToExport);

string script = ts.Generate();

File.WriteAllText(args[2], script);