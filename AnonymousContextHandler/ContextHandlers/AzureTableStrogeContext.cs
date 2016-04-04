using System;
using System.Collections.Generic;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using AnonymousContextHandler.Extensions;
using AnonymousContextHandler.Root;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace AnonymousContextHandler.ContextHandlers
{
    public class AzureTableStrogeContext<T> : IAzureTableStrogeContext<T> where T : TableEntity, new()
    {
        private readonly PluralizationService _pluralizationService = PluralizationService.CreateService(new CultureInfo("en-US"));
        private CloudTable _table;

        private string _uniqueIdentifier;

        public void SetIdParameter(Expression<Func<T, int>> idHolder) => IdHolder = idHolder;
        public void SetUniqueIdentifier(string uniqueIdentifier) => _uniqueIdentifier = uniqueIdentifier;

        public void SetConnectionString(string connectionString)
        {
            var storageAccount = CloudStorageAccount.Parse(connectionString);
            /* StorageCredentials creds = new StorageCredentials("devstoreaccount1", "Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==");
             var storageAccount = new CloudStorageAccount(creds, false);*/
            var tableClient = storageAccount.CreateCloudTableClient();
            //tableClient.Credentials = new StorageCredentials();
            _table = tableClient.GetTableReference(_pluralizationService.Pluralize(typeof (T).Name));
            _table.CreateIfNotExists();
        }

        public Expression<Func<T, int>> IdHolder { get; private set; }

        public T Add(T model)
        {
            var lastModel = List(query => query.Where(x => true))
                .ToList()
                .LastOrDefault();

            var id = lastModel == null
                ? 1
                : IdHolder.Compile()
                    .Invoke(lastModel) + 1;

            model.SetPropertyValue(IdHolder, id);

            _table.Execute(TableOperation.Insert(AzureTableByModel(model)));

            return model;
        }

        public T Update(T model)
        {
            var selectedModel = Find(IdHolder.Compile()
                .Invoke(model));

            if (selectedModel == null)
                return model;

            selectedModel = model;

            _table.Execute(TableOperation.Merge(AzureTableByModel(model)));
            return selectedModel;
        }

        public void Delete(int id)
        {
            var model = Find(id);

            if (model == null)
                return;

            _table.Execute(TableOperation.Delete(AzureTableByModel(model)));
        }

        public List<T> List(Func<IQueryable<T>, IQueryable<T>> query)
        {
            return query(_table.CreateQuery<T>()
                .Where(x => x.PartitionKey == _uniqueIdentifier))
                .ToList();
        }

        public void ReBind()
        {
            throw new NotImplementedException();
        }

        public T Find(int id)
        {
            return _table.CreateQuery<T>()
                .FirstOrDefault(x => x.PartitionKey == _uniqueIdentifier && x.RowKey == id.ToString());
        }

        public void Destroy()
        {
            _table = null;
        }

        private T AzureTableByModel(T model)
        {
            model.PartitionKey = _uniqueIdentifier;
            model.RowKey = string.Format(IdHolder.Compile()
                .Invoke(model)
                .ToString());
            return model;
        }
    }
}