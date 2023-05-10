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
            if (property.PropertyType is TsSystemType tsSystemType)
            {
                switch (tsSystemType.Kind)
                {
                    case SystemTypeKind.Number:
                        sb.AppendLineIndented("if (!values." + propertyName + " && values." + propertyName + " !== 0) {");
                        break;

                    case SystemTypeKind.Date:
                        sb.AppendLineIndented("if (!values." + propertyName + " || (typeof values." + propertyName + " === 'string' && isNaN(Date.parse(values." + propertyName + ")))) {");
                        break;

                    case SystemTypeKind.Bool:
                        sb.AppendLineIndented("if (!values." + propertyName + " && values." + propertyName + " !== false) {");
                        break;

                    default:
                        sb.AppendLineIndented("if (!values." + propertyName + ") {");
                        break;
                }
            }
            else if (property.PropertyType is TsEnum)
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
