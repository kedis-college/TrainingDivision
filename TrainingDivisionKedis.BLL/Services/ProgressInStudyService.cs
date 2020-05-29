using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainingDivisionKedis.BLL.Common;
using TrainingDivisionKedis.BLL.Contracts;
using TrainingDivisionKedis.BLL.DTO.ProgressInStudy;
using TrainingDivisionKedis.Core.SPModels.ProgressInStudy;
using TrainingDivisionKedis.DAL.Contracts;
using AutoMapper;
using TrainingDivisionKedis.Core.SPModels.ControlSchedule;
using TrainingDivisionKedis.DAL.Extensions;

namespace TrainingDivisionKedis.BLL.Services
{
    public class ProgressInStudyService : IProgressInStudyService
    {
        private readonly IAppDbContextFactory _contextFactory;
        private readonly IMapper _mapper;

        public ProgressInStudyService(IAppDbContextFactory contextFactory, IMapper mapper)
        {
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<OperationDetails<ProgressInStudyListDto>> GetByRaspredelenieAndUserAsync(ProgressInStudyRequest request)
        {
            using (var context = _contextFactory.Create())
            {
                try
                {
                    var progressInStudy = await context.ProgressInStudyQuery().GetByRaspredelenieAndUser(request.RaspredelenieId,request.UserId);
                    var controlSchedule = await context.ControlScheduleQuery().GetEditable(request.RaspredelenieId);
                    
                    var progressInStudyResponse = new ProgressInStudyListDto(progressInStudy, controlSchedule);

                    return OperationDetails<ProgressInStudyListDto>.Success(progressInStudyResponse);
                }
                catch (Exception ex)
                {
                    return OperationDetails<ProgressInStudyListDto>.Failure(ex.Message, ex.Source);
                }
            }
        }

        public async Task<OperationDetails<List<SPProgressInStudyGetByRaspredelenieAndUser>>> GetByRaspredelenieAsync(int id)
        {
            using (var context = _contextFactory.Create())
            {
                try
                {
                    var progressInStudy = await context.ProgressInStudyQuery().GetByRaspredelenie(id);
                    return OperationDetails<List<SPProgressInStudyGetByRaspredelenieAndUser>>.Success(progressInStudy);
                }
                catch (Exception ex)
                {
                    return OperationDetails<List<SPProgressInStudyGetByRaspredelenieAndUser>>.Failure(ex.Message, ex.Source);
                }
            }
        }

        public async Task<OperationDetails<List<ProgressInStudyOfStudent>>> GetByStudentAsync(int id)
        {
            using (var context = _contextFactory.Create())
            {
                try
                {
                    var progressInStudy = new List<SPProgressInStudyGetByStudent>();
                    var progressInStudyDto = new List<ProgressInStudyOfStudent>();
                    for (byte i = 1; i < 7; i++)
                    {
                        progressInStudy = await context.ProgressInStudyQuery().GetByStudentAndTerm(id,i);
                        progressInStudyDto.Add(new ProgressInStudyOfStudent(i,progressInStudy));
                    }
                    return OperationDetails<List<ProgressInStudyOfStudent>>.Success(progressInStudyDto);
                }
                catch (Exception ex)
                {
                    return OperationDetails<List<ProgressInStudyOfStudent>>.Failure(ex.Message, ex.Source);
                }
            }
        }

        public async Task<OperationDetails<List<TotalsDto>>> GetTotalsByRaspredelenieAsync(int id)
        {
            using (var context = _contextFactory.Create())
            {
                try
                {
                    var progressInStudy = await context.ProgressInStudyQuery().GetTotalsByRaspredelenie(id);
                    var totalsDto = _mapper.Map<List<TotalsDto>>(progressInStudy);
                    return OperationDetails<List<TotalsDto>>.Success(totalsDto);
                }
                catch (Exception ex)
                {
                    return OperationDetails<List<TotalsDto>>.Failure(ex.Message, ex.Source);
                }
            }
        }

        public async Task<OperationDetails<SPProgressInStudyGetByRaspredelenieAndUser>> UpdateAsync(ProgressInStudyUpdateRequest request)
        {
            using (var context = _contextFactory.Create())
            {
                try
                {
                    var progressInStudy = await context.ProgressInStudyQuery()
                        .Update(request.NumRec, request.Mod1, request.Mod2, request.Dop, request.Itog, request.UserId, request.Date);
                    return OperationDetails<SPProgressInStudyGetByRaspredelenieAndUser>.Success(progressInStudy);
                }
                catch (Exception ex)
                {
                    return OperationDetails<SPProgressInStudyGetByRaspredelenieAndUser>.Failure(ex.Message, ex.Source);
                }
            }
        }
       
        public async Task<OperationDetails<SPProgressInStudyGetTotalsByStudent>> GetTotalsOfStudentAsync(int id)
        {
            using (var context = _contextFactory.Create())
            {
                try
                {
                    var totals = await context.ProgressInStudyQuery().GetTotalsByStudent(id);
                    return OperationDetails<SPProgressInStudyGetTotalsByStudent>.Success(totals);
                }
                catch (Exception ex)
                {
                    return OperationDetails<SPProgressInStudyGetTotalsByStudent>.Failure(ex.Message, ex.Source);
                }
            }
        }
    }
}
