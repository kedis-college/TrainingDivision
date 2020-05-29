using Microsoft.EntityFrameworkCore;
using TrainingDivisionKedis.DAL.Contracts;

namespace TrainingDivisionKedis.DAL.ApplicationDbContext
{
    public class AppDbContextFactory : IAppDbContextFactory
    {
        private readonly DbContextOptions<AppDbContext> _options;

        public AppDbContextFactory(
            DbContextOptions<AppDbContext> options)
        {
            _options = options;
        }

        public AppDbContext Create()
        {
            return new AppDbContext(_options);
        }

    }
}
