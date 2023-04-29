#nullable enable
using CSharpToTypeScript.Models;

namespace CSharpToTypeScript
{
    public delegate string TsMemberTypeFormatter(TsProperty tsProperty, string? memberTypeName);
}
