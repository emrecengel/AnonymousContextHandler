using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AnonymousContextHandler.Extensions;
using AnonymousContextHandler.Root;

namespace AnonymousContextHandler.ContextHandlers
{
    internal class MemoryContext<T> : IMemoryContext<T> where T : class
    {
        private IDataBaseContext<T> _idBaseContext;
        private IIoContext<T> _ioContext;
        private List<T> _list;

        public MemoryContext()
        {
            _list = new List<T>();
        }

        public Expression<Func<T, int>> IdHolder { get; private set; }

        public T Add(T model)
        {
            CheckIdHolderInitialize();

            var id = 1;
            var latestModel = List(query => query.Where(x => true)).ToList().LastOrDefault();
            if (latestModel != null)
                id = IdHolder.Compile()
                    .Invoke(latestModel) + 1;
            model.SetPropertyValue(IdHolder, id);
            _list.Add(model);
            return model;
        }

        public T Update(T model)
        {
            CheckIdHolderInitialize();
            var selectedModel = _list.Find(x => IdHolder.Compile()
                .Invoke(x) == IdHolder.Compile()
                    .Invoke(model));

            if (selectedModel == null)
                return model;

            selectedModel = model;
            return selectedModel;
        }

        public void Delete(int id)
        {
            CheckIdHolderInitialize();
            var model = Find(id);

            if (model == null)
                return;

            _list.Remove(model);
        }

        public List<T> List(Func<IQueryable<T>, IQueryable<T>> query)
        {
            CheckIdHolderInitialize();
            return query(_list.AsQueryable())
                .ToList();
        }

        public void ReBind()
        {
        }

        public T Find(int id) => List(query => query.Where(x => IdHolder.Compile()
            .Invoke(x) == id))
            .FirstOrDefault();

        public void SetIdParameter(Expression<Func<T, int>> idHolder) => IdHolder = idHolder;

        public void Destroy() => _list = null;

        public void SaveToDatabase() => _list.ForEach(x => _idBaseContext.Add(x));

        public void ExportToTextFile() => _list.ForEach(x => _ioContext.Add(x));

        public void SetPath(string path)
        {
            _ioContext = new IoContext<T>();
            _ioContext.SetPath(path);
        }

        public void SetUniqueIdentifier(string uniqueIdentifier)
        {
            if (_ioContext == null)
                _ioContext = new IoContext<T>();

            _ioContext.SetPath(uniqueIdentifier);
        }

        public void SetDbAccessReady(string connectionStringName)
        {
            _idBaseContext = new DataBaseContext<T>();
            _idBaseContext.SetConnectionString(connectionStringName);
        }

        private void CheckIdHolderInitialize()
        {
            if (IdHolder == null)
                throw new Exception("Please set id holder before using this class");
        }
    }
}