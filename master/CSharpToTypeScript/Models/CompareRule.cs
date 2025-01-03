using System.ComponentModel.DataAnnotations;

namespace CSharpToTypeScript.Models
{
    public class CompareRule : ITsValidationRule
    {
        readonly CompareAttribute _Compare;

        public CompareRule(CompareAttribute compare) 
        {
            _Compare = compare;
        }

        public void BuildRule(ScriptBuilder sb, string propertyName, TsProperty property, IReadOnlyDictionary<string, TsProperty> allProperties, ISet<string> constNamesInUse)
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

        public void BuildVuelidateRule(ScriptBuilder sb, string propertyName, TsProperty property, IReadOnlyDictionary<string, TsProperty> allProperties, ISet<string> constNamesInUse)
        {
            FindOrderPropertyName(allProperties, out var otherPropertyName, out var otherPropertyDisplayName);

            sb.AppendIndented("helpers.withMessage('" + (!string.IsNullOrEmpty(_Compare.ErrorMessage) ? _Compare.ErrorMessage
                : (otherPropertyDisplayName + " does not match.")) + "', (value, siblings) => value === siblings." + otherPropertyDisplayName + ')'); 
                // (value, siblings, root) => { ... } if you need access to the root form\r\n)");
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
