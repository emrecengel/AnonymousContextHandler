namespace AnonymousContextHandler.Root
{
    internal interface IDataBaseContext<T> : IContext<T> where T : class
    {
        void SetConnectionString(string connectionString);
    }
}