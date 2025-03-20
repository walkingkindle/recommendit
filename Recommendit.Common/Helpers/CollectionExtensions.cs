namespace Recommendit.Helpers
{
    public static class CollectionExtensions
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
        {
          if(enumerable == null)
            return true;

            return !enumerable.Any();
        }

    public static double[] ConvertStringVectorToDoubleArray(string vector)
    {
        return vector
            .Trim('[', ']')
            .Split(',')
            .Select(s => double.Parse(s.Trim()))
            .ToArray();
    }
    }
}
