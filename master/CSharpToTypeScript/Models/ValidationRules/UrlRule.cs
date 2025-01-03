﻿using System.ComponentModel.DataAnnotations;

namespace CSharpToTypeScript.Models
{
    public class UrlRule : ITsValidationRule
    {
        readonly UrlAttribute _Url;

        public UrlRule(UrlAttribute url) 
        {
            _Url = url;
        }

        public void BuildRule(ScriptBuilder sb, string propertyName, TsProperty property, IReadOnlyDictionary<string, TsProperty> allProperties, ISet<string> constNamesInUse)
        {
            sb.AppendLineIndented("if (values." + propertyName + @" && !/^(http:\/\/|https:\/\/|ftp:\/\/)/.test(values." + propertyName + ")) {");
            using (sb.IncreaseIndentation())
            {
                sb.AppendLineIndented("errors." + propertyName + " = {");
                using (sb.IncreaseIndentation())
                {
                    sb.AppendLineIndented("type: 'pattern',");
                    sb.AppendLineIndented("message: '" + (!string.IsNullOrEmpty(_Url.ErrorMessage) ? string.Format(_Url.ErrorMessage, property.GetDisplayName())
                        : (property.GetDisplayName().Replace("'", "\'") + " is invalid.")) + "'");
                }
                sb.AppendLineIndented("};");
            }
            sb.AppendLineIndented("}");
        }

        public void BuildVuelidateRule(ScriptBuilder sb, string propertyName, TsProperty property, IReadOnlyDictionary<string, TsProperty> allProperties, ISet<string> constNamesInUse)
        {
            sb.AppendIndented(@"urlPattern: helpers.regex(/^(http:\/\/|https:\/\/|ftp:\/\/)");
        }
    }
}
