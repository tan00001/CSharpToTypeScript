#nullable enable
#pragma warning disable VSTHRD010 // Invoke single-threaded types on Main thread
using System.Collections.Generic;
using System.Threading.Tasks;
using Community.VisualStudio.Toolkit;
using EnvDTE;
using Microsoft.VisualStudio.Shell;

namespace CSharpToTypeScript.Commands
{
    [Command(PackageGuids.GenerateTypeScriptCommandString, PackageIds.GenerateTypeScriptCommandID)]
    internal sealed class GenerateTypeScriptCommand : BaseCommand<GenerateTypeScriptCommand>
    {
        static readonly IReadOnlyList<vsCMElement> _ElementTypes = new vsCMElement[] 
        { 
            vsCMElement.vsCMElementClass,
            vsCMElement.vsCMElementStruct,
            vsCMElement.vsCMElementEnum,
            vsCMElement.vsCMElementInterface
        };

        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            await CSharpToTypeScriptPackage.ExecuteCommandAsync(CSharpToTypeScriptPackage.DefaultGenerator, ".ts", _ElementTypes);
        }
    }
}
