using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TrainingDivisionKedis.BLL.Common;
using TrainingDivisionKedis.Controllers;
using TrainingDivisionKedis.Core.Contracts.Queries;
using TrainingDivisionKedis.Core.Models;
using TrainingDivisionKedis.DAL.ApplicationDbContext;
using TrainingDivisionKedis.DAL.Contracts;
using TrainingDivisionKedis.DAL.Extensions;

namespace TrainingDivisionKedis.Tests.ControllersIntegrationTests
{
    public class TestsControllerTests
    {
        TestsController _sut;
        readonly IMapper _mapper;

        private static List<TestAnswer> TestAnswers = new List<TestAnswer>
        {
            new TestAnswer { Id = 1, Text = "Answer text1", QuestionId = 1 },
            new TestAnswer { Id = 2, Text = "Answer text2", QuestionId = 1 },
            new TestAnswer { Id = 3, Text = "Answer text3", QuestionId = 2 },
            new TestAnswer { Id = 4, Text = "Answer text4", QuestionId = 2 },
            new TestAnswer { Id = 5, Text = "Answer text5", QuestionId = 3 },
            new TestAnswer { Id = 6, Text = "Answer text6", QuestionId = 3 },
            new TestAnswer { Id = 7, Text = "Answer text7", QuestionId = 4 },
            new TestAnswer { Id = 8, Text = "Answer text8", QuestionId = 4 },
            new TestAnswer { Id = 9, Text = "Answer text9", QuestionId = 5 },
            new TestAnswer { Id = 10, Text = "Answer text10", QuestionId = 5 },
            new TestAnswer { Id = 11, Text = "Answer text11", QuestionId = 6 },
            new TestAnswer { Id = 12, Text = "Answer text12", QuestionId = 6 }
        };

        private static List<TestQuestion> TestQuestions = new List<TestQuestion>
        {
            new TestQuestion { Id = 1, TestId = 1, Text = "Question text1", CorrectAnswerId = 1 },
            new TestQuestion { Id = 2, TestId = 1, Text = "Question text2", CorrectAnswerId = 4 },
            new TestQuestion { Id = 3, TestId = 1, Text = "Question text3", CorrectAnswerId = 6 },
            new TestQuestion { Id = 4, TestId = 2, Text = "Question text4", CorrectAnswerId = 7 },
            new TestQuestion { Id = 5, TestId = 2, Text = "Question text5", CorrectAnswerId = 9 },
            new TestQuestion { Id = 6, TestId = 2, Text = "Question text6", CorrectAnswerId = 12 }
        };

        private static List<Test> Tests = new List<Test>
        {
            new Test { Id = 1, Active = true, CreatedAt = new DateTime(2020,5,10), Draft = false, Name = "Test name 1", QuestionsPerTest = 2, QuestionsTotal = 3, SubjectId = 1, TermId = 1, UpdatedAt = new DateTime(2020,5,11), Visible = true },
            new Test { Id = 2, Active = true, CreatedAt = new DateTime(2020,5,11), Draft = true, Name = "Test name 2", QuestionsPerTest = 2, QuestionsTotal = 3, SubjectId = 1, TermId = 1, UpdatedAt = new DateTime(2020,5,12), Visible = true },
            new Test { Id = 3, Active = false, CreatedAt = new DateTime(2020,5,12), Draft = false, Name = "Test name 3", QuestionsPerTest = 2, QuestionsTotal = 3, SubjectId = 1, TermId = 1, UpdatedAt = new DateTime(2020,5,13), Visible = false },
            new Test { Id = 4, Active = true, CreatedAt = new DateTime(2020,5,13), Draft = false, Name = "Test name 4", QuestionsPerTest = 2, QuestionsTotal = 3, SubjectId = 1, TermId = 1, UpdatedAt = new DateTime(2020,5,14), Visible = false }
        };

        private static List<TestResult> TestResults = new List<TestResult>
        {
            new TestResult { Id = 1, Ball = 1, CreatedAt = new DateTime()  }
        };

        public TestsControllerTests()
        {
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfileBLL());
            });
            _mapper = mappingConfig.CreateMapper();
        }

        private static Mock<IAppDbContextFactory> SetupContextFactory(ITestsQuery testsQuery, ITestQuestionsQuery testQuestionsQuery, ITestAnswersQuery testAnswersQuery, ITestResultsQuery testResultsQuery, ITestResultItemsQuery testResultItemsQuery)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                // don't raise the error warning us that the in memory db doesn't support transactions
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;
            var dbContext = new AppDbContext(options);

            dbContext.Tests.AddRange(Tests);
            dbContext.TestQuestions.AddRange(TestQuestions);
            dbContext.TestAnswers.AddRange(TestAnswers);

            dbContext.SaveChanges();

            QueryExtensions.TestsQueryFactory = context => testsQuery;
            QueryExtensions.TestQuestionsQueryFactory = context => testQuestionsQuery;
            QueryExtensions.TestsQueryFactory = context => testsQuery;
            QueryExtensions.TestsQueryFactory = context => testsQuery;
            QueryExtensions.TestsQueryFactory = context => testsQuery;

            var mockDbContextFactory = new Mock<IAppDbContextFactory>();
            mockDbContextFactory.Setup(cf => cf.Create()).Returns(dbContext);
            return mockDbContextFactory;
        }

        public void Index_ShouldReturnViewWithModel()
        {

        }
    }
}
