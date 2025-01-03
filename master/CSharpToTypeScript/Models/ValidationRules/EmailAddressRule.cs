using System.ComponentModel.DataAnnotations;

namespace CSharpToTypeScript.Models
{
    public class EmailAddressRule : ITsValidationRule
    {
        readonly EmailAddressAttribute _EmailAddress;

        public EmailAddressRule(EmailAddressAttribute emailAddress) 
        {
            _EmailAddress = emailAddress;
        }

        public void BuildRule(ScriptBuilder sb, string propertyName, TsProperty property, IReadOnlyDictionary<string, TsProperty> allProperties, ISet<string> constNamesInUse)
        {
            sb.AppendLineIndented("if (values." + propertyName + @" && !/\S+@\S+\.\S+/.test(values." + propertyName + ")) {");
            using (sb.IncreaseIndentation())
            {
                sb.AppendLineIndented("errors." + propertyName + " = {");
                using (sb.IncreaseIndentation())
                {
                    sb.AppendLineIndented("type: 'pattern',");
                    sb.AppendLineIndented("message: '" + (!string.IsNullOrEmpty(_EmailAddress.ErrorMessage) ? string.Format(_EmailAddress.ErrorMessage, property.GetDisplayName())
                        : (property.GetDisplayName().Replace("'", "\'") + " is invalid.")) + "'");
                }
                sb.AppendLineIndented("};");
            }
            sb.AppendLineIndented("}");
        }

        public void BuildVuelidateRule(ScriptBuilder sb, string propertyName, TsProperty property, IReadOnlyDictionary<string, TsProperty> allProperties, ISet<string> constNamesInUse)
        {
            sb.AppendIndented("email");
        }
    }
}
