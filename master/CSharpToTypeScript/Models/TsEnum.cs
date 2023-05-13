namespace CSharpToTypeScript.Models
{
    public class TsEnum : TsModuleMember
    {
        public ICollection<TsEnumValue> Values { get; private set; }

        public TsEnum(ITsModuleService tsModuleService, Type type)
          : base(tsModuleService, type)
        {
            if (!this.Type.IsEnum)
                throw new ArgumentException("ClrType isn't enum.");

            this.Values = new List<TsEnumValue>(GetEnumValues(type));
        }

        protected static IEnumerable<TsEnumValue> GetEnumValues(Type enumType) => enumType.GetFields()
            .Where(fieldInfo => fieldInfo.IsLiteral && !string.IsNullOrEmpty(fieldInfo.Name))
            .Select(fieldInfo => new TsEnumValue(fieldInfo));
    }
}
