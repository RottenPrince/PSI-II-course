using SharedModels.Lobby;
using System.Collections.Concurrent;

namespace BrainBoxAPI.Caching
{
    public class RoomContentCache : IDictionaryCache<int, RoomContentDTO>
    {
        private ConcurrentDictionary<int, RoomContentDTO> _cache;

        public RoomContentCache()
        {
            _cache = new ConcurrentDictionary<int, RoomContentDTO>();
        }

        public bool CheckIfExists(int key)
        {
            return _cache.ContainsKey(key);
        }

        public RoomContentDTO GetCached(int key)
        {
            return _cache.GetValueOrDefault(key);
        }

        public RoomContentDTO GetOrCompute(int key, Func<int, RoomContentDTO> compute)
        {
            return _cache.GetOrAdd(key, compute);
        }

        public bool Invalidate(int key)
        {
            return _cache.Remove(key, out var _);
        }

        public void InvalidateAll()
        {
            _cache.Clear();
        }
    }
}
