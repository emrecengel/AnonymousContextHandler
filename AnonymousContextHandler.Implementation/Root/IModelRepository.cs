using System;
using System.Linq;
using System.Linq.Expressions;

namespace AnonymousContextHandler.Root
{
    public interface IModelRepository<T> where T : class
    {
        /// <summary>
        ///     Adds the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        T Add(T model);

        T Update(T model);
        void Delete(int id);
        IQueryable<T> List(Func<IQueryable<T>, IQueryable<T>> query);
        T Find(int id);
        void Destroy();

        void SaveToDatabase(string connectionStringName);
        string SaveToFileAsJson(string path, Func<IQueryable<T>, IQueryable<T>> query);

        void SaveToDataBaseAsJsonToAField(T targetModel, string connectionStringName,
            Expression<Func<T, string>> fieldToSaveLogs);

        void SaveToDataBaseAsJsonToAField<TModel>(TModel targetModel, Expression<Func<TModel, int>> idHolder,
            string connectionStringName, Expression<Func<TModel, string>> fieldToSaveLogs) where TModel : class;
    }
}