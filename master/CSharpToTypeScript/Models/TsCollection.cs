using System.Collections;
using System.Diagnostics;

namespace CSharpToTypeScript.Models
{
    [DebuggerDisplay("TsCollection - ItemsType={ItemsType}")]
    public class TsCollection : TsType
    {
        public TsType ItemsType { get; set; }

        public int Dimension { get; set; }

        public TsCollection(ITsModuleService tsModuleService, Type type)
          : base(type)
        {
            Type? enumerableType = TsType.GetEnumeratedType(this.Type);
            if (enumerableType == this.Type)
                this.ItemsType = TsType.Any;
            else if (enumerableType != null)
                this.ItemsType = TsType.Create(tsModuleService, enumerableType);
            else if (typeof(IEnumerable).IsAssignableFrom(this.Type))
                this.ItemsType = TsType.Any;
            else
                throw new ArgumentException(string.Format("The type '{0}' is not collection.", this.Type.FullName));
            this.Dimension = TsCollection.GetCollectionDimension(type);
        }

        private static int GetCollectionDimension(Type t)
        {
            if (t.IsArray)
            {
                return TsCollection.GetCollectionDimension(t.GetElementType()!) + 1;
            }

            Type? enumerableType;
            return t != typeof(string) && (enumerableType = TsType.GetEnumeratedType(t)) != null && enumerableType != t ? TsCollection.GetCollectionDimension(enumerableType) + 1 : 0;
        }
    }
}
