using System;
using System.Linq.Expressions;

namespace AnonymousContextHandler.Root
{
    internal interface IContextHandler
    {
        IModelRepository<T> IoModelRepository<T>(Expression<Func<T, int>> idHolder, string folderPath,
            string uniqueIdentifier) where T : class;

        /// <summary>
        ///     Databases the model repository.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="idHolder">The identifier holder.</param>
        /// <param name="connectionStringName">Name of the connection string.</param>
        /// <returns></returns>
        IModelRepository<T> DbModelRepository<T>(Expression<Func<T, int>> idHolder, string connectionStringName)
            where T : class;

        /* IIoModelRepository<T> IoModelRepositoryWithDatabaseInteraction<T>(Expression<Func<T, int>> idHolder, string folderPath,
            string uniqueIdentifier, string connectionString) where T : class;*/

        IModelRepository<T> MemoryRepository<T>(Expression<Func<T, int>> idHolder) where T : class;
    }
}