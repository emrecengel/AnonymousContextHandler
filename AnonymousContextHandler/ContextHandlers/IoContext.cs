using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using AnonymousContextHandler.Extensions;
using AnonymousContextHandler.Repository;
using AnonymousContextHandler.Root;

namespace AnonymousContextHandler.ContextHandlers
{
    internal class IoContext<T> : IIoContext<T> where T : class
    {
        private List<T> _list;
        private IModelRepository<T> _modelRepository;
        private string _path;
        private string _uniqueIdentifier;

        public void SetIdParameter(Expression<Func<T, int>> idHolder) => IdHolder = idHolder;
        private void Checknitilization()
        {
            if (_list == null)
            {
                try
                {
                    _list = File.ReadAllText(PathByType())
                  .ParseJson<List<T>>() ?? new List<T>();
                }
                catch (Exception)
                {

                }


            }
        }

        public void Destroy()
        {
            if (File.Exists(PathByType()))
                File.Delete(PathByType());
        }

        public string FullPath => PathByType();

        public void SetPath(string path)
        {
            _path = path;
            Checknitilization();
        }

        public void SetDbAccessReady(string connectionStringName)
        {
            var databaseContext = new DataBaseContext<T>();
            databaseContext.SetConnectionString(connectionStringName);
            databaseContext.SetIdParameter(IdHolder);
            _modelRepository = new ModelRepository<T>(databaseContext);
        }

        public void SetUniqueIdentifier(string uniqueIdentifier) => _uniqueIdentifier = uniqueIdentifier;

        public Expression<Func<T, int>> IdHolder { get; private set; }

        public T Add(T model)
        {
            CheckIdHolderInitialize();

            var lastModel = List(query => query.Where(x => true)).ToList().LastOrDefault();
            var id = 0;
            if (lastModel == null)
            {
                id = 1;
            }
            else
            {
                id = IdHolder.Compile()
                    .Invoke(lastModel) + 1;
            }


            model.SetPropertyValue(IdHolder, id);
            _list.Add(model);


            Save();
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
            Save();
            return selectedModel;
        }

        public void Delete(int id)
        {
            CheckIdHolderInitialize();
            var model = Find(id);

            if (model == null)
                return;

            _list.Remove(model);
            Save();
        }

        public List<T> List(Func<IQueryable<T>, IQueryable<T>> query)
        {
            CheckIdHolderInitialize();
            Checknitilization();
            return query(_list.AsQueryable())
                .ToList();
        }

        public void ReBind() { }// _list = File.ReadAllText(PathByType())
                                // .ParseJson<List<T>>() ?? new List<T>();

        public T Find(int id) => List(query => query.Where(x => IdHolder.Compile()
            .Invoke(x) == id))
            .FirstOrDefault();

        public void SaveAllRecordsSeperatelyToDatabase()
        {
            if (_modelRepository == null)
                throw new Exception(
                    "Please make use IoModelRepositoryWithDatabaseInteraction and provide valid connectionStringName");

            _list.ForEach(record =>
            {
                record.SetPropertyValue(IdHolder, 0);
                _modelRepository.Add(record);
            });
        }

        public void SaveAsJsonToAFieldRecordToDatabase(T model, Expression<Func<T, string>> fieldToSaveLogs)
        {
            if (_modelRepository == null)
                throw new Exception(
                    "Please make use IoModelRepositoryWithDatabaseInteraction and provide valid connectionStringName");

            model.SetPropertyValue(fieldToSaveLogs, _list.ToJson());
            model.SetPropertyValue(IdHolder, 0);
            _modelRepository.Add(model);
        }

        private void Save()
        {
            try
            {
                File.WriteAllText(PathByType(), _list.ToJson());
            }
            catch (Exception) { }

        }

        private void CheckIdHolderInitialize()
        {
            if (IdHolder == null)
                throw new Exception("Please set id holder before using this class");
        }

        private string PathByType()
        {
            if (string.IsNullOrWhiteSpace(_path))
                throw new Exception("Please set path before using this context, it cannot be empty string");

            if (string.IsNullOrWhiteSpace(_uniqueIdentifier))
                throw new Exception("Please set uniqueIdentifier before using this context, it cannot be empty string");

            if (!Directory.Exists(_path))
                Directory.CreateDirectory(_path);

            var filePath = string.Format("{0}{1}-{2}.txt", _path, typeof(T).Name, _uniqueIdentifier);
            if (!File.Exists(filePath))
                File.Create(filePath).Close();
            return filePath;
        }
    }
}