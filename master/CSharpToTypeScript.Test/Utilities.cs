using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CSharpToTypeScript.Test
{
    internal class Utilities
    {
        public static string GetSourceFilePathName([CallerFilePath] string? callerFilePath = null) => callerFilePath ?? "";

        public static string GetProjectFolder() => Path.GetDirectoryName(GetSourceFilePathName())!;

        public static string GetTestDataFileContents(string testDataFileName) => File.ReadAllText(Path.Combine(GetProjectFolder(), @"TestData\src", testDataFileName + ".ts"));
    }
}
