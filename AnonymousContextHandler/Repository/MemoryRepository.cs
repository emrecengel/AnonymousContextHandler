namespace AnonymousContextHandler.Implementation.Repository
{
    public class MemoryRepository<T> : ModelRepository<T>, IMemoryRepository<T> where T : class
    {
        private readonly IMemoryContext<T> _context;

        public MemoryRepository(IContext<T> context) : base(context)
        {
            _context = context as IMemoryContext<T>;
        }

        public void SaveToDatabase() => _context.SaveToDatabase();

        public void ExportToTextFile() => _context.ExportToTextFile();
    }
}