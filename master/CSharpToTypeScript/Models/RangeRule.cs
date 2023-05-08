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
            if (property.PropertyType is TsSystemType tsSystemType)
            {
                if (tsSystemType.Kind == SystemTypeKind.Number)
                {
                    sb.AppendLineIndented("if (values." + propertyName + " || values." + propertyName + " === 0) {");
                    using (sb.IncreaseIndentation())
                    {
                        if (_Range.Maximum != null)
                        {
                            AppendMaxNumberRule(sb, propertyName, property);
                        }

                        if (_Range.Minimum != null)
                        {
                            AppendMinNumberRule(sb, propertyName, property);
                        }
                    }
                    sb.AppendLineIndented("}");
                    return;
                }
                else if (tsSystemType.Kind == SystemTypeKind.Date)
                {
                    sb.AppendLineIndented("if (values." + propertyName + " || values." + propertyName + " === 0) {");
                    using (sb.IncreaseIndentation())
                    {
                        if (_Range.Maximum != null)
                        {
                            AppendMaxDateRule(sb, propertyName, property);
                        }

                        if (_Range.Minimum != null)
                        {
                            AppendMinDateRule(sb, propertyName, property);
                        }
                    }
                    sb.AppendLineIndented("}");
                    return;
                }
            }

            sb.AppendLineIndented("if (values." + propertyName + ") {");
            using (sb.IncreaseIndentation())
            {
                if (_Range.Maximum != null)
                {
                    AppendMaxRule(sb, propertyName, property);
                }

                if (_Range.Minimum != null)
                {
                    AppendMinRule(sb, propertyName, property);
                }
            }

            sb.AppendLineIndented("}");
        }

        private void AppendMinNumberRule(ScriptBuilder sb, string propertyName, TsProperty property)
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

        private void AppendMaxNumberRule(ScriptBuilder sb, string propertyName, TsProperty property)
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

        private void AppendMinDateRule(ScriptBuilder sb, string propertyName, TsProperty property)
        {
            sb.AppendLineIndented("const propValue: number = Date.parse(values." + propertyName + ");");
            sb.AppendLineIndented("const minValue: number =  Date.parse('" + _Range.Minimum + "');");
            sb.AppendLineIndented("if (isNaN(propValue) || propValue < minValue) {");
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

        private void AppendMaxDateRule(ScriptBuilder sb, string propertyName, TsProperty property)
        {
            sb.AppendLineIndented("const propValue: number = Date.parse(values." + propertyName + ");");
            sb.AppendLineIndented("const maxValue: number = Date.parse('" + _Range.Maximum + "');");
            sb.AppendLineIndented("if (isNaN(propValue) || propValue > maxValue) {");
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

        private void AppendMinRule(ScriptBuilder sb, string propertyName, TsProperty property)
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

        private void AppendMaxRule(ScriptBuilder sb, string propertyName, TsProperty property)
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
    }
}
