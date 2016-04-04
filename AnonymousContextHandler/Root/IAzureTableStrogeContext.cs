using Microsoft.WindowsAzure.Storage.Table;

namespace AnonymousContextHandler.Root
{
    internal interface IAzureTableStrogeContext<T>:IContext<T> where T : TableEntity, new()
    {
     
        void SetUniqueIdentifier(string uniqueIdentifier);
        void SetConnectionString(string connectionString);
    }
}