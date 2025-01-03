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

        public void BuildRule(ScriptBuilder sb, string propertyName, TsProperty property, IReadOnlyDictionary<string, TsProperty> allProperties, ISet<string> constNamesInUse)
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
                        : (property.GetDisplayName().Replace("'", "\'") + " is invalid.")) + "'");
                }
                sb.AppendLineIndented("};");
            }
            sb.AppendLineIndented("}");
        }

        public void BuildVuelidateRule(ScriptBuilder sb, string propertyName, TsProperty property, IReadOnlyDictionary<string, TsProperty> allProperties, ISet<string> constNamesInUse)
        {
            sb.AppendIndented("numberPattern: helpers.regex(/^(?:" +
                "4\\d{12}(?:\\d{3})?" +            // Visa (13 or 16 digits)
                "|5[1-5]\\d{14}" +                 // MasterCard (16 digits)
                "|6(?:011|5\\d{2})\\d{12}" +       // Discover (16 digits)
                "|3[47]\\d{13}" +                  // American Express (15 digits)
                "|3(?:0[0-5]|[68]\\d)\\d{11}" +    // Diners Club (14 digits)
                "|(?:2131|1800|35\\d{3})\\d{11}" + // JCB (15 digits)
                ")$/)");
        }
    }
}
