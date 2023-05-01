using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpToTypeScript.Models
{
    public class StringLengthRule : ITsValidationRule
    {
        readonly StringLengthAttribute _StringLength;

        public StringLengthRule(StringLengthAttribute stringLength) 
        {
            _StringLength = stringLength;
        }

        public void BuildRule(ScriptBuilder sb, string propertyName, TsProperty property, IReadOnlyDictionary<string, TsProperty> allProperties)
        {
            sb.AppendLineIndented("if ((values." + propertyName + "?.Length ?? 0) > " + _StringLength.MaximumLength + ") {");
            using (sb.IncreaseIndentation())
            {
                sb.AppendLineIndented("errorBuffer." + propertyName + ".push({");
                using (sb.IncreaseIndentation())
                {
                    sb.AppendLineIndented("type: 'maxLength',");
                    sb.AppendLineIndented("message: '" + (!string.IsNullOrEmpty(_StringLength.ErrorMessage) ? _StringLength.ErrorMessage
                        : property.GetDisplayName() + " cannot exceed " + _StringLength.MaximumLength + " characters.") + "'");
                }
                sb.AppendLineIndented("});");
            }
            sb.AppendLineIndented("}");

            if (_StringLength.MinimumLength > 0)
            {
                sb.AppendLineIndented("if ((values." + propertyName + "?.Length ?? 0) < " + _StringLength.MinimumLength + ") {");
                using (sb.IncreaseIndentation())
                {
                    sb.AppendLineIndented("errorBuffer." + propertyName + ".push({");
                    using (sb.IncreaseIndentation())
                    {
                        sb.AppendLineIndented("type: 'minLength',");
                        sb.AppendLineIndented("message: '" + (!string.IsNullOrEmpty(_StringLength.ErrorMessage) ? _StringLength.ErrorMessage
                            : property.GetDisplayName() + " cannot be less than " + _StringLength.MinimumLength + " characters long.") + "'");
                    }
                    sb.AppendLineIndented("});");
                }
                sb.AppendLineIndented("}");
            }
        }
    }
}
