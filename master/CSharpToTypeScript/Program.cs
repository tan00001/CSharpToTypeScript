// See https://aka.ms/new-console-template for more information
using System.Reflection;
using System.Text;
using CSharpToTypeScript;
using CSharpToTypeScript.AlternateGenerators;
using CSharpToTypeScript.Properties;

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

Assembly assembly;

try
{
    assembly = Assembly.LoadFrom(args[0]);
}
catch (Exception ex)
{
    Console.WriteLine("Unable to load assembly \"" + args[0] + "\". Error: \"" + ex.Message + "\".");
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
    _ when args.Length > 3 && StringComparer.OrdinalIgnoreCase.Equals(args[3], "withvuelidate") => new TsGeneratorWithVuelidate(enableNamespace),
    _ when args.Length > 3 && args[3].StartsWith("withform(", StringComparison.OrdinalIgnoreCase) => new TsGeneratorWithForm(GetColCount("withform(", args[3]), GetReactstrapModalTitle("withform(", args[3]), enableNamespace),
    _ when args.Length > 3 && args[3].StartsWith("withvue(", StringComparison.OrdinalIgnoreCase) => new TsGeneratorWithVue(GetColCount("withvue(", args[3]), GetReactstrapModalTitle("withvue(", args[3]), enableNamespace),
    _ => new TsGenerator(enableNamespace)
};

var ts = TypeScript.Definitions(tsGenerator)
    .For(typeToExport, true);

var scriptsByNamespaces = ts.Generate();
var directoryName = Path.GetDirectoryName(args[2]) ?? string.Empty;

if (scriptsByNamespaces.Count == 1)
{
    var output = scriptsByNamespaces.Values.First();
    File.WriteAllText(args[2], output.Script);
    foreach (var otherFileType in output.OtherFileTypes)
    {
        var filePath = Path.Combine(directoryName, otherFileType.Key);
        File.WriteAllText(filePath, otherFileType.Value);
    }
    return;
}

foreach (var script in scriptsByNamespaces)
{
    if (ts.Member.NamespaceName == script.Key)
    {
        File.WriteAllText(args[2], script.Value.Script);
        foreach (var otherFileType in script.Value.OtherFileTypes)
        {
            var filePath = Path.Combine(directoryName, otherFileType.Key);
            File.WriteAllText(filePath, otherFileType.Value);
        }
    }
    else
    {
        var filePath = Path.Combine(directoryName, script.Key + script.Value.FileType);
        if (!File.Exists(filePath) || !FileContainsScript(script.Value.Script, filePath))
        {
            File.WriteAllText(filePath, script.Value.Script);
            foreach (var otherFileType in script.Value.OtherFileTypes)
            {
                filePath = Path.Combine(directoryName, otherFileType.Key);
                if (!File.Exists(filePath))
                {
                    File.WriteAllText(filePath, otherFileType.Value);
                }
            }
        }
    }
}

static int GetColCount(string typePrefix, string formLayout)
{
    string errorMessage = string.Format(Resources.GeneratorTypeSpecError, typePrefix);

    var gridSize = formLayout.Substring(typePrefix.Length).TrimEnd(')');

    int separatorIndex = gridSize.IndexOf(',');
    if (separatorIndex > 0)
    {
        gridSize = gridSize.Substring(0, separatorIndex);
    }

    gridSize = gridSize.Trim();

    if (!Int32.TryParse(gridSize, out int colCount)
        || colCount <= 0
        || colCount > TsGeneratorWithForm.MaxColCount)
    {
        throw new ArgumentException(errorMessage, nameof(formLayout));
    }

    return colCount;
}

static string? GetReactstrapModalTitle(string typePrefix, string formLayout)
{
    string errorMessage = string.Format(Resources.GeneratorTypeSpecError, typePrefix);

    var reactstrapModalTitle = formLayout.Substring(typePrefix.Length).TrimEnd(')');

    int separatorIndex = reactstrapModalTitle.IndexOf(',');
    if (separatorIndex < 0)
    {
        return null;
    }

    reactstrapModalTitle = reactstrapModalTitle.Substring(++separatorIndex).Trim();

    if (string.IsNullOrEmpty(reactstrapModalTitle))
    {
        return reactstrapModalTitle;
    }

    if (reactstrapModalTitle.Length > 256)
    {
        throw new ArgumentException(errorMessage, nameof(formLayout));
    }

    return reactstrapModalTitle;
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