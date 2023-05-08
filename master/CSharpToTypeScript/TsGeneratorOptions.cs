namespace CSharpToTypeScript
{
    [Flags]
    public enum TsGeneratorOptions
    {
        Properties = 1,
        Enums = 2,
        Fields = 4,
        Constants = 8,
    }
}
