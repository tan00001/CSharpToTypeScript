#nullable enable
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using Microsoft.Build.Evaluation;

namespace CSharpToTypeScript
{
    internal class ProjectSettings : IDisposable
    {
        private bool disposedValue;

        readonly Project Project;

        readonly string ProjectPath;

        public ProjectSettings(string projectFilePath)
        {
            Project = ProjectCollection.GlobalProjectCollection.LoadedProjects.FirstOrDefault(p => string.Compare(p.FullPath, projectFilePath, true) == 0)
                ?? new Project(projectFilePath);

            ProjectPath = Path.GetDirectoryName(projectFilePath);
        }

        public string GetScriptOutputFolder()
        {
            var scriptOutputFolder = Project.GetPropertyValue("CSharpToTypeScriptOutputFolder");
            if (string.IsNullOrEmpty(scriptOutputFolder))
            {
                scriptOutputFolder = ProjectPath;
            }
            else
            {
                return Path.Combine(ProjectPath, scriptOutputFolder);
            }

            return scriptOutputFolder;
        }

        public bool GetEnableNamespaceSetting()
        {
            var enableNamespace = Project.GetPropertyValue("CSharpToTypeScriptEnableNamespace");
            return string.Compare(enableNamespace, "true", true) == 0;
        }

        public void SetScriptOutputFolder(string scriptOutputFolder)
        {
            Project.SetProperty("CSharpToTypeScriptOutputFolder", GetRelativePath(ProjectPath, scriptOutputFolder));
            Project.Save();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Project? project;
                    while ((project = ProjectCollection.GlobalProjectCollection.LoadedProjects.FirstOrDefault(p => string.Compare(p.FullPath, Project.FullPath, true) == 0)) != null)
                    {
                        ProjectCollection.GlobalProjectCollection.UnloadProject(project);
                    };
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        /// <summary>
        /// Creates a relative path from one file or folder to another.
        /// </summary>
        /// <param name="fromPath">Contains the directory that defines the start of the relative path.</param>
        /// <param name="toPath">Contains the path that defines the endpoint of the relative path.</param>
        /// <returns>The relative path from the start directory to the end path.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="fromPath"/> or <paramref name="toPath"/> is <c>null</c>.</exception>
        /// <exception cref="UriFormatException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        private static string GetRelativePath(string fromPath, string toPath)
        {
            Uri fromUri = new (AppendDirectorySeparatorChar(fromPath));
            Uri toUri = new (AppendDirectorySeparatorChar(toPath));

            if (fromUri.Scheme != toUri.Scheme)
            {
                return toPath;
            }

            Uri relativeUri = fromUri.MakeRelativeUri(toUri);
            string relativePath = Uri.UnescapeDataString(relativeUri.ToString());

            if (string.Equals(toUri.Scheme, Uri.UriSchemeFile, StringComparison.OrdinalIgnoreCase))
            {
                relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            }

            return relativePath;
        }

        private static string AppendDirectorySeparatorChar(string path)
        {
            // Append a slash only if the path is a directory and does not have a slash.
            if (!Path.HasExtension(path) &&
                !path.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                return path + Path.DirectorySeparatorChar;
            }

            return path;
        }

        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
