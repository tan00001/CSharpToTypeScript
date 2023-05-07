using System.ComponentModel.DataAnnotations;

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
            sb.AppendLineIndented("if (values." + propertyName + " && ([...values." + propertyName + "].filter(c => c >= '0' && c <= '9')");
            using (sb.IncreaseIndentation())
            { 
                sb.AppendLineIndented(".reduce((checksum, c, index) => {");
                using (sb.IncreaseIndentation())
                {
                    sb.AppendLineIndented("let digitValue = (c.charCodeAt(0) - 48) * ((index & 2) !== 0 ? 2 : 1);");
                    sb.AppendLineIndented("while (digitValue > 0) {");
                    using (sb.IncreaseIndentation())
                    {
                        sb.AppendLineIndented("checksum += digitValue % 10;");
                        sb.AppendLineIndented("digitValue /= 10;");
                    }
                    sb.AppendLineIndented("}");
                    sb.AppendLineIndented("return checksum;");
                }
                sb.AppendLineIndented("}, 0) % 10) !== 0) {");
            }
            using (sb.IncreaseIndentation())
            {
                sb.AppendLineIndented("errors." + propertyName + " = {");
                using (sb.IncreaseIndentation())
                {
                    sb.AppendLineIndented("type: 'pattern',");
                    sb.AppendLineIndented("message: '" + (!string.IsNullOrEmpty(_CreditCard.ErrorMessage) ? string.Format(_CreditCard.ErrorMessage, property.GetDisplayName())
                        : (property.GetDisplayName() + " is invalid.")) + "'");
                }
                sb.AppendLineIndented("};");
            }
            sb.AppendLineIndented("}");
        }
    }
}
