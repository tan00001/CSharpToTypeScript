using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpToTypeScript.Models
{
    public class CreditCardRule : ITsValidationRule
    {
        readonly CreditCardAttribute _CreditCard;

        public CreditCardRule(CreditCardAttribute creditCard) 
        {
            _CreditCard = creditCard;
        }

        public void BuildRule(ScriptBuilder sb, string propertyName, TsProperty property, IReadOnlyDictionary<string, TsProperty> allProperties)
        {
            sb.AppendLine("if (values." + propertyName + " && ([...values." + propertyName + "].filter(c => c >= '0' && c <= '9')");
            sb.AppendLineIndented(".reduce((checksum, c, index) => {");
            using (sb.IncreaseIndentation())
            { 
                sb.AppendLineIndented("var digitValue = (digit - '0') * (index & 2) !== 0 ? 2 : 1);");
                sb.AppendLineIndented("while (digitValue > 0) {");
                using (sb.IncreaseIndentation())
                { 
                    sb.AppendLineIndented("checksum += digitValue % 10;");
                    sb.AppendLineIndented("digitValue /= 10;");
                    sb.AppendLine("}");
                }
                sb.AppendLineIndented("return checksum;");
                sb.AppendLine("}, 0) % 10) === 0) {");
            }
            using (sb.IncreaseIndentation())
            {
                sb.AppendLine("errorBuffer." + propertyName + ".push({");
                using (sb.IncreaseIndentation())
                {
                    sb.AppendLine("type: 'pattern',");
                    sb.AppendLineIndented("message: '" + (!string.IsNullOrEmpty(_CreditCard.ErrorMessage) ? _CreditCard.ErrorMessage
                        : property.GetDisplayName() + " is invalid.") + "'");
                    sb.AppendLine("});");
                }
                sb.AppendLine("}");
            }
            sb.AppendLine("}");
        }
    }
}
