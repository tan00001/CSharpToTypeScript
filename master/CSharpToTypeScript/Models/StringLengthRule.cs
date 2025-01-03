﻿using System.ComponentModel.DataAnnotations;

namespace CSharpToTypeScript.Models
{
    public class StringLengthRule : ITsValidationRule
    {
        readonly StringLengthAttribute _StringLength;

        public StringLengthRule(StringLengthAttribute stringLength) 
        {
            _StringLength = stringLength;
        }

        public void BuildRule(ScriptBuilder sb, string propertyName, TsProperty property, IReadOnlyDictionary<string, TsProperty> allProperties, ISet<string> constNamesInUse)
        {
            sb.AppendLineIndented("if ((values." + propertyName + "?.length ?? 0) > " + _StringLength.MaximumLength + ") {");
            using (sb.IncreaseIndentation())
            {
                sb.AppendLineIndented("errors." + propertyName + " = {");
                using (sb.IncreaseIndentation())
                {
                    sb.AppendLineIndented("type: 'maxLength',");
                    sb.AppendLineIndented("message: '" + (!string.IsNullOrEmpty(_StringLength.ErrorMessage) ? _StringLength.ErrorMessage
                        : (property.GetDisplayName().Replace("'", "\'") + " cannot exceed " + _StringLength.MaximumLength + " characters.")) + "'");
                }
                sb.AppendLineIndented("};");
            }
            sb.AppendLineIndented("}");

            if (_StringLength.MinimumLength > 0)
            {
                sb.AppendLineIndented("if ((values." + propertyName + "?.length ?? 0) < " + _StringLength.MinimumLength + ") {");
                using (sb.IncreaseIndentation())
                {
                    sb.AppendLineIndented("errors." + propertyName + " = {");
                    using (sb.IncreaseIndentation())
                    {
                        sb.AppendLineIndented("type: 'minLength',");
                        sb.AppendLineIndented("message: '" + (!string.IsNullOrEmpty(_StringLength.ErrorMessage) ? _StringLength.ErrorMessage
                            : (property.GetDisplayName().Replace("'", "\'") + " cannot be less than " + _StringLength.MinimumLength + " characters long.")) + "'");
                    }
                    sb.AppendLineIndented("};");
                }
                sb.AppendLineIndented("}");
            }
        }

        public void BuildVuelidateRule(ScriptBuilder sb, string propertyName, TsProperty property, IReadOnlyDictionary<string, TsProperty> allProperties, ISet<string> constNamesInUse)
        {
            if (_StringLength.MinimumLength > 0)
            {
                sb.AppendLineIndented(@"minLength: minLength(" + _StringLength.MinimumLength + "),");
            }

            sb.AppendIndented(@"maxLength: maxLength(" + _StringLength.MaximumLength + ")");
        }
    }
}
