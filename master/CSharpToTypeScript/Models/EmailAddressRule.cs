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
            sb.AppendLineIndented("if (values." + propertyName + @" && !/\S+@\S+\.\S+/.test(values." + propertyName + ")) {");
            using (sb.IncreaseIndentation())
            {
                sb.AppendLineIndented("errorBuffer." + propertyName + ".push({");
                using (sb.IncreaseIndentation())
                {
                    sb.AppendLineIndented("type: 'pattern',");
                    sb.AppendLineIndented("message: '" + (!string.IsNullOrEmpty(_EmailAddress.ErrorMessage) ? string.Format(_EmailAddress.ErrorMessage, property.GetDisplayName())
                        : (property.GetDisplayName() + " is invalid.") + "'"));
                }
                sb.AppendLineIndented("});");
            }
            sb.AppendLineIndented("}");
        }
    }
}
