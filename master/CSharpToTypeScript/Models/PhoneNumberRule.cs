using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace CSharpToTypeScript.Models
{
    public class PhoneNumberRule : ITsValidationRule
    {
        readonly PhoneAttribute _Phone;

        public PhoneNumberRule(PhoneAttribute phone) 
        {
            _Phone = phone;
        }

        public void BuildRule(ScriptBuilder sb, string propertyName, TsProperty property, IReadOnlyDictionary<string, TsProperty> allProperties)
        {
            sb.AppendLineIndented("if (values." + propertyName + @" && !/^(\+1|1)?[ -]?(\([2-9][0-9]{2}\)|[2-9][0-9]{2})[ -]?[2-9][0-9]{2}[ -]?[0-9]{4}$/.test(values." + propertyName + ")) {");
            using (sb.IncreaseIndentation())
            {
                sb.AppendLineIndented("errorBuffer." + propertyName + ".push({");
                using (sb.IncreaseIndentation())
                {
                    sb.AppendLineIndented("type: 'pattern',");
                    sb.AppendLineIndented("message: '" + (!string.IsNullOrEmpty(_Phone.ErrorMessage) ? string.Format(_Phone.ErrorMessage, property.GetDisplayName())
                        : (property.GetDisplayName() + " is invalid.") + "'"));
                }
                sb.AppendLineIndented("});");
            }
            sb.AppendLineIndented("}");
        }
    }
}
