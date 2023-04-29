using System;
using System.Collections.Generic;
using CSharpToTypeScript.Models;

namespace CSharpToTypeScript
{
    public class TsTypeFormatterCollection : ITsTypeFormatter
    {
        internal Dictionary<Type, TsTypeFormatter> _formatters;

        internal TsTypeFormatterCollection() => this._formatters = new Dictionary<Type, TsTypeFormatter>();

        public string FormatType(TsType type) => _formatters.TryGetValue(type.GetType(), out TsTypeFormatter? value) ? value(type, this) : "any";

        public void RegisterTypeFormatter<TFor>(TsTypeFormatter formatter) where TFor : TsType => this._formatters[typeof(TFor)] = formatter;
    }
}
