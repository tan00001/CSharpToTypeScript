using System.ComponentModel.DataAnnotations;

namespace CSharpToTypeScript.Models
{
    public class PhoneNumberRule : ITsValidationRule
    {
        readonly PhoneAttribute _Phone;

        public PhoneNumberRule(PhoneAttribute phone) 
        {
            _Phone = phone;
        }

        public void BuildRule(ScriptBuilder sb, string propertyName, TsProperty property, IReadOnlyDictionary<string, TsProperty> allProperties, ISet<string> constNamesInUse)
        {
            sb.AppendLineIndented("if (values." + propertyName + @" && !/^(\+1|1)?[ -]?(\([2-9][0-9]{2}\)|[2-9][0-9]{2})[ -]?[2-9][0-9]{2}[ -]?[0-9]{4}$/.test(values." + propertyName + ")) {");
            using (sb.IncreaseIndentation())
            {
                sb.AppendLineIndented("errors." + propertyName + " = {");
                using (sb.IncreaseIndentation())
                {
                    sb.AppendLineIndented("type: 'pattern',");
                    sb.AppendLineIndented("message: '" + (!string.IsNullOrEmpty(_Phone.ErrorMessage) ? string.Format(_Phone.ErrorMessage, property.GetDisplayName())
                        : (property.GetDisplayName().Replace("'", "\'") + " is invalid.")) + "'");
                }
                sb.AppendLineIndented("};");
            }
            sb.AppendLineIndented("}");
        }
    }
}
