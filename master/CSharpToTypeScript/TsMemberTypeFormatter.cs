#nullable enable
using CSharpToTypeScript.Models;

namespace CSharpToTypeScript
{
    public delegate string TsMemberTypeFormatter(TsProperty tsProperty, string? memberTypeName, string currentNamespaceName,
            IReadOnlyDictionary<string, IReadOnlyDictionary<string, Int32>> importNames);
}
