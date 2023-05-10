// See https://aka.ms/new-console-template for more information
using System.Reflection;
using System.Text;
using CSharpToTypeScript;
using CSharpToTypeScript.AlternateGenerators;

if (args.Length < 3)
{
    Console.WriteLine("Usage: csharptotypescript <dll/exe file name> <class name> <output file name> <generator name> <enableNamespace=true/false> <launchDebugger>\n" +
        "\tFor example: csharptotypescript \"myassembly.dll\" \"MyNamespace.MyClass\" \"ClientApp\\Models\\MyClass.ts\" withForm(4) enableNamespace=false");
    return;
}

#if DEBUG
if (args.Length > 5 && args[5] == "LaunchDebugger")
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

bool enableNamespace = args.Length > 4 && StringComparer.OrdinalIgnoreCase.Equals(args[4], "enableNamespace=true");

TsGenerator tsGenerator = args switch
{
    _ when args.Length > 3 && StringComparer.OrdinalIgnoreCase.Equals(args[3], "withresolver") => new TsGeneratorWithResolver(enableNamespace),
    _ when args.Length > 3 && args[3].StartsWith("withform(", StringComparison.OrdinalIgnoreCase) => new TsGeneratorWithForm(GetColCount(args[3]), enableNamespace),
    _ => new TsGenerator(enableNamespace)
};

var ts = TypeScript.Definitions(tsGenerator)
    .For(typeToExport);

var scriptsByNamespaces = ts.Generate();

if (scriptsByNamespaces.Count == 1)
{
    File.WriteAllText(args[2], scriptsByNamespaces.Values.First().Script);
    return;
}

var directoryName = Path.GetDirectoryName(args[2]) ?? string.Empty;
foreach (var script in scriptsByNamespaces)
{
    if (ts.Member.NamespaceName == script.Key)
    {
        File.WriteAllText(args[2], script.Value.Script);
    }
    else
    {
        var filePath = Path.Combine(directoryName, script.Key + script.Value.FileType);
        if (!File.Exists(filePath) || !FileContainsScript(script.Value.Script, filePath))
        {
            File.WriteAllText(filePath, script.Value.Script);
        }
    }
}

static int GetColCount(string formLayout)
{
    const string errorMessage = "Invalid form layout. Expected a string with the format \"withform(<column count>).";

    if (string.IsNullOrWhiteSpace(formLayout))
    {
        throw new ArgumentException("Form layout cannot be blank.", nameof(formLayout));
    }

    if (!formLayout.StartsWith("withform(", StringComparison.OrdinalIgnoreCase))
    {
        throw new ArgumentException(errorMessage, nameof(formLayout));
    }

    var gridSize = formLayout.Substring("withform(".Length).TrimEnd(')');
    if (!Int32.TryParse(gridSize, out int colCount)
        || colCount <= 0
        || colCount > TsGeneratorWithForm.MaxColCount)
    {
        throw new ArgumentException(errorMessage, nameof(formLayout));
    }

    return colCount;
}

static bool FileContainsScript(string script, string filePath)
{
    var existingFileContent = File.ReadAllText(filePath);

    var scriptBodyBuilder = new StringBuilder();

    var scriptLines = script.Split("\r\n");

    foreach (var line in scriptLines)
    {
        if (line.StartsWith("import "))
        {
            if (!existingFileContent.Contains(line))
            {
                return false;
            }
            continue;
        }

        scriptBodyBuilder.AppendLine(line);
    }

    return existingFileContent.Contains(scriptBodyBuilder.ToString().Trim());
}