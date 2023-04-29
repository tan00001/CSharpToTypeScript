using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace CSharpToTypeScript.Models
{
    public class TsEnum : TsModuleMember
    {
        public bool IsIgnored { get; set; }

        public ICollection<TsEnumValue> Values { get; private set; }

        public TsEnum(Type type)
          : base(type)
        {
            if (!this.Type.IsEnum)
                throw new ArgumentException("ClrType isn't enum.");
            this.Values = new List<TsEnumValue>(this.GetEnumValues(type));

            var displayAttribute = this.Type.GetCustomAttribute<DisplayAttribute>(false);

            if (displayAttribute != null)
            {
                var displayName = displayAttribute.GetName();
                if (!string.IsNullOrEmpty(displayName))
                    this.Name = displayName;
            }
        }

        protected IEnumerable<TsEnumValue> GetEnumValues(System.Type enumType) => enumType.GetFields()
            .Where(fieldInfo => fieldInfo.IsLiteral && !string.IsNullOrEmpty(fieldInfo.Name))
            .Select(fieldInfo => new TsEnumValue(fieldInfo));
    }
}
