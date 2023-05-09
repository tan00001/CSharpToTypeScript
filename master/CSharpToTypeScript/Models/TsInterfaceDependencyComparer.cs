namespace CSharpToTypeScript.Models
{
    public class TsInterfaceDependencyComparer : IComparer<TsInterface>
    {
        public int Compare(TsInterface? x, TsInterface? y)
        {
            return (x, y) switch
            {
                _ when x != null && y != null => CompareDependency(x, y),
                _ when x == null && y != null => -1,
                _ when x != null && y == null => 1,
                _ => 0
            };
        }

        private static Int32 CompareDependency(TsInterface x, TsInterface y)
        {
            if (CompareDependency(x.Interfaces, y))
            {
                return 1;
            }

            if (CompareDependency(y.Interfaces, x))
            {
                return -1;
            }

            return 0;
        }

        public static bool CompareDependency(IList<TsInterface> x, TsInterface y) => x.Any(i => i == y || CompareDependency(i.Interfaces, y));
    }
}
