using System;

namespace CSharpToTypeScript
{
    [Flags]
    public enum TsGeneratorOutput
    {
        Properties = 1,
        Enums = 2,
        Fields = 4,
        Constants = 8,
    }
}
