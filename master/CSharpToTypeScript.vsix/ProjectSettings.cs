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

        public ProjectSettings(string projectFilePath)
        {
            var project = ProjectCollection.GlobalProjectCollection.LoadedProjects.FirstOrDefault(p => string.Compare(p.FullPath, projectFilePath, true) == 0);

            if (project == null)
            {
                project = new Project(projectFilePath);
            }

            Project = project;
        }

        public string GetScriptOutputFolder()
        {
            var scriptOutputFolder = Project.GetPropertyValue("CSharpToTypeScriptOutputFolder");
            if (string.IsNullOrEmpty(scriptOutputFolder))
            {
                scriptOutputFolder = Path.GetDirectoryName(Project.FullPath);
                SetScriptOutputFolder(scriptOutputFolder);
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
            Project.SetProperty("CSharpToTypeScriptOutputFolder", scriptOutputFolder);
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

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~ProjectSettings()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
