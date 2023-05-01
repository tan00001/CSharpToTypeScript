using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CSharpToTypeScript.Test
{
    internal static class CSharpToTypeScriptProgram
    {
        static readonly MethodInfo MainMethod = typeof(TypeScript).Assembly.GetType("Program")!.GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
            .First(m => m.Name == "<Main>$");

        public static void Run(string className, string outputPath, string generatorName)
        {
            var classAssemblyPath = Assembly.GetExecutingAssembly().Location;
            Run(classAssemblyPath, className, outputPath, generatorName);
        }

        public static void Run(string classAssemblyPath, string className, string outputPath, string generatorName)
        {
            if (!string.IsNullOrEmpty(generatorName))
            {
                var parameters = new string[] { classAssemblyPath, className, outputPath, generatorName };
                MainMethod.Invoke(null, new object?[] { parameters });
            }
            else
            {
                var parameters = new string[] { classAssemblyPath, className, outputPath };
                MainMethod.Invoke(null, new object?[] { parameters });
            }
        }
    }
}
