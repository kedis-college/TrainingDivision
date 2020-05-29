using TrainingDivisionKedis.DAL.ApplicationDbContext;

namespace TrainingDivisionKedis.DAL.Contracts
{
    public interface IAppDbContextFactory
    {
        AppDbContext Create();
    }
}
