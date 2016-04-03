using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace AnonymousContextHandler.Root
{
    internal interface IContext<T> where T : class
    {
        Expression<Func<T, int>> IdHolder { get; }
        T Add(T model);
        T Update(T model);
        void Delete(int id);
        List<T> List(Func<IQueryable<T>, IQueryable<T>> query);
        void ReBind();
        T Find(int id);
        void SetIdParameter(Expression<Func<T, int>> idHolder);
        void Destroy();
    }
}