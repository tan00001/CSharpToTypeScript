using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace CSharpToTypeScript.Models
{
    public class RequiredRule : ITsValidationRule
    {
        readonly RequiredAttribute? _Required;

        readonly DataMemberAttribute? _DataMember;

        public RequiredRule(RequiredAttribute? required, DataMemberAttribute? dataMember) 
        {
            _Required = required;
            _DataMember = dataMember;
        }

        public void BuildRule(ScriptBuilder sb, string propertyName, TsProperty property, IReadOnlyDictionary<string, TsProperty> allProperties, ISet<string> constNamesInUse)
        {
            if (property.PropertyType is TsSystemType tsSystemType)
            {
                switch (tsSystemType.Kind)
                {
                    case SystemTypeKind.Number:
                        if (!DisallowAllDefaultValues())
                        {
                            sb.AppendLineIndented("if (!values." + propertyName + " && values." + propertyName + " !== 0) {");
                        }
                        else
                        {
                            sb.AppendLineIndented("if (!values." + propertyName + ") {");
                        }
                        break;

                    case SystemTypeKind.Date:
                        sb.AppendLineIndented("if (!values." + propertyName + " || isNaN(new Date(values." + propertyName + ").getTime())) {");
                        break;

                    case SystemTypeKind.Bool:
                        if (!DisallowAllDefaultValues())
                        {
                            sb.AppendLineIndented("if (!values." + propertyName + " && values." + propertyName + " !== false) {");
                        }
                        else
                        {
                            sb.AppendLineIndented("if (!values." + propertyName + ") {");
                        }
                        break;

                    default:
                        sb.AppendLineIndented("if (!values." + propertyName + ") {");
                        break;
                }
            }
            else if (property.PropertyType is TsEnum)
            {
                if (!DisallowAllDefaultValues())
                {
                    sb.AppendLineIndented("if (!values." + propertyName + " && values." + propertyName + " !== 0) {");
                }
                else
                {
                    sb.AppendLineIndented("if (!values." + propertyName + ") {");
                }
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
                    sb.AppendLineIndented("message: '" + (!string.IsNullOrEmpty(_Required?.ErrorMessage) ? _Required?.ErrorMessage 
                        : (property.GetDisplayName().Replace("'", "\'") + " is required.")) + "'");
                }
                sb.AppendLineIndented("};");
            }
            sb.AppendLineIndented("}");
        }

        public void BuildVuelidateRule(ScriptBuilder sb, string propertyName, TsProperty property, IReadOnlyDictionary<string, TsProperty> allProperties, ISet<string> constNamesInUse)
        {
            sb.AppendIndented(@"required");
        }

        private bool DisallowAllDefaultValues()
        {
            return _DataMember?.EmitDefaultValue != null && _DataMember?.EmitDefaultValue == false;
        }
    }
}
