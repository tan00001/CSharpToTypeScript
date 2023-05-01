using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace CSharpToTypeScript.Models
{
    public class TsEnum : TsModuleMember
    {
        public ICollection<TsEnumValue> Values { get; private set; }

        public TsEnum(Type type)
          : base(type)
        {
            if (!this.Type.IsEnum)
                throw new ArgumentException("ClrType isn't enum.");

            this.Values = new List<TsEnumValue>(this.GetEnumValues(type));
        }

        protected IEnumerable<TsEnumValue> GetEnumValues(Type enumType) => enumType.GetFields()
            .Where(fieldInfo => fieldInfo.IsLiteral && !string.IsNullOrEmpty(fieldInfo.Name))
            .Select(fieldInfo => new TsEnumValue(fieldInfo));
    }
}
