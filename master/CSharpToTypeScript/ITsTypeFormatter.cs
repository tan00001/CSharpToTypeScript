using CSharpToTypeScript.Models;

namespace CSharpToTypeScript
{
    public interface ITsTypeFormatter
    {
        string FormatType(TsType type);
    }
}
