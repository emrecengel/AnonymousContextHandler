using System;
using System.Linq.Expressions;
using AnonymousContextHandler.ContextHandlers;
using AnonymousContextHandler.Repository;
using AnonymousContextHandler.Root;
using Microsoft.WindowsAzure.Storage.Table;

namespace AnonymousContextHandler
{
    internal class ContextHandler : IContextHandler
    {
        public IModelRepository<T> IoModelRepository<T>(Expression<Func<T, int>> idHolder, string folderPath,
            string uniqueIdentifier) where T : class
        {
            IIoContext<T> context = new IoContext<T>();
            context.SetIdParameter(idHolder);
            context.SetUniqueIdentifier(uniqueIdentifier);
            context.SetPath(folderPath);

            return new ModelRepository<T>(context);
        }

        public IModelRepository<T> DbModelRepository<T>(Expression<Func<T, int>> idHolder, string connectionStringName)
            where T : class
        {
            IDataBaseContext<T> context = new DataBaseContext<T>();
            context.SetConnectionString(connectionStringName);
            context.SetIdParameter(idHolder);
            return new ModelRepository<T>(context);
        }

        public IModelRepository<T> MemoryRepository<T>(Expression<Func<T, int>> idHolder) where T : class
        {
            IMemoryContext<T> context = new MemoryContext<T>();
            context.SetIdParameter(idHolder);
            return new ModelRepository<T>(context);
        }

        public IModelRepository<T> AzureTableRepository<T>(Expression<Func<T, int>> idHolder, string connectionString, string uniqueIdentifier) where T : TableEntity, new()
        {
            IAzureTableStrogeContext<T> context = new AzureTableStrogeContext<T>(); ;
            context.SetIdParameter(idHolder);
            context.SetUniqueIdentifier(uniqueIdentifier);
            context.SetConnectionString(connectionString);
            return new ModelRepository<T>(context);
        }

        /*  public IMemoryRepository<T> MemoryRepositoryWithDbInteraction<T>(Expression<Func<T, int>> idHolder, string connectionStringName) where T : class
          {
              IMemoryContext<T> context = new MemoryContext<T>();
              context.SetDbAccessReady(connectionStringName);
              context.SetIdParameter(idHolder);
              return new MemoryRepository<T>(context);
          }

          public IMemoryRepository<T> MemoryRepositoryWithIoInteraction<T>(Expression<Func<T, int>> idHolder, string folderPath,
              string uniqueIdentifier) where T : class
          {
              IMemoryContext<T> context = new MemoryContext<T>();
              context.SetPath(folderPath);
              context.SetUniqueIdentifier(uniqueIdentifier);
              context.SetIdParameter(idHolder);
              return new MemoryRepository<T>(context);
          }*/

        /* public IIoModelRepository<T> IoModelRepositoryWithDatabaseInteraction<T>(Expression<Func<T, int>> idHolder, string folderPath,
             string uniqueIdentifier, string connectionString) where T : class
         {
             IIoContext<T> context = new IoContext<T>();
             context.SetIdParameter(idHolder);
             context.SetUniqueIdentifier(uniqueIdentifier);
             context.SetPath(folderPath);
             context.SetDbAccessReady(connectionString);

             return new IoModelRepository<T>(context);
         }*/
    }
}