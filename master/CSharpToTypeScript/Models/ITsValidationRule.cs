namespace CSharpToTypeScript.Models
{
    public interface ITsValidationRule
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="propertyName"></param>
        /// <param name="property"></param>
        /// <param name="allProperties"></param>
        /// <returns>Local const names used</returns>
        void BuildRule(ScriptBuilder sb, string propertyName, TsProperty property, IReadOnlyDictionary<string, TsProperty> allProperties, ISet<string> constNamesInUse);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="propertyName"></param>
        /// <param name="property"></param>
        /// <param name="allProperties"></param>
        /// <returns>Local const names used</returns>
        void BuildVuelidateRule(ScriptBuilder sb, string propertyName, TsProperty property, IReadOnlyDictionary<string, TsProperty> allProperties, ISet<string> constNamesInUse);
    }
}
