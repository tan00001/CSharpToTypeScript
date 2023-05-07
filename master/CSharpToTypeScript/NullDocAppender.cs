using CSharpToTypeScript.Models;

namespace CSharpToTypeScript
{
    public class NullDocAppender : IDocAppender
    {
        public void AppendClassDoc(ScriptBuilder sb, TsClass classModel, string? className)
        {
        }

        public void AppendInterfaceDoc(ScriptBuilder sb, TsInterface interfaceModel, string? className)
        {
        }

        public void AppendPropertyDoc(
          ScriptBuilder sb,
          TsProperty property,
          string propertyName,
          string propertyType)
        {
        }

        public void AppendConstantModuleDoc(
          ScriptBuilder sb,
          TsProperty property,
          string propertyName,
          string propertyType)
        {
        }

        public void AppendEnumDoc(ScriptBuilder sb, TsEnum enumModel, string? enumName)
        {
        }

        public void AppendEnumValueDoc(ScriptBuilder sb, TsEnumValue value)
        {
        }
    }
}
