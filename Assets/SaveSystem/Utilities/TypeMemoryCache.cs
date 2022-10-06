using System.Collections.Generic;

public class TypeMemoryCache<Key, Type>
{
    private static Dictionary<Key, Type> _cache = new Dictionary<Key, Type>();
    
    public bool TryGet(Key key, out Type cacheObject)
    {
        if (_cache.TryGetValue(key, out cacheObject))
        {
            return true;
        }
        return false;
    }

    public Type Get(Key key)
    {
        return _cache[key];
    }

    public void Cache(Key key, Type cacheObject)
    {
        _cache.Add(key, cacheObject);
    }

    public void RemoveObjectCache(Key key)
    {
        _cache.Remove(key);
    }

    public void ClearCache()
    {
        _cache.Clear();
    }

    public int GetSize()
    {
        return _cache.Count;
    }
}
