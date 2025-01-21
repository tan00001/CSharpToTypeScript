#nullable enable

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Community.VisualStudio.Toolkit;
using CSharpToTypeScript.Commands;
using CSharpToTypeScript.Options;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.Win32;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Task = System.Threading.Tasks.Task;
using Process = System.Diagnostics.Process;
using MessageBox = Community.VisualStudio.Toolkit.MessageBox;
using Application = System.Windows.Application;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Diagnostics;

namespace CSharpToTypeScript
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [InstalledProductRegistration(Vsix.Name, Vsix.Description, Vsix.Version)]
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(PackageGuids.CSharpToTypeScriptPackageString)]
    [ProvideUIContextRule(PackageGuids.CSharpEditorContextString,
        name: "Supported Files",
        expression: "CSharp",
        termNames: new[] { "CSharp" },
        termValues: new[] { "HierSingleSelectionName:.cs$" })]
    public sealed class CSharpToTypeScriptPackage : AsyncPackage
    {
        public const string DefaultGenerator = "default";
        public const string ResolverGenerator = "withresolver";
        public const string FormGenerator = "withform";
        public const string VuelidateRulesGenerator = "withvuelidate";
        public const string VueGenerator = "withvue";

        static readonly IReadOnlyDictionary<vsCMElement, string> _ElementNames = new Dictionary<vsCMElement, string>()
        {
            {  vsCMElement.vsCMElementEnum, "enum" },
            {  vsCMElement.vsCMElementClass, "class" },
            {  vsCMElement.vsCMElementInterface, "interface" },
            {  vsCMElement.vsCMElementStruct, "struct" },
        };

        const Int32 ProcessingWaitTime = 60 * 1000;
        static readonly string ExeFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"net9\CSharpToTypeScript.exe");
        static readonly string Net8ExeFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"net8\CSharpToTypeScript.exe");
        static readonly string Net7ExeFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"net7\CSharpToTypeScript.exe");

        public static RatingPrompt RatingPrompt { get; private set; } = null!;

        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to monitor for initialization cancellation, which can occur when VS is shutting down.</param>
        /// <param name="progress">A provider for progress updates.</param>
        /// <returns>A task representing the async work of package initialization, or an already completed task if there is none. Do not return null from this method.</returns>
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            // When initialized asynchronously, the current thread may be a background thread at this point.
            // Do any initialization that requires the UI thread after switching to the UI thread.
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            RatingPrompt = new("Programount.CSharpToTypeScript", Vsix.Name, await General.GetLiveInstanceAsync(), 2);

            await this.RegisterCommandsAsync();
        }

        #endregion

        #pragma warning disable VSTHRD010 // Invoke single-threaded types on Main thread
        public static async Task ExecuteCommandAsync(string generatorName, string fileExtension, IReadOnlyList<vsCMElement> elementTypes)
        {
            // Get the DTE object
            if (GetGlobalService(typeof(DTE)) is not DTE dte)
            {
                return;
            }

            var document = dte.ActiveDocument;
            if (document == null)
            {
                var messageBox = new MessageBox();
                await messageBox.ShowAsync("Please open the document and place the cursor in a C# class definition first.", "",
                    OLEMSGICON.OLEMSGICON_INFO, OLEMSGBUTTON.OLEMSGBUTTON_OK);
                return;
            }

            if (document.Language != "CSharp")
            {
                dte.StatusBar.Text = "Only C# source is supported.";
                return;
            }

            // Get the project containing the selected item
            var project = document.ProjectItem.ContainingProject;
            if (project == null)
            {
                dte.StatusBar.Text = "No project was found.";
                return;
            }

            string documentPath = document.FullName;

            // Get the output assembly path
            string outputAssmblyPath = project.GetOutputAssmblyPathForActiveConfiguration();
            if (!await ValidateOutputAssemblyPathAsync(outputAssmblyPath, project.Name, documentPath))
            {
                return;
            }

            // Get the full name of the class/interface/enum/struct
            string typeName = await GetTypeNameAsync(dte, document, elementTypes);
            if (string.IsNullOrEmpty(typeName))
            {
                return;
            }

            var typeParamNames = GetTypeParamNames(typeName);
            if ((typeParamNames.Count > 0 
                    && (generatorName == ResolverGenerator 
                        || generatorName == VuelidateRulesGenerator)) // Type selection when not using dialog box
                || generatorName == FormGenerator
                || generatorName == VueGenerator)
            {
                ExportOptionsDlg formLayoutDlg = new(generatorName == FormGenerator | generatorName == VueGenerator,
                    generatorName == VueGenerator, typeParamNames);
                if (await formLayoutDlg.ShowDialogAsync(WindowStartupLocation.CenterOwner) != true)
                {
                    return;
                }
                typeName = ReplaceTypeParams(typeName, formLayoutDlg.TypeParams);
                if (generatorName == FormGenerator
                    || generatorName == VueGenerator)
                {
                    generatorName += '(' + formLayoutDlg.ColCount.ToString();
                    if (formLayoutDlg.UseReactstrapModal)
                    {
                        generatorName = '"' + generatorName + ',' + formLayoutDlg.ReactstrapModalTitle + ")\"";
                    }
                    else
                    {
                        generatorName += ')';
                    }
                }
            }
            else
            {
                typeName = RemoveTypeParams(typeName);
            }

            string scriptOutputFolder;
            bool enableNamespace;
            using (var projectSettings = new ProjectSettings(project.FullName))
            {
                scriptOutputFolder = projectSettings.GetScriptOutputFolder();
                enableNamespace = projectSettings.GetEnableNamespaceSetting();
            }

            SaveFileDialog dialog = new()
            {
                DefaultExt = fileExtension,
                FileName = Path.GetFileNameWithoutExtension(documentPath) + fileExtension,
                Filter = fileExtension == ".tsx" ? "TypeScript XML Files | *.tsx" :
                    (fileExtension == ".vue" ? "Vue Files | *.vue" : "TypeScript Files | *.ts"),
                InitialDirectory = scriptOutputFolder
            };

            if (dialog.ShowDialog(Application.Current.MainWindow) != true)
            {
                return;
            }

            var directoryName = Path.GetDirectoryName(dialog.FileName);
            if (string.Compare(directoryName, scriptOutputFolder.TrimEnd(Path.DirectorySeparatorChar,
                    Path.AltDirectorySeparatorChar), true) != 0)
            {
                using var projectSettings = new ProjectSettings(project.FullName);
                projectSettings.SetScriptOutputFolder(directoryName);
            }

            string scriptFilePath = Path.Combine(directoryName, Path.GetFileNameWithoutExtension(dialog.FileName) + ".ts");
            if (File.Exists(scriptFilePath))
            {
                var messageBox = new MessageBox();
                if (await messageBox.ShowAsync($"The file \"{scriptFilePath}\" already exists. Overwrite?", "",
                    OLEMSGICON.OLEMSGICON_WARNING, OLEMSGBUTTON.OLEMSGBUTTON_YESNO) == VSConstants.MessageBoxResult.IDNO)
                {
                    return;
                }
            }

            try
            {
                // Start the new process
                using Process process = new()
                {
#if __DEBUG__
                    StartInfo = new()
                    {
                        FileName = ExeFilePath,
                        Arguments = '"' + outputAssmblyPath + "\" "
                            + typeName + " \""
                            + dialog.FileName + "\" "
                            + generatorName + " "
                            + "enableNamespace=" + enableNamespace
                            + " LaunchDebugger",
                        RedirectStandardOutput = false,
                        RedirectStandardError = false,
                        UseShellExecute = true,
                        CreateNoWindow = false
                    }
#else
                    StartInfo = new()
                    {
                        FileName = ExeFilePath,
                        Arguments = '"' + outputAssmblyPath + "\" "
                            + typeName + " \""
                            + dialog.FileName + "\" "
                            + generatorName + " "
                            + "enableNamespace=" + enableNamespace,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
#endif
                };
                try
                {
                    process.Start();
#if __DEBUG__
                    System.Threading.Thread.Sleep(360000);
#endif

                    // Optionally, read the standard output and standard error streams if needed
                    var readStdoutTask = process.StandardOutput.ReadToEndAsync();
                    var readStdErrTask = process.StandardError.ReadToEndAsync();

                    // Wait for the process to exit
                    if (!process.WaitForExit(ProcessingWaitTime))
                    {
                        var messageBox = new MessageBox();
                        if (await messageBox.ShowAsync("The background process for generating TypeScript file is taking longer than expected. Continue waiting?", "",
                            OLEMSGICON.OLEMSGICON_INFO, OLEMSGBUTTON.OLEMSGBUTTON_YESNO) == VSConstants.MessageBoxResult.IDNO)
                        {
                            process.Kill();
                            return;
                        }
                        process.WaitForExit();
                    }

                    string output = await readStdoutTask;
                    string error = await readStdErrTask;

                    if (!string.IsNullOrEmpty(output) || !string.IsNullOrEmpty(error))
                    {
                        if (output.Contains("You must install or update .NET to run this application."))
                        {
                            (output, error) = await RunNet8ProcessAsync(outputAssmblyPath, typeName, dialog.FileName,
                                generatorName, enableNamespace);

                            if (!string.IsNullOrEmpty(output) || !string.IsNullOrEmpty(error))
                            {
                                if (output.Contains("You must install or update .NET to run this application."))
                                {
                                    (output, error) = await RunNet7ProcessAsync(outputAssmblyPath, typeName, dialog.FileName,
                                        generatorName, enableNamespace);
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(output) || !string.IsNullOrEmpty(error))
                        {
                            var messageBox = new MessageBox();
                            await messageBox.ShowErrorAsync(output, error);
                            return;
                        }
                    }

                    await VS.Documents.OpenAsync(dialog.FileName);

                    CSharpToTypeScriptPackage.RatingPrompt.RegisterSuccessfulUsage();
                }
                catch
                {
                    process.Kill();
                    throw;
                }
            }
            catch (Exception ex)
            {
                var messageBox = new MessageBox();
                await messageBox.ShowErrorAsync(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "");
            }
        }

        private static async Task<(string output, string error)> RunNet8ProcessAsync(string outputAssmblyPath, string typeName, string fileName,
            string generatorName, bool enableNamespace)
        {
            using Process process8 = new()
            {
                StartInfo = new()
                {
                    FileName = Net8ExeFilePath,
                    Arguments = '"' + outputAssmblyPath + "\" "
                        + typeName + " \""
                        + fileName + "\" "
                        + generatorName + " "
                        + "enableNamespace=" + enableNamespace,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process8.Start();

            // Optionally, read the standard output and standard error streams if needed
            var readStdoutTask = process8.StandardOutput.ReadToEndAsync();
            var readStdErrTask = process8.StandardError.ReadToEndAsync();

            // Wait for the process to exit
            if (!process8.WaitForExit(ProcessingWaitTime))
            {
                var messageBox = new MessageBox();
                if (await messageBox.ShowAsync("The background process for generating TypeScript file is taking longer than expected. Continue waiting?", "",
                    OLEMSGICON.OLEMSGICON_INFO, OLEMSGBUTTON.OLEMSGBUTTON_YESNO) == VSConstants.MessageBoxResult.IDNO)
                {
                    process8.Kill();
                    return (string.Empty, "The background process for generating TypeScript file is cancelled.");
                }
                process8.WaitForExit();
            }

            var output = await readStdoutTask;
            var error = await readStdErrTask;

            return (output, error);
        }

        private static async Task<(string output, string error)> RunNet7ProcessAsync(string outputAssmblyPath, string typeName, string fileName,
            string generatorName, bool enableNamespace)
        {
            using Process process7 = new()
            {
                StartInfo = new()
                {
                    FileName = Net7ExeFilePath,
                    Arguments = '"' + outputAssmblyPath + "\" "
                        + typeName + " \""
                        + fileName + "\" "
                        + generatorName + " "
                        + "enableNamespace=" + enableNamespace,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process7.Start();

            // Optionally, read the standard output and standard error streams if needed
            var readStdoutTask = process7.StandardOutput.ReadToEndAsync();
            var readStdErrTask = process7.StandardError.ReadToEndAsync();

            // Wait for the process to exit
            if (!process7.WaitForExit(ProcessingWaitTime))
            {
                var messageBox = new MessageBox();
                if (await messageBox.ShowAsync("The background process for generating TypeScript file is taking longer than expected. Continue waiting?", "",
                    OLEMSGICON.OLEMSGICON_INFO, OLEMSGBUTTON.OLEMSGBUTTON_YESNO) == VSConstants.MessageBoxResult.IDNO)
                {
                    process7.Kill();
                    return (string.Empty, "The background process for generating TypeScript file is cancelled.");
                }
                process7.WaitForExit();
            }

            var output = await readStdoutTask;
            var error = await readStdErrTask;

            return (output, error);
        }

        private static async Task<string> GetTypeNameAsync(DTE dte, Document document, IReadOnlyList<vsCMElement> elementTypes)
        {
            // Get the TextSelection object
            if (document.Selection is not TextSelection textSelection)
            {
                dte.StatusBar.Text = "No selection was found.";
                return string.Empty;
            }

            try
            {
                // Get the current cursor position
                var cursorPosition = textSelection.ActivePoint;

                List<Exception> exceptions = new ();
                // Get the CodeElement at the cursor position
                foreach (var elementType in elementTypes)
                {
                    try
                    {
                        CodeElement? codeElement = document.ProjectItem.FileCodeModel.CodeElementFromPoint(cursorPosition, elementType);
                        if (codeElement != null && !string.IsNullOrEmpty(codeElement.FullName))
                        {
                            return codeElement.FullName;
                        }
                    }
                    catch (Exception ex)
                    {
                        exceptions.Add(ex);
                    }
                }

                if (exceptions.Count > 0)
                {
                    dte.StatusBar.Text = "Error: " + string.Join(", ", exceptions.Select(e => e.Message)) + ".";
                }
                else
                {
                    dte.StatusBar.Text = "Please select one of the following: " + string.Join(", ", elementTypes.Select(t => _ElementNames[t])) + ".";
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                var messageBox = new MessageBox();
                await messageBox.ShowErrorAsync(ex.Message, ex.InnerException != null ? ex.InnerException.Message : "");
                return string.Empty;
            }
        }

        private static IReadOnlyList<string> GetTypeParamNames(string typeName)
        {
            var typeParamIndex = typeName.IndexOf('<');
            if (typeParamIndex < 0 || !typeName.EndsWith(">"))
            {
                return Array.Empty<string>();
            }

            return typeName.Substring(typeParamIndex + 1).TrimEnd('>').Split(',').Select(t => t.Trim()).ToList();
        }

        private static string RemoveTypeParams(string typeName)
        {
            var typeParamIndex = typeName.IndexOf('<');
            if (typeParamIndex < 0 || !typeName.EndsWith(">"))
            {
                return typeName;
            }

            return typeName.Substring(0, typeParamIndex) + '`' + (typeName.Substring(typeParamIndex).Count(c => c == ',') + 1);
        }

        private static string ReplaceTypeParams(string typeName, IReadOnlyList<string> typeParams)
        {
            var typeParamIndex = typeName.IndexOf('<');
            if (typeParamIndex < 0 || !typeName.EndsWith(">"))
            {
                return typeName;
            }

            return '"' + typeName.Substring(0, typeParamIndex) + '`' + typeParams.Count + '[' 
                + string.Join(",", typeParams.Select(p => FormatTypeParam(p))) 
                + "]\"";
        }

        private static string FormatTypeParam(string typeParam)
        {
            if (typeParam.StartsWith("System.") && !typeParam.Contains(','))
            {
                return '[' + Type.GetType(typeParam).AssemblyQualifiedName + ']';
            }

            return '[' + typeParam + ']';
        }

        private static async Task<bool> ValidateOutputAssemblyPathAsync(string outputAssmblyPath, string projectName, string documentPath)
        {
            if (string.IsNullOrEmpty(outputAssmblyPath))
            {
                var messageBox = new MessageBox();
                await messageBox.ShowAsync("The project \"" + projectName + "\" does not have an output assembly file.", "",
                    OLEMSGICON.OLEMSGICON_INFO, OLEMSGBUTTON.OLEMSGBUTTON_OK);
                return false;
            }

            if (!File.Exists(outputAssmblyPath))
            {
                var messageBox = new MessageBox();
                await messageBox.ShowAsync("File \"" + Path.GetFileName(outputAssmblyPath) + "\" does not exist.", "",
                    OLEMSGICON.OLEMSGICON_INFO, OLEMSGBUTTON.OLEMSGBUTTON_OK);
                return false;
            }

            if (File.GetLastWriteTime(outputAssmblyPath) < File.GetLastWriteTime(documentPath))
            {
                var messageBox = new MessageBox();
                await messageBox.ShowAsync("File \"" + Path.GetFileName(documentPath) + "\" was updated since the last build of \""
                    + Path.GetFileName(outputAssmblyPath) + "\". Please re-build first.", "",
                    OLEMSGICON.OLEMSGICON_INFO, OLEMSGBUTTON.OLEMSGBUTTON_OK);
                return false;
            }

            return true;
        }
    }
}
