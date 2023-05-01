// See https://aka.ms/new-console-template for more information
using System.Reflection;
using CSharpToTypeScript;
using CSharpToTypeScript.AlternateGenerators;

if (args.Length < 3)
{
    Console.WriteLine("Usage: csharptotypescript <dll/exe file name> <class name> <output file name>\n" +
        "\tFor example: csharptotypescript \"myassembly.dll\" \"MyNamespace.MyClass\" \"ClientApp\\Models\\MyClass.ts\"");
    return;
}

#if DEBUG
if (args.Length > 4 && args[4] == "LaunchDebugger")
{
    Console.ReadLine();
}
#endif

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

bool enableNamespace = args.Length > 4 && StringComparer.OrdinalIgnoreCase.Equals(args[5], "true");

TsGenerator tsGenerator = args switch
{
    _ when args.Length > 3 && StringComparer.OrdinalIgnoreCase.Equals(args[4], "withresolver") => new TsGeneratorWithResolver(enableNamespace),
    _ => new TsGenerator(enableNamespace)
};

var ts = TypeScript.Definitions(tsGenerator)
    .For(typeToExport);

string script = ts.Generate();

File.WriteAllText(args[2], script);