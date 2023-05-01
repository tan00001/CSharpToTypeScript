using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpToTypeScript.Models
{
    public interface ITsValidationRule
    {
        void BuildRule(ScriptBuilder sb, string propertyName, TsProperty property, IReadOnlyDictionary<string, TsProperty> allProperties);
    }
}
