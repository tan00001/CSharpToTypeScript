using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpToTypeScript.Models
{
    public class RegularExpressionRule : ITsValidationRule
    {
        readonly RegularExpressionAttribute _ReqularExpression;

        public RegularExpressionRule(RegularExpressionAttribute regularExpression) 
        {
            _ReqularExpression = regularExpression;
        }

        public void BuildRule(ScriptBuilder sb, string propertyName, TsProperty property, IReadOnlyDictionary<string, TsProperty> allProperties)
        {
            sb.AppendLineIndented("if (values." + propertyName + " && !/" + _ReqularExpression.Pattern + "/.test(values." + propertyName + ")) {");
            using (sb.IncreaseIndentation())
            {
                sb.AppendLineIndented("errors." + propertyName + " = {");
                using (sb.IncreaseIndentation())
                {
                    sb.AppendLineIndented("type: 'pattern',");
                    sb.AppendLineIndented("message: '" + (!string.IsNullOrEmpty(_ReqularExpression.ErrorMessage) ? _ReqularExpression.ErrorMessage
                        :(property.GetDisplayName() + " is invalid.")) + "'");
                }
                sb.AppendLineIndented("};");
            }
            sb.AppendLineIndented("}");
        }
    }
}
