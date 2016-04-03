namespace AnonymousContextHandler.Root
{
    internal interface IMemoryContext<T> : IContext<T> where T : class
    {
        void SaveToDatabase();
        void ExportToTextFile();
        void SetPath(string path);
        void SetUniqueIdentifier(string uniqueIdentifier);
        void SetDbAccessReady(string connectionStringName);
    }
}