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
            sb.AppendLine("if (values." + propertyName + @" && !/^(http:\/\/|https:\/\/|ftp:\/\/)/.test(values." + propertyName + ") {");
            using (sb.IncreaseIndentation())
            {
                sb.AppendLine("errorBuffer." + propertyName + ".push({");
                using (sb.IncreaseIndentation())
                {
                    sb.AppendLine("type: 'pattern',");
                    sb.AppendLineIndented("message: '" + (!string.IsNullOrEmpty(_Url.ErrorMessage) ? _Url.ErrorMessage
                        : property.GetDisplayName() + " is invalid.") + "'");
                    sb.AppendLine("});");
                }
                sb.AppendLine("}");
            }
            sb.AppendLine("}");
        }
    }
}
