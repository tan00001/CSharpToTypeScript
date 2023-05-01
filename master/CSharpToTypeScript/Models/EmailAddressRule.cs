using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpToTypeScript.Models
{
    public class EmailAddressRule : ITsValidationRule
    {
        readonly EmailAddressAttribute _EmailAddress;

        public EmailAddressRule(EmailAddressAttribute emailAddress) 
        {
            _EmailAddress = emailAddress;
        }

        public void BuildRule(ScriptBuilder sb, string propertyName, TsProperty property, IReadOnlyDictionary<string, TsProperty> allProperties)
        {
            sb.AppendLine("if (values." + propertyName + @" && !/\S+@\S+\.\S+/.test(values." + propertyName + ") {");
            using (sb.IncreaseIndentation())
            {
                sb.AppendLine("errorBuffer." + propertyName + ".push({");
                using (sb.IncreaseIndentation())
                {
                    sb.AppendLine("type: 'pattern',");
                    sb.AppendLineIndented("message: '" + (!string.IsNullOrEmpty(_EmailAddress.ErrorMessage) ? _EmailAddress.ErrorMessage
                        : property.GetDisplayName() + " is invalid.") + "'");
                    sb.AppendLine("});");
                }
                sb.AppendLine("}");
            }
            sb.AppendLine("}");
        }
    }
}
