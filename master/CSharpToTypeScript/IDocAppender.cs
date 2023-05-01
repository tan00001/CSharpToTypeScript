#nullable enable
using CSharpToTypeScript.Models;

namespace CSharpToTypeScript
{
    public interface IDocAppender
    {
        void AppendClassDoc(ScriptBuilder sb, TsClass classModel, string? className);

        void AppendInterfaceDoc(ScriptBuilder sb, TsInterface interfaceModel, string? className);

        void AppendPropertyDoc(
          ScriptBuilder sb,
          TsProperty property,
          string propertyName,
          string propertyType);

        void AppendConstantModuleDoc(
          ScriptBuilder sb,
          TsProperty property,
          string propertyName,
          string propertyType);

        void AppendEnumDoc(ScriptBuilder sb, TsEnum enumModel, string? enumName);

        void AppendEnumValueDoc(ScriptBuilder sb, TsEnumValue value);
    }
}
