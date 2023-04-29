#nullable enable
using System;

namespace CSharpToTypeScript
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Enum, Inherited = false, AllowMultiple = false)]
    public sealed class TsNamespaceAttribute : Attribute
    {
        public string? Name { get; set; }
    }
}
