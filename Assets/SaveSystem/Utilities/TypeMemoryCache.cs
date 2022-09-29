using System.Collections.Generic;

public class TypeMemoryCache<T>
{
    private Dictionary<object, T> _cache = new Dictionary<object, T>();
    
    public bool TryGet(object key, out T cacheObject)
    {
        if (_cache.TryGetValue(key, out cacheObject))
        {
            return true;
        }
        return false;
    }

    public T Get(object key)
    {
        return _cache[key];
    }

    public void Cache(object key, T cacheObject)
    {
        _cache.Add(key, cacheObject);
    }

    public void RemoveObjectCache(object key)
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
