using System;
using System.Linq.Expressions;

namespace AnonymousContextHandler.Root
{
    internal interface IIoContext<T> : IContext<T> where T : class
    {
        string FullPath { get; }
        void SetPath(string path);
        void SetUniqueIdentifier(string uniqueIdentifier);
        void SaveAllRecordsSeperatelyToDatabase();
        void SaveAsJsonToAFieldRecordToDatabase(T model, Expression<Func<T, string>> fieldToSaveLogs);

        void SetDbAccessReady(string connectionStringName);
    }
}