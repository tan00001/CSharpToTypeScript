using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpToTypeScript.Models
{
    public class RangeRule : ITsValidationRule
    {
        readonly RangeAttribute _Range;

        public RangeRule(RangeAttribute range) 
        {
            _Range = range;
        }

        public void BuildRule(ScriptBuilder sb, string propertyName, TsProperty property, IReadOnlyDictionary<string, TsProperty> allProperties)
        {
            sb.AppendLineIndented("if (values." + propertyName + ") {");
            using (sb.IncreaseIndentation())
            {
                if (_Range.Maximum != null)
                {
                    sb.AppendLineIndented("if (values." + propertyName + ".Length > " + _Range.Maximum + ") {");
                    using (sb.IncreaseIndentation())
                    {
                        sb.AppendLineIndented("errorBuffer." + propertyName + ".push({");
                        using (sb.IncreaseIndentation())
                        {
                            sb.AppendLineIndented("type: 'max',");
                            sb.AppendLineIndented("message: '" + (!string.IsNullOrEmpty(_Range.ErrorMessage) ? _Range.ErrorMessage
                                : property.GetDisplayName() + " cannot exceed " + _Range.Maximum + ".") + "'");
                        }
                        sb.AppendLineIndented("});");
                    }
                    sb.AppendLineIndented("}");
                }

                if (_Range.Minimum != null)
                {
                    sb.AppendLineIndented("if (values." + propertyName + ".Length < " + _Range.Minimum + ") {");
                    using (sb.IncreaseIndentation())
                    {
                        sb.AppendLineIndented("errorBuffer." + propertyName + ".push({");
                        using (sb.IncreaseIndentation())
                        {
                            sb.AppendLineIndented("type: 'minLength',");
                            sb.AppendLineIndented("message: '" + (!string.IsNullOrEmpty(_Range.ErrorMessage) ? _Range.ErrorMessage
                                : property.GetDisplayName() + " cannot be less than " + _Range.Minimum + ".") + "'");
                        }
                        sb.AppendLineIndented("});");
                    }
                    sb.AppendLineIndented("}");
                }
            }
            sb.AppendLineIndented("}");
        }
    }
}
