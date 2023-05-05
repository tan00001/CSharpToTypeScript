using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpToTypeScript.Models
{
    public class UrlRule : ITsValidationRule
    {
        readonly UrlAttribute _Url;

        public UrlRule(UrlAttribute url) 
        {
            _Url = url;
        }

        public void BuildRule(ScriptBuilder sb, string propertyName, TsProperty property, IReadOnlyDictionary<string, TsProperty> allProperties)
        {
            sb.AppendLineIndented("if (values." + propertyName + @" && !/^(http:\/\/|https:\/\/|ftp:\/\/)/.test(values." + propertyName + ")) {");
            using (sb.IncreaseIndentation())
            {
                sb.AppendLineIndented("errors." + propertyName + " = {");
                using (sb.IncreaseIndentation())
                {
                    sb.AppendLineIndented("type: 'pattern',");
                    sb.AppendLineIndented("message: '" + (!string.IsNullOrEmpty(_Url.ErrorMessage) ? string.Format(_Url.ErrorMessage, property.GetDisplayName())
                        : (property.GetDisplayName() + " is invalid.")) + "'");
                }
                sb.AppendLineIndented("};");
            }
            sb.AppendLineIndented("}");
        }
    }
}
