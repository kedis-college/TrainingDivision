using Microsoft.EntityFrameworkCore;
using TrainingDivisionKedis.Core.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;
using TrainingDivisionKedis.DAL.Extensions;

namespace TrainingDivisionKedis.DAL.ApplicationDbContext
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        public virtual DbSet<Year> Years { get; set; }
        public virtual DbSet<Term> Terms { get; set; }
        public virtual DbSet<TermSeason> TermSeasons { get; set; }
        public virtual DbSet<Raspredelenie> Raspredelenie { get; set; }
        public virtual DbSet<Prize> Prizes { get; set; }
        public virtual DbSet<UmkFile> UmkFiles { get; set; }
        public virtual DbSet<DirectorName> DirectorNames { get; set; }
        public virtual DbSet<Message> Messages { get; set; }
        public virtual DbSet<FileType> FileTypes { get; set; }
        public virtual DbSet<MessageFile> MessageFiles { get; set; }
        public virtual DbSet<Test> Tests { get; set; }
        public virtual DbSet<TestQuestion> TestQuestions { get; set; }
        public virtual DbSet<TestAnswer> TestAnswers { get; set; }
        public virtual DbSet<TestResult> TestResults { get; set; }
        public virtual DbSet<TestResultItem> TestResultItems { get; set; }
        public virtual DbSet<Group> Groups { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            QueryExtensions.OnModelCreating(modelBuilder);     
        }

        #region DefineLoggerFactory
        public static readonly ILoggerFactory loggerFactory = new LoggerFactory(new[] {
              new DebugLoggerProvider()
        });
        #endregion

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLoggerFactory(loggerFactory)  //tie-up DbContext with LoggerFactory object
                .EnableSensitiveDataLogging();
        }
    }
}
