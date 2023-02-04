using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence
{
    public class AppDbContextInitialiser
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AppDbContextInitialiser> _logger;

        public AppDbContextInitialiser(
            AppDbContext context,
            ILogger<AppDbContextInitialiser> logger)
        {
            _logger = logger;
            _context = context;
        }

        public async Task InitialiseAsync()
        {
            try
            {
                await _context.Database.EnsureCreatedAsync();
                await _context.Database.MigrateAsync();
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "An error occurred while initialising the database.");
                throw;
            }
        }

        public Task SeedAsync()
        {
            try
            {
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }
        }
    }
}