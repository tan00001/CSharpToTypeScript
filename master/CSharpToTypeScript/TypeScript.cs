namespace CSharpToTypeScript
{
    public static class TypeScript
    {
        public static TypeScriptFluent Definitions() => new ();

        public static TypeScriptFluent Definitions(TsGenerator scriptGenerator) => new (scriptGenerator);
    }
}
