namespace CSharpToTypeScript.Models
{
    public class TsClassDependencyComparer : IComparer<TsClass>
    {
        public int Compare(TsClass? x, TsClass? y)
        {
            return (x, y) switch
            {
                _ when x != null && y != null => CompareDependency(x, y),
                _ when x == null && y != null => -1,
                _ when x != null && y == null => 1,
                _ => 0
            };
        }

        private static int CompareDependency(TsClass x, TsClass y)
        {
            for (var parent = x.Type.BaseType; parent != null; parent = parent.BaseType)
            {
                if (y.Type == parent)
                {
                    return 1;
                }
            }

            for (var parent = y.Type.BaseType; parent != null; parent = parent.BaseType)
            {
                if (x.Type == parent)
                {
                    return -1;
                }
            }

            return 0;
        }
    }
}
