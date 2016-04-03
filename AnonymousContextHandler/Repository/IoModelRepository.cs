using System;
using System.Linq;
using System.Linq.Expressions;

namespace AnonymousContextHandler.Implementation.Repository
{
    public class IoModelRepository<T> : ModelRepository<T>, IIoModelRepository<T> where T : class
    {
        private readonly IIoContext<T> _context;

        public IoModelRepository(IContext<T> context) : base(context)
        {
            _context = context as IIoContext<T>;
        }

        public void SaveAllRecordsSeperatelyToDatabase() => _context.SaveAllRecordsSeperatelyToDatabase();

        public void SaveAsJsonToAFieldRecordToDatabase(T model, Expression<Func<T, string>> fieldToSaveLogs) => _context.SaveAsJsonToAFieldRecordToDatabase(model, fieldToSaveLogs);
     
    }
}
