using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpToTypeScript.Models
{
    public class CompareRule : ITsValidationRule
    {
        readonly CompareAttribute _Compare;

        public CompareRule(CompareAttribute compare) 
        {
            _Compare = compare;
        }

        public void BuildRule(ScriptBuilder sb, string propertyName, TsProperty property, IReadOnlyDictionary<string, TsProperty> allProperties)
        {
            var otherPropertyName = FindOrderPropertyName(allProperties);

            sb.AppendLine("if (values." + propertyName + " !== values." + otherPropertyName + ") {");
            using (sb.IncreaseIndentation())
            {
                sb.AppendLine("errorBuffer." + propertyName + ".push({");
                using (sb.IncreaseIndentation())
                {
                    sb.AppendLine("type: 'pattern',");
                    sb.AppendLineIndented("message: '" + (!string.IsNullOrEmpty(_Compare.ErrorMessage) ? _Compare.ErrorMessage
                        : property.GetDisplayName() + " is invalid.") + "'");
                    sb.AppendLine("});");
                }
                sb.AppendLine("}");
            }
            sb.AppendLine("}");
        }

        private string FindOrderPropertyName(IReadOnlyDictionary<string, TsProperty> allProperties)
        {
            foreach (var property in allProperties)
            {
                if (property.Value.Name == _Compare.OtherProperty)
                {
                    return property.Key;
                }
            }

            throw new ArgumentException("Cannot find property \"" + _Compare.OtherProperty + "\"", nameof(allProperties));
        }
    }
}
