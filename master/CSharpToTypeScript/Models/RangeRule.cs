using System.ComponentModel.DataAnnotations;

namespace CSharpToTypeScript.Models
{
    public class RangeRule : ITsValidationRule
    {
        readonly RangeAttribute _Range;

        readonly DataTypeAttribute? _DataType;

        static readonly Int32[] _DurationMultipliers = new[]{ 1000, 60000, 3600000, 86400000 };

        static readonly DateTime _Epoch = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public RangeRule(RangeAttribute range, DataTypeAttribute? dataType) 
        {
            _Range = range;
            _DataType = dataType;
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
                    sb.AppendLineIndented("if (values." + propertyName + " && !isNaN(values." + propertyName + ".getTime())) {");
                    using (sb.IncreaseIndentation())
                    {
                        sb.AppendLineIndented("const propValue: number = values." + propertyName + ".getTime();");
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
                if (IsDateType())
                {
                    sb.AppendLineIndented("const propValue: number = Date.parse(values." + propertyName + ");");
                    if (_Range.Maximum != null)
                    {
                        AppendMaxDateStringRule(sb, propertyName, property);
                    }

                    if (_Range.Minimum != null)
                    {
                        AppendMinDateStringRule(sb, propertyName, property);
                    }
                }
                else if (IsCurrencyType())
                {
                    sb.AppendLineIndented("const propValue: number = parseFloat(values." + propertyName + @".replace(/\D/g, ''));");
                    if (_Range.Maximum != null)
                    {
                        AppendMaxCurrencyRule(sb, propertyName, property);
                    }

                    if (_Range.Minimum != null)
                    {
                        AppendMinCurrencyRule(sb, propertyName, property);
                    }
                }
                else if (IsDurationType())
                {
                    sb.AppendLineIndented("const multipliers = [1000, 60000, 3600000, 86400000];");
                    sb.AppendLineIndented("const parts = values." + propertyName + ".split(':').reverse().map(parseFloat);");
                    if (_Range.Maximum != null)
                    {
                        AppendMaxDurationRule(sb, propertyName, property);
                    }

                    if (_Range.Minimum != null)
                    {
                        AppendMinDurationRule(sb, propertyName, property);
                    }
                }
                else
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
            var minimum = GetTimeSinceEpoch(_Range.Minimum);
            sb.AppendLineIndented("if (isNaN(propValue) || propValue < " + minimum + ") {");
            using (sb.IncreaseIndentation())
            {
                sb.AppendLineIndented("errors." + propertyName + " = {");
                using (sb.IncreaseIndentation())
                {
                    sb.AppendLineIndented("type: 'min',");
                    sb.AppendLineIndented("message: '" + (!string.IsNullOrEmpty(_Range.ErrorMessage) ? _Range.ErrorMessage
                        : (property.GetDisplayName() + " cannot be ealier than " + _Range.Minimum + ".")) + "'");
                }
                sb.AppendLineIndented("};");
            }
            sb.AppendLineIndented("}");
        }

        private void AppendMaxDateRule(ScriptBuilder sb, string propertyName, TsProperty property)
        {
            var maximum = GetTimeSinceEpoch(_Range.Maximum);
            sb.AppendLineIndented("if (isNaN(propValue) || propValue > " + maximum + ") {");
            using (sb.IncreaseIndentation())
            {
                sb.AppendLineIndented("errors." + propertyName + " = {");
                using (sb.IncreaseIndentation())
                {
                    sb.AppendLineIndented("type: 'max',");
                    sb.AppendLineIndented("message: '" + (!string.IsNullOrEmpty(_Range.ErrorMessage) ? _Range.ErrorMessage
                        : (property.GetDisplayName() + " cannot be later than " + _Range.Maximum + ".")) + "'");
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

        private void AppendMinDurationRule(ScriptBuilder sb, string propertyName, TsProperty property)
        {
            if (!TryParseDuration(_Range.Minimum, out decimal minimum))
            {
                throw new ArgumentException("\"" + _Range.Minimum.ToString() + "\" is not a valid maximum.");
            }

            sb.AppendLineIndented("const propValueForMin: number = parts.some(p => isNaN(p)) ? " + (minimum - 1) + " : parts.reduce((total, part, index) =>");
            using (sb.IncreaseIndentation())
            {
                sb.AppendLineIndented("(index < multipliers.length ? total + part * multipliers[index] : total), 0);");
            }
            sb.AppendLineIndented("if (propValueForMin < " + minimum + ") {");
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

        private void AppendMaxDurationRule(ScriptBuilder sb, string propertyName, TsProperty property)
        {
            if(!TryParseDuration(_Range.Maximum, out decimal maximum))
            {
                throw new ArgumentException("\"" + _Range.Maximum.ToString() + "\" is not a valid maximum.");
            }

            sb.AppendLineIndented("const propValueForMax: number = parts.some(p => isNaN(p)) ? " + (maximum + 1) + " : parts.reduce((total, part, index) =>");
            using (sb.IncreaseIndentation())
            {
                sb.AppendLineIndented("(index < multipliers.length ? total + part * multipliers[index] : total), 0);");
            }
            sb.AppendLineIndented("if (propValueForMax > " + maximum + ") {");
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

        private void AppendMinCurrencyRule(ScriptBuilder sb, string propertyName, TsProperty property)
        {
            if (!decimal.TryParse(_Range.Minimum.ToString(), out var minumum))
            {
                throw new ArgumentException("\"" + _Range.Minimum.ToString() + "\" is not a valid minimum.");
            }

            sb.AppendLineIndented("if (isNaN(propValue) || propValue < " + minumum + ") {");
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

        private void AppendMaxCurrencyRule(ScriptBuilder sb, string propertyName, TsProperty property)
        {
            if (!decimal.TryParse(_Range.Maximum.ToString(), out var maximum))
            {
                throw new ArgumentException("\"" + _Range.Maximum.ToString() + "\" is not a valid maximum.");
            }
            sb.AppendLineIndented("if (isNaN(propValue) || propValue > " + maximum + ") {");
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

        private void AppendMinDateStringRule(ScriptBuilder sb, string propertyName, TsProperty property)
        {
            var minimum = GetTimeSinceEpoch(_Range.Minimum);
            sb.AppendLineIndented("if (isNaN(propValue) || propValue < " + minimum + ") {");
            using (sb.IncreaseIndentation())
            {
                sb.AppendLineIndented("errors." + propertyName + " = {");
                using (sb.IncreaseIndentation())
                {
                    sb.AppendLineIndented("type: 'min',");
                    sb.AppendLineIndented("message: '" + (!string.IsNullOrEmpty(_Range.ErrorMessage) ? _Range.ErrorMessage
                        : (property.GetDisplayName() + " cannot be earlier than " + _Range.Minimum + ".")) + "'");
                }
                sb.AppendLineIndented("};");
            }
            sb.AppendLineIndented("}");
        }

        private void AppendMaxDateStringRule(ScriptBuilder sb, string propertyName, TsProperty property)
        {
            var maximum = GetTimeSinceEpoch(_Range.Maximum);
            sb.AppendLineIndented("if (isNaN(propValue) || propValue > " + maximum + ") {");
            using (sb.IncreaseIndentation())
            {
                sb.AppendLineIndented("errors." + propertyName + " = {");
                using (sb.IncreaseIndentation())
                {
                    sb.AppendLineIndented("type: 'max',");
                    sb.AppendLineIndented("message: '" + (!string.IsNullOrEmpty(_Range.ErrorMessage) ? _Range.ErrorMessage
                        : (property.GetDisplayName() + " cannot be later than " + _Range.Maximum + ".")) + "'");
                }
                sb.AppendLineIndented("};");
            }
            sb.AppendLineIndented("}");
        }

        private static double GetTimeSinceEpoch(object dateTime)
        {
            if (!DateTimeOffset.TryParse(dateTime.ToString(), out var dateTimeOffset))
            {
                throw new ArgumentException("Invalid date time.", nameof(dateTime));
            }

            return (dateTimeOffset - _Epoch).TotalMilliseconds;
        }

        private bool IsDateType()
        {
            if (_DataType == null)
            {
                return _Range.OperandType == typeof(DateTime) || _Range.OperandType == typeof(DateTimeOffset);
            };

            return _DataType.DataType == DataType.Date || _DataType.DataType == DataType.DateTime || _DataType.DataType == DataType.Time;
        }

        private bool IsCurrencyType()
        {
            return _DataType?.DataType == DataType.Currency;
        }

        private bool IsDurationType()
        {
            return _Range.OperandType == typeof(TimeSpan) ||  _DataType?.DataType == DataType.Duration;
        }

        private static bool TryParseDuration(object durationSetting, out decimal duration)
        {
            if (durationSetting is int durationInt)
            {
                duration = durationInt;
                return true;
            }

            if (durationSetting is double durationDouble)
            {
                duration = Convert.ToDecimal(durationDouble);
                return true;
            }

            System.Diagnostics.Debug.Assert(durationSetting is string);
            string durationString = (string)durationSetting;

            duration = 0;

            var parts = durationString.Split(':', StringSplitOptions.RemoveEmptyEntries).Reverse().ToList();
            if (parts.Count == 0)
            {
                return false;
            }

            Int32 partCount = Math.Min(_DurationMultipliers.Length, parts.Count);
            for (var i = 0; i < partCount; i++)
            {
                if (!decimal.TryParse(parts[i], out var part))
                {
                    return false;
                }
                duration += part * _DurationMultipliers[i];
            }

            return true;
        }
    }
}
