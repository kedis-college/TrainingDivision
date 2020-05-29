using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainingDivisionKedis.BLL.Common;
using TrainingDivisionKedis.BLL.Contracts;
using TrainingDivisionKedis.BLL.DTO.TestsClient;
using TrainingDivisionKedis.BLL.DTO.UmkFiles;
using TrainingDivisionKedis.Core.Models;
using TrainingDivisionKedis.DAL.ApplicationDbContext;
using TrainingDivisionKedis.DAL.Contracts;
using TrainingDivisionKedis.DAL.Extensions;

namespace TrainingDivisionKedis.BLL.Services
{
    public class TestClientService : ITestClientService
    {
        private readonly IAppDbContextFactory _contextFactory;
        private readonly IMapper _mapper;

        public TestClientService(IAppDbContextFactory contextFactory, IMapper mapper)
        {
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<OperationDetails<TestDto>> GetByIdAsync(int id, int userId)
        {
            using (var context = _contextFactory.Create())
            {
                try
                {                    
                    // Get test
                    var test = await context.Tests.Where(t=>t.Id == id && t.Active && !t.Draft && t.Visible).FirstOrDefaultAsync();
                    if(test == null)
                        throw new Exception("Тест с id: " + id + "не найден");
                    // Check if already passed test
                    var availableTests = await context.TestsQuery().GetByStudentAndSubjectAndTerm(userId, test.SubjectId, test.TermId);
                    var testToPass = availableTests.Find(t=>t.Id == id);
                    if(testToPass == null || testToPass.FinishedAt.HasValue)
                        throw new Exception("Тест недоступен для прохождения");
                    if(!testToPass.IsOpened)
                        throw new Exception("Время для прохождения теста вышло!");

                    var testDto = _mapper.Map<TestDto>(test);   
                    // Check if already started
                    if(testToPass.TestResultId.HasValue)
                    {          
                        //Started
                        testDto.TestResultId = testToPass.TestResultId.Value;
                        testDto.StartedAt = testToPass.StartedAt;
                    }
                    else
                    {       
                        //Create new TestResult
                        var newTestResult = await CreateTestResult(context, id, userId);
                        testDto.TestResultId = newTestResult.Id;
                    }
                    // Get questions
                    testDto.Questions = await context.TestQuestionsQuery().GetByTestResultId(testDto.TestResultId);
                    foreach (var question in testDto.Questions)
                    {
                        // Get answers
                        question.Answers = await context.TestAnswers.Where(a => a.QuestionId == question.Id).ToListAsync();
                    }
                    testDto.QuestionsTotal = test.QuestionsPerTest;
                    return OperationDetails<TestDto>.Success(testDto);
                }
                catch (Exception ex)
                {
                    return OperationDetails<TestDto>.Failure(ex.Message, ex.Source);
                }
            }
        }

        public async Task<OperationDetails<TestListDto>> GetBySubjectAndTermAsync(GetBySubjectAndTermRequest request)
        {
            using (var context = _contextFactory.Create())
            {
                try
                {
                    var tests = await context.TestsQuery().GetByStudentAndSubjectAndTerm(request.UserId, request.SubjectId, request.TermId);
                    var subjectName = await context.CurriculumQuery().GetNameById(request.SubjectId);
                    var dto = new TestListDto { Tests = tests, SubjectName = subjectName.Name };
                    return OperationDetails<TestListDto>.Success(dto);
                }
                catch (Exception ex)
                {
                    return OperationDetails<TestListDto>.Failure(ex.Message, ex.Source);
                }
            }
        }

        private async Task<TestResult> CreateTestResult(AppDbContext context, int testId, int studentId)
        {
            var createdTestResult = await context.TestResultsQuery().Create(testId, studentId);
            if (createdTestResult == null)
                throw new Exception("Ошибка при добавлении ответа теста");
            var questions = await context.TestQuestionsQuery().GetRandomSet(testId);
            foreach (var question in questions)
            {
                var createdItem = await context.TestResultItemsQuery().Create(createdTestResult.Id, question.Id, null);
                if (createdItem == null)
                    throw new Exception("Ошибка при добавлении ответа теста");
                createdTestResult.TestResultItems.Add(createdItem);
            }
            return createdTestResult;
        }

        public async Task<OperationDetails<bool>> Start(TestStartRequest request)
        {
            using (var context = _contextFactory.Create())
            {
                try
                {
                    var affectedRows = await context.TestResultsQuery().Start(request.TestResultId, request.StartedAt);
                    if (affectedRows == 0)
                        throw new Exception("Ошибка при выполнении запроса");
                    return OperationDetails<bool>.Success(true);
                }
                catch (Exception ex)
                {
                    return OperationDetails<bool>.Failure(ex.Message, ex.Source);
                }
            }
        }

        public async Task<OperationDetails<bool>> CreateResultItem(TestResultItem testResultItem)
        {
            using (var context = _contextFactory.Create())
            {
                try
                {
                    // Get testResult
                    var testResult = await context.TestResults.Where(t => t.Id == testResultItem.TestResultId).Include(t=>t.Test).FirstOrDefaultAsync();
                    if (testResult == null)
                        throw new Exception("Результат теста с id: " + testResultItem.TestResultId + "не найден");
                    if (!testResult.IsOpened)
                        throw new Exception("Время для прохождения теста вышло!");
                    testResultItem = await context.TestResultItemsQuery().Create(testResult.Id, testResultItem.TestQuestionId, testResultItem.AnswerId);
                    if (testResultItem == null)
                        throw new Exception("Ошибка при добавлении ответа теста");
                    return OperationDetails<bool>.Success(true);
                }
                catch (Exception ex)
                {
                    return OperationDetails<bool>.Failure(ex.Message, ex.Source);
                }
            }
        }

        public async Task<OperationDetails<TestResult>> Finish(int id)
        {
            using (var context = _contextFactory.Create())
            {
                try
                {
                    var affectedRows = await context.TestResultsQuery().Finish(id,DateTime.Now);
                    if (affectedRows == 0)
                        throw new Exception("Ошибка при выполнении запроса");
                    var testResult = await context.TestResults.Include(t=>t.Test).Where(t => t.Id == id).FirstOrDefaultAsync();
                    return OperationDetails<TestResult>.Success(testResult);
                }
                catch (Exception ex)
                {
                    return OperationDetails<TestResult>.Failure(ex.Message, ex.Source);
                }
            }
        }
    }
}
