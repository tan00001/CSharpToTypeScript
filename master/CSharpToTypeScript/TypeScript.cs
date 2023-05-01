namespace CSharpToTypeScript
{
    public static class TypeScript
    {
        public static TypeScriptFluent Definitions(bool enableNamespace = false) => new (enableNamespace);

        public static TypeScriptFluent Definitions(TsGenerator scriptGenerator) => new (scriptGenerator);
    }
}
