// https://stackoverflow.com/questions/43021/how-do-you-get-the-index-of-the-current-iteration-of-a-foreach-loop
[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public static class IEnumerableIndexExtension {
    public static IEnumerable<(int index, T item)> WithIndex<T>(this IEnumerable<T> self)       
       => self.Select((item, index) => (index, item));
}

