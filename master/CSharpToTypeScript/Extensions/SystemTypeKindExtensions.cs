using CSharpToTypeScript.Models;

namespace CSharpToTypeScript.Extensions
{
    public static class SystemTypeKindExtensions
    {
        public static string ToTypeScriptString(this SystemTypeKind type)
        {
            if (type == SystemTypeKind.Bool)
                return "boolean";
            return type == SystemTypeKind.Date ? "Date | string" : type.ToString().ToLower();
        }
    }
}
