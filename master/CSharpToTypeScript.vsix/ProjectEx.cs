#nullable enable
#pragma warning disable VSTHRD010 // Invoke single-threaded types on Main thread
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnvDTE;

namespace CSharpToTypeScript
{
    internal static class ProjectEx
    {
        public static string GetOutputAssmblyPathForActiveConfiguration(this Project project)
        {
            // Get the Configuration object
            try
            {
                Configuration config = project.ConfigurationManager.ActiveConfiguration;

                // Get the output path and assembly name from the project properties
                string projectDirectory = Path.GetDirectoryName(project.FullName);
                string outputFolderPath = config.Properties.Item("OutputPath").Value.ToString();
                string outputFileName = project.Properties.Item("OutputFileName").Value.ToString();

                if (string.IsNullOrEmpty(outputFileName))
                {
                    return string.Empty;
                }

                if (outputFileName.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                {
                    return Path.Combine(projectDirectory, outputFolderPath, Path.GetFileNameWithoutExtension(outputFileName) + ".dll");
                }

                return Path.Combine(projectDirectory, outputFolderPath, outputFileName);
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
