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
            FindOrderPropertyName(allProperties, out var otherPropertyName, out var otherPropertyDisplayName);

            sb.AppendLineIndented("if (values." + propertyName + " !== values." + otherPropertyName + ") {");
            using (sb.IncreaseIndentation())
            {
                sb.AppendLineIndented("errors." + propertyName + " = {");
                using (sb.IncreaseIndentation())
                {
                    sb.AppendLineIndented("type: 'pattern',");
                    sb.AppendLineIndented("message: '" + (!string.IsNullOrEmpty(_Compare.ErrorMessage) ? _Compare.ErrorMessage
                        : (otherPropertyDisplayName + " does not match.")) + "'");
                }
                sb.AppendLineIndented("};");
            }
            sb.AppendLineIndented("}");
        }

        private void FindOrderPropertyName(IReadOnlyDictionary<string, TsProperty> allProperties, out string otherPropertyName, out string otherPropertyDisplayName)
        {
            foreach (var property in allProperties)
            {
                if (property.Value.Name == _Compare.OtherProperty)
                {
                    otherPropertyName = property.Key;
                    otherPropertyDisplayName = property.Value.GetDisplayName();
                    return;
                }
            }

            throw new ArgumentException("Cannot find property \"" + _Compare.OtherProperty + "\"", nameof(allProperties));
        }
    }
}
