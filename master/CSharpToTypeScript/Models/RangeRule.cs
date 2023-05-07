using System.ComponentModel.DataAnnotations;

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
                    sb.AppendLineIndented("if (values." + propertyName + " > " + _Range.Maximum + ") {");
                    using (sb.IncreaseIndentation())
                    {
                        sb.AppendLineIndented("errors." + propertyName + " = {");
                        using (sb.IncreaseIndentation())
                        {
                            sb.AppendLineIndented("type: 'max',");
                            sb.AppendLineIndented("message: '" + (!string.IsNullOrEmpty(_Range.ErrorMessage) ? _Range.ErrorMessage
                                : (property.GetDisplayName() + " cannot exceed " + _Range.Maximum + ".")) + "'");
                        }
                        sb.AppendLineIndented("};");
                    }
                    sb.AppendLineIndented("}");
                }

                if (_Range.Minimum != null)
                {
                    sb.AppendLineIndented("if (values." + propertyName + " < " + _Range.Minimum + ") {");
                    using (sb.IncreaseIndentation())
                    {
                        sb.AppendLineIndented("errors." + propertyName + " = {");
                        using (sb.IncreaseIndentation())
                        {
                            sb.AppendLineIndented("type: 'min',");
                            sb.AppendLineIndented("message: '" + (!string.IsNullOrEmpty(_Range.ErrorMessage) ? _Range.ErrorMessage
                                : (property.GetDisplayName() + " cannot be less than " + _Range.Minimum + ".")) + "'");
                        }
                        sb.AppendLineIndented("};");
                    }
                    sb.AppendLineIndented("}");
                }
            }
            sb.AppendLineIndented("}");
        }
    }
}
