namespace SSystem
{
    public interface ISaveSystem
    {
        public void Save<T>(T data);
        public T Load<T>();
        public void Rewrite<T>(T data);
        public void Delete<T>();
    }
}

