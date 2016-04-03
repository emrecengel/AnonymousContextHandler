using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using AnonymousContextHandler.Extensions;
using AnonymousContextHandler.Root;

namespace AnonymousContextHandler.ContextHandlers
{
    internal class DataBaseContext<T> : IDataBaseContext<T> where T : class
    {
        private EfContext<T> _context;

        public Expression<Func<T, int>> IdHolder { get; private set; }

        public T Add(T model)
        {
            _context.Entry(model)
                .State = EntityState.Added;
            var properties = GetAttributeAssignedProperties<ForeignKeyAttribute>(model.GetType())
                .Where(x => x != null);

            foreach (var propertyInfo in properties)
            {
                var selectedProperty = model.GetPropValue(propertyInfo.Name);
                if (selectedProperty == null)
                    continue;

                var navigationEntry = _context.Entry(selectedProperty);
                if (navigationEntry.State == EntityState.Added)
                    ((IObjectContextAdapter) _context).ObjectContext.Detach(selectedProperty);
            }

            SaveChanges();
            return model;
        }

        public T Update(T model)
        {
            var selectedModel = Get(IdHolder.Compile()
                .Invoke(model));

            if (selectedModel == null)
                throw new Exception(string.Format("Couldn't update {0} with id {1}", model.GetType()
                    .Name, IdHolder.Compile()
                        .Invoke(model)));

            var entry = _context.Entry(selectedModel);
            _context.Entry(selectedModel)
                .CurrentValues.SetValues(model);
            SaveChanges();
            entry.State = EntityState.Detached;
            return selectedModel;
        }

        public void Delete(int id)
        {
            var model = Get(id);

            if (model == null)
                throw new Exception(string.Format("Couldn't update {0} with id {1}", model.GetType()
                    .Name, id));

            _context.Set<T>()
                .Remove(model);

            var properties = GetAttributeAssignedProperties<ForeignKeyAttribute>(model.GetType())
                .Where(x => x != null);

            foreach (var propertyInfo in properties)
            {
                var selectedProperty = model.GetPropValue(propertyInfo.Name);
                if (selectedProperty == null)
                    continue;

                var navigationEntry = _context.Entry(selectedProperty);
                if (navigationEntry.State == EntityState.Deleted)
                    ((IObjectContextAdapter) _context).ObjectContext.Detach(selectedProperty);
            }

            SaveChanges();
        }

        public List<T> List(Func<IQueryable<T>, IQueryable<T>> query)
        {
            if (_context.Database.Connection.State != ConnectionState.Open)
                _context.Database.Connection.Open();

            return query(_context.Set<T>()
                .AsQueryable())
                .AsNoTracking()
                .ToList();
        }

        public void ReBind()
        {
        }

        public T Find(int id)
        {
            if (id == 0)
                return null;

            var selectedSet = _context.Set<T>()
                .AsQueryable();
            return selectedSet.AsNoTracking()
                .FirstOrDefault(x => IdHolder.Compile()
                    .Invoke(x) == id);
        }

        public void SetIdParameter(Expression<Func<T, int>> idHolder)
        {
            IdHolder = idHolder;
        }

        public void Destroy() => _context = null;

        public void SetConnectionString(string connectionString)
        {
            _context = new EfContext<T>(connectionString);
            _context.Configuration.AutoDetectChangesEnabled = false;
            _context.Configuration.LazyLoadingEnabled = false;
            _context.Configuration.ProxyCreationEnabled = false;
            _context.Configuration.ValidateOnSaveEnabled = false;
            _context.Configuration.UseDatabaseNullSemantics = true;
        }

        private T Get(int id) => _context.Set<T>()
            .Find(id);

        private void SaveChanges()
        {
            _context.SaveChanges();
        }

        private static IEnumerable<PropertyInfo> GetAttributeAssignedProperties<T>(Type selectedType)
            where T : Attribute
        {
            return selectedType.GetProperties()
                .Where(prop => prop.GetCustomAttributes(true)
                    .Any(customAttribute => customAttribute is T));
        }
    }
}