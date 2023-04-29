#nullable enable
using System.Threading.Tasks;
using Community.VisualStudio.Toolkit;
using System.Reflection;
using System.IO;
using EnvDTE;
using Microsoft.VisualStudio.Threading;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;
using Process = System.Diagnostics.Process;
using MessageBox = Community.VisualStudio.Toolkit.MessageBox;
using Application = System.Windows.Application;
using Microsoft.VisualStudio;

namespace CSharpToTypeScript.Commands
{
    [Command(PackageGuids.CSharpToTypeScriptPackageString, PackageIds.GenerateTypeScriptCommandID)]
    internal sealed class GenerateTypeScriptCommand : BaseCommand<GenerateTypeScriptCommand>
    {
        static readonly string ExeFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "CSharpToTypeScript.exe");

        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            // Get the DTE object
            if (CSharpToTypeScriptPackage.GetGlobalService(typeof(DTE)) is not DTE dte)
            {
                return;
            }

            // Get the selected item
            SelectedItem? selectedItem = dte?.SelectedItems.Item(1);
            if (selectedItem == null)
            {
                dte.StatusBar.Text = "No selected item was found.";
                return;
            }

            var document = dte?.ActiveDocument;
            if (document == null || document.Language == "CSharp")
            {
                dte.StatusBar.Text = "No selected item was found.";
                return;
            }

            // Get the TextSelection object
            TextSelection? textSelection = document.Selection as TextSelection;
            if (textSelection == null)
            {
                dte.StatusBar.Text = "No selected item was found.";
                return;
            }

            // Get the current cursor position
            var cursorPosition = textSelection.ActivePoint;

            // Get the CodeElement at the cursor position
            CodeElement? codeElement = document.ProjectItem.FileCodeModel.CodeElementFromPoint(cursorPosition, vsCMElement.vsCMElementClass);

            if (codeElement == null)
            {
                dte.StatusBar.Text = "No selected item was found.";
                return;
            }

            // Get the full name of the class
            string className = codeElement.FullName;

            // Get the project containing the selected item
            var project = selectedItem.Project;
            if (project == null)
            {
                dte.StatusBar.Text = "No project was found.";
                return;
            }

            // Get the Configuration object
            Configuration config = project.ConfigurationManager.ActiveConfiguration;

            // Get the output path and assembly name from the project properties
            string outputPath = config.Properties.Item("OutputPath").Value.ToString();
            string assemblyName = project.Properties.Item("AssemblyName").Value.ToString();

            // Get the full output folder path by combining the project folder path and the output path
            string projectFolderPath = Path.GetDirectoryName(project.FullName);
            string outputFolderPath = Path.GetFullPath(Path.Combine(projectFolderPath, outputPath));

            // Combine the output folder path and the assembly name to get the output DLL path
            string outputDllPath = Path.Combine(outputFolderPath, $"{assemblyName}.dll");
            var documentPath = dte?.ActiveDocument.Path;

            if (File.GetLastWriteTime(outputDllPath) < File.GetLastWriteTime(documentPath))
            {
                var messageBox = new MessageBox();
                await messageBox.ShowAsync("File \"" + Path.GetFileName(documentPath) + "\" was updated since the last build of \""
                    + Path.GetFileName(outputDllPath) + "\"", "",
                    OLEMSGICON.OLEMSGICON_INFO, OLEMSGBUTTON.OLEMSGBUTTON_OK);
                return;
            }

            var currentProject = dte.ActiveDocument.ProjectItem.ContainingProject;
            string projectFilePath = currentProject.FullName;
            string userFilePath = Path.ChangeExtension(projectFilePath, ".user");
            var projectUserFile = new Microsoft.Build.Evaluation.Project(userFilePath);
            var csharpToTypeScriptOutputFolder = projectUserFile.GetPropertyValue("CSharpToTypeScriptOutputFolder");
            if (string.IsNullOrEmpty(csharpToTypeScriptOutputFolder))
            {
                csharpToTypeScriptOutputFolder = Path.GetDirectoryName(documentPath);
                projectUserFile.SetProperty("CSharpToTypeScriptOutputFolder", csharpToTypeScriptOutputFolder);
                project.Save();
            }

            SaveFileDialog dialog = new()
            {
                DefaultExt = ".ts",
                FileName = Path.GetFileNameWithoutExtension(documentPath) + ".cs",
                InitialDirectory = csharpToTypeScriptOutputFolder
            };

            if (dialog.ShowDialog(Application.Current.MainWindow) == true)
            {
                // Start the new process
                using Process process = new()
                {
                    StartInfo = new()
                    {
                        FileName = ExeFilePath,
                        Arguments = '"' + outputDllPath + "\" " + className + " \"" + dialog.FileName + '"', 
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };
                process.Start();

                // Optionally, read the standard output and standard error streams if needed
                string output = await process.StandardOutput.ReadToEndAsync();
                string error = await process.StandardError.ReadToEndAsync();

                // Wait for the process to exit
                process.WaitForExit();

                await VS.Documents.OpenAsync(dialog.FileName);

                CSharpToTypeScriptPackage.RatingPrompt.RegisterSuccessfulUsage();
            }

            await TaskScheduler.Default;
        }
    }
}
