namespace CSharpToTypeScript
{
    public class TypeConvertorCollection
    {
        internal Dictionary<Type, TypeConvertor> _convertors;

        public TypeConvertorCollection() => this._convertors = new Dictionary<Type, TypeConvertor>();

        public void RegisterTypeConverter<TFor>(TypeConvertor convertor) => this._convertors[typeof(TFor)] = convertor;

        public bool IsConvertorRegistered(Type type) => this._convertors.ContainsKey(type);

        public string? ConvertType(Type type) => _convertors.TryGetValue(type, out TypeConvertor? value) ? value(type) : (string?)null;
    }
}
