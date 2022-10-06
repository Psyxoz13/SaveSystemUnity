namespace SSystem
{
    public interface ISaveSystem
    {
        public void Save<T>(T data);
        public void Save(object data, System.Type type);
        public T Load<T>();
        public object Load(System.Type type);
        public void Rewrite<T>(T data);
        public void Delete<T>();
    }
}

