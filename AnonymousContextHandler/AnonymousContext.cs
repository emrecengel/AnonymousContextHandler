using System;
using System.Linq.Expressions;
using AnonymousContextHandler.Root;
using Microsoft.WindowsAzure.Storage.Table;

namespace AnonymousContextHandler
{
    public class AnonymousContext
    {
        private static readonly ContextHandler ContextHandler;

        static AnonymousContext()
        {
            ContextHandler = new ContextHandler();
        }

        /// <summary>
        ///     IO model repository, this is a realtime writer, as soon as an operation takes place, will be written to the file.
        /// </summary>
        /// <example>
        ///     The following example shows an implementation this.
        ///     <code lang="cs"><![CDATA[
        /// contextHandler.IoModelRepository<T>(x => x.Id, "C:/Temp/IOContextTest/", Guid.NewGuid().ToString());
        /// ]]></code>
        /// </example>
        /// <typeparam name="T"></typeparam>
        /// <param name="idHolder">
        ///     Specifis the variable that is going to be used as the id
        /// </param>
        /// <param name="folderPath">The folder path.</param>
        /// <param name="uniqueIdentifier">The unique identifier.</param>
        /// <returns>
        ///     <see cref="IModelRepository{T}" />
        /// </returns>
        public static IModelRepository<T> IoModelRepository<T>(Expression<Func<T, int>> idHolder, string folderPath,
            string uniqueIdentifier) where T : class
            => ContextHandler.IoModelRepository(idHolder, folderPath, uniqueIdentifier);

        public static IModelRepository<T> EfModelRepository<T>(Expression<Func<T, int>> idHolder,
            string connectionStringName) where T : class
            => ContextHandler.DbModelRepository(idHolder, connectionStringName);

        public static IModelRepository<T> MemoryRepository<T>(Expression<Func<T, int>> idHolder) where T : class
            => ContextHandler.MemoryRepository(idHolder);

        public static IModelRepository<T> AzureTableRepository<T>(Expression<Func<T, int>> idHolder, string connectionString, string uniqueIdentifier) where T : TableEntity, new()
            => ContextHandler.AzureTableRepository(idHolder, connectionString, uniqueIdentifier);
    }
}