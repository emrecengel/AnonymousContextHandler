using System;
using System.Linq;
using System.Linq.Expressions;
using AnonymousContextHandler.ContextHandlers;
using AnonymousContextHandler.Extensions;
using AnonymousContextHandler.Root;

namespace AnonymousContextHandler.Repository
{
    internal class ModelRepository<T> : IModelRepository<T> where T : class
    {
        private readonly IContext<T> _context;

        public ModelRepository(IContext<T> context)
        {
            _context = context;
        }

        public T Add(T model) => _context.Add(model);

        public T Update(T model) => _context.Update(model);
        public void Delete(int id) => _context.Delete(id);

        public IQueryable<T> List(Func<IQueryable<T>, IQueryable<T>> query) => _context.List(query)
            .AsQueryable();

        public T Find(int id) => _context.Find(id);
        public void Destroy() => _context.Destroy();

        public void SaveToDatabase(string connectionStringName)
        {
            if (!(_context is DataBaseContext<T>))
            {
                var dbContext = new DataBaseContext<T>();
                dbContext.SetConnectionString(connectionStringName);
                dbContext.SetIdParameter(_context.IdHolder);
                var list = List(query => query.Where(x => true))
                    .ToList();
                list.ForEach(x => dbContext.Add(x));
            }
        }

        public string SaveToFileAsJson(string url, Func<IQueryable<T>, IQueryable<T>> query)
        {
            if (!(_context is IoContext<T>))
            {
                var ioContext = new IoContext<T>();
                ioContext.SetIdParameter(_context.IdHolder);
                ioContext.SetUniqueIdentifier(Guid.NewGuid()
                    .ToString());
                ioContext.SetPath(url);

                var list = List(query)
                    .ToList();
                list.ForEach(x => ioContext.Add(x));
                return ioContext.FullPath;
            }

            return ((IoContext<T>) _context).FullPath;
        }

        public void SaveToDataBaseAsJsonToAField(T targetModel, string connectionStringName,
            Expression<Func<T, string>> fieldToSaveLogs)
        {
            if (!(_context is DataBaseContext<T>))
            {
                var dbContext = new DataBaseContext<T>();
                dbContext.SetConnectionString(connectionStringName);
                dbContext.SetIdParameter(_context.IdHolder);
                var jsonList = List(query => query.Where(x => true))
                    .ToList()
                    .ToJson();
                targetModel.SetPropertyValue(fieldToSaveLogs, jsonList);
                dbContext.Add(targetModel);
            }
        }

        public void SaveToDataBaseAsJsonToAField<TModel>(TModel targetModel, Expression<Func<TModel, int>> idHolder,
            string connectionStringName, Expression<Func<TModel, string>> fieldToSaveLogs) where TModel : class
        {
            var dbContext = new DataBaseContext<TModel>();
            dbContext.SetConnectionString(connectionStringName);
            dbContext.SetIdParameter(idHolder);
            var jsonList = List(query => query.Where(x => true))
                .ToList()
                .ToJson();
            targetModel.SetPropertyValue(fieldToSaveLogs, jsonList);
            dbContext.Add(targetModel);
        }
    }
}