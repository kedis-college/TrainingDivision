using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainingDivisionKedis.BLL.Common;
using TrainingDivisionKedis.BLL.Contracts;
using TrainingDivisionKedis.BLL.DTO.TestsAdmin;
using TrainingDivisionKedis.BLL.DTO.UmkFiles;
using TrainingDivisionKedis.Core.Models;
using TrainingDivisionKedis.DAL;
using TrainingDivisionKedis.DAL.ApplicationDbContext;
using TrainingDivisionKedis.DAL.Contracts;
using TrainingDivisionKedis.DAL.Extensions;

namespace TrainingDivisionKedis.BLL.Services
{
    public class TestAdminService : ITestAdminService
    {
        private readonly IAppDbContextFactory _contextFactory;
        private readonly IMapper _mapper;

        public TestAdminService(IAppDbContextFactory contextFactory, IMapper mapper)
        {
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<OperationDetails<bool>> CreateAsync(TestCreateDto request)
        {
            using (var context = _contextFactory.Create())
            {
                var transaction = context.Database.BeginTransaction();
                try
                {
                    var checkUserRight = await context.RaspredelenieQuery().CheckTeacherBySubjectAndTerm((short)request.UserId, request.SubjectId, request.TermId);
                    if (checkUserRight == null || checkUserRight.Result == false)
                    {
                        throw new Exception("Отказ в доступе");
                    }
                    await CreateAsync(context, request);
                    transaction.Commit();
                    transaction.Dispose();
                    return OperationDetails<bool>.Success(true);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return OperationDetails<bool>.Failure(ex.Message, "");
                }
            }
        }

        private async Task<Test> CreateAsync(AppDbContext context, TestCreateDto request)
        {
            if (request.QuestionsPerTest < 1 || request.QuestionsPerTest > request.Questions.Count)
                throw new Exception("Количество вопросов на тест должно быть меньше или равно общему количеству вопросов и больше 0");
            // Create test
            var createdTest = await context.TestsQuery().Create(request.Name, request.SubjectId, request.TermId, request.QuestionsPerTest, request.TimeLimit);
            var questionIndex = 1;
            foreach (var question in request.Questions)
            {
                if (question.CorrectAnswerId == null)
                    throw new Exception("Выберите правильный ответ на вопрос " + questionIndex);
                // Create questions
                var createdQuestion = await context.TestQuestionsQuery().Create(question.Text, createdTest.Id);
                var answerIndex = 0;
                foreach (var answer in question.Answers)
                {
                    // Create answers
                    await context.TestAnswersQuery().Create(answer.Text, createdQuestion.Id, question.CorrectAnswerId == answerIndex);
                    answerIndex++;
                }
                questionIndex++;
            }
            return createdTest;
        }

        public async Task<OperationDetails<bool>> DeleteAsync(int id)
        {
            using (var context = _contextFactory.Create())
            {
                try
                {
                    var testResults = context.TestResults.Where(t => t.TestId == id).Count();
                    var affectedRows = 0;
                    if (testResults > 0)
                        affectedRows = await context.TestsQuery().UpdateState(id, null, null, false);
                    else
                        affectedRows = await context.TestsQuery().Delete(id);                       
                    if (affectedRows == 0)
                        throw new Exception("Ошибка при удалении записи");
                    return OperationDetails<bool>.Success(true);
                }
                catch (Exception ex)
                {
                    return OperationDetails<bool>.Failure(ex.Message, ex.Source);
                }
            }
        }

        public async Task<OperationDetails<Test>> GetByIdAsync(int id)
        {
            using (var context = _contextFactory.Create())
            {
                try
                {
                    // Get test
                    var test = await context.Tests.FindAsync(id);
                    if (test == null)
                        throw new Exception("Тест с id: " + id + "не найден");
                    // Get questions
                    test.Questions = await context.TestQuestions.Where(q => q.TestId == test.Id).ToListAsync();
                    foreach (var question in test.Questions)
                    {
                        // Get answers
                        question.Answers = await context.TestAnswers.Where(a => a.QuestionId == question.Id).ToListAsync();
                    }
                    return OperationDetails<Test>.Success(test);
                }
                catch (Exception ex)
                {
                    return OperationDetails<Test>.Failure(ex.Message, "");
                }

            }
        }

        public async Task<OperationDetails<TestListDto>> GetBySubjectAndTermAsync(GetBySubjectAndTermRequest request)
        {
            using (var context = _contextFactory.Create())
            {
                try
                {
                    var checkUserRight = await context.RaspredelenieQuery().CheckTeacherBySubjectAndTerm((short)request.UserId, request.SubjectId, request.TermId);
                    if (checkUserRight == null || checkUserRight.Result == false)
                    {
                        throw new Exception("Отказ в доступе");
                    }
                    var tests = await context.Tests.Where(t => t.SubjectId == request.SubjectId && t.TermId == request.TermId && t.Active).ToListAsync();
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

        public async Task<OperationDetails<bool>> UpdateVisibilityAsync(TestStateDto request)
        {
            using (var context = _contextFactory.Create())
            {
                try
                {
                    var affectedRows = await context.TestsQuery().UpdateState(request.Id, request.Visible, request.Draft, request.Active);
                    if (affectedRows == 0)
                        throw new Exception("Ошибка при обновлении записи");
                    else
                        return OperationDetails<bool>.Success(true);
                }
                catch (Exception ex)
                {
                    return OperationDetails<bool>.Failure(ex.Message, ex.Source);
                }
            }
        }

        public async Task<OperationDetails<List<TestResult>>> GetResultsBySubjectAndTermAndStudentAsync(GetBySubjectAndTermRequest request)
        {
            using (var context = _contextFactory.Create())
            {
                try
                {
                    var testResults = await context.TestResults.Include(t => t.Test).Where(t => t.StudentId == request.UserId && t.Test.Active && !t.Test.Draft && t.Test.SubjectId == request.SubjectId && t.Test.TermId == request.TermId).ToListAsync();
                    return OperationDetails<List<TestResult>>.Success(testResults);
                }
                catch (Exception ex)
                {
                    return OperationDetails<List<TestResult>>.Failure(ex.Message, ex.Source);
                }
            }
        }

        public async Task<OperationDetails<TestResultsListDto>> GetResultsByTestIdAsync(int id)
        {
            using (var context = _contextFactory.Create())
            {
                try
                {
                    var testResultsListDto = new TestResultsListDto
                    {
                        Test = await context.Tests.FindAsync(id)
                    };

                    var groups = await context.TestResultsQuery().GetGroups(id);
                    foreach(var group in groups)
                    {
                        var spTestResults = await context.TestResultsQuery().GetWithStudents(id, group.Id);                       
                        testResultsListDto.Groups.Add(new TestGroup { GroupName = group.Name, TestResults = spTestResults});
                    }
                    return OperationDetails<TestResultsListDto>.Success(testResultsListDto);
                }
                catch (Exception ex)
                {
                    return OperationDetails<TestResultsListDto>.Failure(ex.Message, ex.Source);
                }
            }
        }

        public async Task<OperationDetails<TestResultDto>> GetResultByIdAsync(int id)
        {
            using (var context = _contextFactory.Create())
            {
                try
                {
                    var testResult = await context.TestResults.FindAsync(id);
                    if (testResult == null)
                        throw new Exception("Результат теста с id: " + id + " не найден");
                    testResult.TestResultItems = await context.TestResultItems.Where(t=>t.TestResultId == id).Include(t => t.TestQuestion).ThenInclude(q=>q.Answers).ToListAsync();
                    var student = await context.StudentsQuery().GetById(testResult.StudentId);
                    if (student == null)
                        throw new Exception("Студент с id: " + testResult.StudentId + " не найден");
                    var testResultDto = new TestResultDto { TestResult = testResult, Student = student };
                    return OperationDetails<TestResultDto>.Success(testResultDto);
                }
                catch (Exception ex)
                {
                    return OperationDetails<TestResultDto>.Failure(ex.Message, ex.Source);
                }
            }
        }

        public async Task<OperationDetails<bool>> UpdateAsync(Test test)
        {
            using (var context = _contextFactory.Create())
            {
                var transaction = context.Database.BeginTransaction();
                try
                {
                    var oldTest = await context.Tests.FindAsync(test.Id);
                    if (oldTest == null)
                        throw new Exception("Тест с id: " + test.Id + "не найден");
                    if(!oldTest.Draft)
                        throw new Exception("Тест опубликован и не может быть редактирован");
                    var affectedRows = await context.TestsQuery().Delete(test.Id);
                    if (affectedRows == 0)
                        throw new Exception("Ошибка при обновлении записи");
                    var createDto = _mapper.Map<TestCreateDto>(test);
                    await CreateAsync(context, createDto);                   
                    transaction.Commit();
                    transaction.Dispose();
                    return OperationDetails<bool>.Success(true);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    transaction.Dispose();
                    return OperationDetails<bool>.Failure(ex.Message, ex.Source);
                }
            }
        }
    }
}
