﻿#nullable enable
#pragma warning disable VSTHRD010 // Invoke single-threaded types on Main thread
using System.Collections.Generic;
using System.Threading.Tasks;
using Community.VisualStudio.Toolkit;
using EnvDTE;
using Microsoft.VisualStudio.Shell;

namespace CSharpToTypeScript.Commands
{
    [Command(PackageGuids.GenerateReactFormResolverCommandString, PackageIds.GenerateReactFormResolverCommandID)]
    internal sealed class GenerateReactFormResolverCommand : BaseCommand<GenerateReactFormResolverCommand>
    {
        static readonly IReadOnlyList<vsCMElement> _ElementTypes = new vsCMElement[] 
        {
            vsCMElement.vsCMElementClass,
            vsCMElement.vsCMElementStruct
        };

        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            await CSharpToTypeScriptPackage.ExecuteCommandAsync(CSharpToTypeScriptPackage.ResolverGenerator, ".ts", _ElementTypes);
        }
    }
}
