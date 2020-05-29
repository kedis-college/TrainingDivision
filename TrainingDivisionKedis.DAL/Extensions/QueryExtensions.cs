using Microsoft.EntityFrameworkCore;
using System;
using TrainingDivisionKedis.Core.Contracts.Queries;
using TrainingDivisionKedis.DAL.ApplicationDbContext;
using TrainingDivisionKedis.DAL.QueryDecorators;

namespace TrainingDivisionKedis.DAL.Extensions
{
    public static class QueryExtensions
    {
        public static Func<DbContext, IControlScheduleQuery> ControlScheduleQueryFactory = context => new ControlScheduleQueryDecorator(context);
        public static Func<DbContext, ICurriculumQuery> CurriculumQueryFactory = context => new CurriculumQueryDecorator(context);
        public static Func<AppDbContext, IMessageFileQuery> MessageFileQueryFactory = context => new MessageFileQueryDecorator(context);
        public static Func<DbContext, IMessagesQuery> MessagesQueryFactory = context => new MessagesQueryDecorator(context);
        public static Func<DbContext, IProgressInStudyQuery> ProgressInStudyQueryFactory = context => new ProgressInStudyQueryDecorator(context);
        public static Func<DbContext, IRaspredelenieQuery> RaspredelenieQueryFactory = context => new RaspredelenieQueryDecorator(context);
        public static Func<DbContext, IStudentsQuery> StudentsQueryFactory = context => new StudentsQueryDecorator(context);
        public static Func<DbContext, ITeachersQuery> TeachersQueryFactory = context => new TeachersQueryDecorator(context);
        public static Func<AppDbContext, ITestAnswersQuery> TestAnswersQueryFactory = context => new TestAnswersQueryDecorator(context);
        public static Func<AppDbContext, ITestQuestionsQuery> TestQuestionsQueryFactory = context => new TestQuestionsQueryDecorator(context);
        public static Func<AppDbContext, ITestsQuery> TestsQueryFactory = context => new TestsQueryDecorator(context);
        public static Func<AppDbContext, IUmkFilesQuery> UmkFilesQueryFactory = context => new UmkFilesQueryDecorator(context);
        public static Func<AppDbContext, ITestResultsQuery> TestResultsQueryFactory = context => new TestResultsQueryDecorator(context);
        public static Func<AppDbContext, ITestResultItemsQuery> TestResultItemsQueryFactory = context => new TestResultItemsQueryDecorator(context);

        public static IControlScheduleQuery ControlScheduleQuery(this DbContext context)
        {
            return ControlScheduleQueryFactory(context);
        }

        public static ICurriculumQuery CurriculumQuery(this DbContext context)
        {
            return CurriculumQueryFactory(context);
        }

        public static IMessageFileQuery MessageFileQuery(this AppDbContext context)
        {
            return MessageFileQueryFactory(context);
        }

        public static IMessagesQuery MessagesQuery(this DbContext context)
        {
            return MessagesQueryFactory(context);
        }

        public static IProgressInStudyQuery ProgressInStudyQuery(this DbContext context)
        {
            return ProgressInStudyQueryFactory(context);
        }

        public static IRaspredelenieQuery RaspredelenieQuery(this DbContext context)
        {
            return RaspredelenieQueryFactory(context);
        }

        public static IStudentsQuery StudentsQuery(this DbContext context)
        {
            return StudentsQueryFactory(context);
        }

        public static ITeachersQuery TeachersQuery(this DbContext context)
        {
            return TeachersQueryFactory(context);
        }

        public static ITestAnswersQuery TestAnswersQuery(this AppDbContext context)
        {
            return TestAnswersQueryFactory(context);
        }

        public static ITestQuestionsQuery TestQuestionsQuery(this AppDbContext context)
        {
            return TestQuestionsQueryFactory(context);
        }

        public static ITestsQuery TestsQuery(this AppDbContext context)
        {
            return TestsQueryFactory(context);
        }

        public static IUmkFilesQuery UmkFilesQuery(this AppDbContext context)
        {
            return UmkFilesQueryFactory(context);
        }

        public static ITestResultsQuery TestResultsQuery(this AppDbContext context)
        {
            return TestResultsQueryFactory(context);
        }

        public static ITestResultItemsQuery TestResultItemsQuery(this AppDbContext context)
        {
            return TestResultItemsQueryFactory(context);
        }

        public static void OnModelCreating(ModelBuilder modelBuilder)
        {
            ProgressInStudyQueryDecorator.OnModelCreating(modelBuilder);
            RaspredelenieQueryDecorator.OnModelCreating(modelBuilder);
            CurriculumQueryDecorator.OnModelCreating(modelBuilder);
            StudentsQueryDecorator.OnModelCreating(modelBuilder);
            TeachersQueryDecorator.OnModelCreating(modelBuilder);
            ControlScheduleQueryDecorator.OnModelCreating(modelBuilder);
            MessagesQueryDecorator.OnModelCreating(modelBuilder);
            TestsQueryDecorator.OnModelCreating(modelBuilder);
            TestQuestionsQueryDecorator.OnModelCreating(modelBuilder);
            TestResultsQueryDecorator.OnModelCreating(modelBuilder);
        }
    }
}
