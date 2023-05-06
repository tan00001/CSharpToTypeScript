#nullable enable
#pragma warning disable VSTHRD010 // Invoke single-threaded types on Main thread
using System.Threading.Tasks;
using System.Windows;
using Community.VisualStudio.Toolkit;
using EnvDTE;
using Microsoft.VisualStudio.Shell;

namespace CSharpToTypeScript.Commands
{
    [Command(PackageGuids.GenerateReactstrapFormCommandString, PackageIds.GenerateReactstrapFormCommandID)]
    internal sealed class GenerateReactstrapFormCommand : BaseCommand<GenerateReactstrapFormCommand>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            FormLayoutDlg formLayoutDlg = new();

            if (await formLayoutDlg.ShowDialogAsync(WindowStartupLocation.CenterOwner) != true)
            {
                return;
            }

            await CSharpToTypeScriptPackage.ExecuteCommandAsync("withform(" + formLayoutDlg.ColCount + ')', ".tsx");
        }
    }
}
