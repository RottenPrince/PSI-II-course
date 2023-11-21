using System.Collections.Concurrent;

namespace BrainBoxAPI.Caching
{
    public interface IDictionaryCache<T, U>
    {
        public delegate U ComputeCachedValueDelegate(T key);
        public void InvalidateAll();
        public bool Invalidate(T key);
        public bool CheckIfExists(T key);
        public U? GetCached(T key);
        public U GetOrCompute(T key, Func<T, U> compute);
    }
}
