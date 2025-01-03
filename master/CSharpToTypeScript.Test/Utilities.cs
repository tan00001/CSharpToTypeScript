using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using CSharpToTypeScript.AlternateGenerators;

namespace CSharpToTypeScript.Test
{
    internal class Utilities
    {
        public static string GetSourceFilePathName([CallerFilePath] string? callerFilePath = null) => callerFilePath ?? "";

        public static string GetProjectFolder() => Path.GetDirectoryName(GetSourceFilePathName())!;

        public static string GetTestDataFileContents(string testDataFileName) => File.ReadAllText(Path.Combine(GetProjectFolder(), @"TestData\src", testDataFileName + ".ts"));

        public static string GetTestResolverFileContents(string testDataFileName) => File.ReadAllText(Path.Combine(GetProjectFolder(), @"TestData\src", testDataFileName + "WithResolver.ts"));

        public static string GetTestFormFileContents(string testDataFileName) => File.ReadAllText(Path.Combine(GetProjectFolder(),
            @"TestData\src", testDataFileName + (testDataFileName == TsGeneratorWithForm.BootstrapUtilsNamespace ? ".tsx" : "Form.tsx")));

        public static string GetTestDataFileContents(string subFolfer, string testDataFileName) => File.ReadAllText(Path.Combine(GetProjectFolder(), @"TestData\src", subFolfer, testDataFileName + ".ts"));

        public static string GetTestFormFileContents(string subFolfer, string testDataFileName) => File.ReadAllText(Path.Combine(GetProjectFolder(),
            @"TestData\src", subFolfer, testDataFileName + (testDataFileName == TsGeneratorWithForm.BootstrapUtilsNamespace ? ".tsx" : "Form.tsx")));

        public static string GetTestVuelidateRuleContents(string subFolfer, string testDataFileName) => File.ReadAllText(Path.Combine(GetProjectFolder(),
            @"TestData\src", subFolfer, testDataFileName + "Rules.ts"));
    }
}
