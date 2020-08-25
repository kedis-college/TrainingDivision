using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using TrainingDivisionKedis.BLL.Common;
using TrainingDivisionKedis.BLL.Contracts;
using TrainingDivisionKedis.BLL.DTO.Curriculum;
using TrainingDivisionKedis.Core.SPModels.Curriculum;
using TrainingDivisionKedis.DAL;
using TrainingDivisionKedis.DAL.Contracts;
using TrainingDivisionKedis.DAL.Extensions;

namespace TrainingDivisionKedis.BLL.Services
{
    public class CurriculumService : ICurriculumService
    {
        private readonly IAppDbContextFactory _contextFactory;

        public CurriculumService(IAppDbContextFactory contextFactory)
        {
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
        }

        public async Task<OperationDetails<string>> GetSubjectNameById(int id)
        {
            using (var context = _contextFactory.Create())
            {
                try
                {
                    var subject = await context.CurriculumQuery().GetNameById(id);
                    return OperationDetails<string>.Success(subject?.Name);
                }
                catch (Exception ex)
                {
                    return OperationDetails<string>.Failure(ex.Message, ex.Source);
                }
            }
        }

        public async Task<OperationDetails<List<SPSubjectsGetByStudent>>> GetSubjectsOfStudentAsync(GetSubjectsOfStudentRequest request)
        {
            using (var context = _contextFactory.Create())
            {
                try
                {
                    var subjects = await context.CurriculumQuery().GetByStudentAndTerm(request.UserId, request.TermId);
                    return OperationDetails<List<SPSubjectsGetByStudent>>.Success(subjects);
                }
                catch (Exception ex)
                {
                    return OperationDetails<List<SPSubjectsGetByStudent>>.Failure(ex.Message, ex.Source);
                }
            }
        }
    }
}
