using System.ComponentModel.DataAnnotations;

namespace CSharpToTypeScript.Models
{
    public class RequiredRule : ITsValidationRule
    {
        readonly RequiredAttribute _Required;

        public RequiredRule(RequiredAttribute required) 
        {
            _Required = required;
        }

        public void BuildRule(ScriptBuilder sb, string propertyName, TsProperty property, IReadOnlyDictionary<string, TsProperty> allProperties)
        {
            if (property.PropertyType is TsSystemType tsSystemType && tsSystemType.Kind == SystemTypeKind.Number)
            {
                sb.AppendLineIndented("if (!values." + propertyName + " && values." + propertyName + " !== 0) {");
            }
            else
            {
                sb.AppendLineIndented("if (!values." + propertyName + ") {");
            }

            using (sb.IncreaseIndentation())
            {
                sb.AppendLineIndented("errors." + propertyName + " = {");
                using (sb.IncreaseIndentation())
                {
                    sb.AppendLineIndented("type: 'required',");
                    sb.AppendLineIndented("message: '" + (!string.IsNullOrEmpty(_Required.ErrorMessage) ? _Required.ErrorMessage 
                        : (property.GetDisplayName() + " is required.")) + "'");
                }
                sb.AppendLineIndented("};");
            }
            sb.AppendLineIndented("}");
        }
    }
}
