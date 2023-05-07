namespace CSharpToTypeScript.Models
{
    public interface ITsValidationRule
    {
        void BuildRule(ScriptBuilder sb, string propertyName, TsProperty property, IReadOnlyDictionary<string, TsProperty> allProperties);
    }
}
