namespace Infrastructure.Persistence
{
    public class UnitOfWork
    {
        private readonly AppDbContext _dbContext;

        public UnitOfWork(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> SaveChange()
        {
            var rowEffected = 0;
            try
            {
                rowEffected = await _dbContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                Dispose();
                throw;
            }

            return rowEffected;
        }

        // Release memory if saveChange() has error
        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}