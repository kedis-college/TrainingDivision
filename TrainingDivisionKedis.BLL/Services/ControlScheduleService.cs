using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrainingDivisionKedis.BLL.Common;
using TrainingDivisionKedis.BLL.Contracts;
using TrainingDivisionKedis.BLL.DTO.ControlSchedule;
using TrainingDivisionKedis.Core.Models;
using TrainingDivisionKedis.DAL.Contracts;
using AutoMapper;
using System.Data.SqlClient;
using TrainingDivisionKedis.DAL.Extensions;

namespace TrainingDivisionKedis.BLL.Services
{
    public class ControlScheduleService : IControlScheduleService
    {
        private readonly IAppDbContextFactory _contextFactory;
        private readonly IMapper _mapper;

        public ControlScheduleService(IAppDbContextFactory contextFactory, IMapper mapper)
        {
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<OperationDetails<ControlScheduleDto>> CreateAsync(ControlScheduleDto request)
        {
            using (var context = _contextFactory.Create())
            {
                try
                {
                    var controlSchedule = await context.ControlScheduleQuery().Create(request.YearId, request.SeasonId,
                        request.UserId, request.DateStart.Value, request.DateEnd, request.Mod1DateStart, request.Mod1DateEnd,
                        request.Mod2DateStart, request.Mod2DateEnd, request.ItogDateStart, request.ItogDateEnd);
                    if (controlSchedule == null)
                        throw new Exception("Ошибка при добавлении записи");
                    request = _mapper.Map<ControlScheduleDto>(controlSchedule);
                    return OperationDetails<ControlScheduleDto>.Success(request);
                }
                catch (Exception ex)
                {
                    if(ex.InnerException != null)
                        return OperationDetails<ControlScheduleDto>.Failure(ex.InnerException.Message, ex.Source);
                    else
                        return OperationDetails<ControlScheduleDto>.Failure(ex.Message, ex.Source);
                }
            }
        }

        public async Task<OperationDetails<List<ControlScheduleListDto>>> GetByYearAndSeasonAsync(GetByYearAndSeasonRequest request)
        {
            using (var context = _contextFactory.Create())
            {
                try
                {
                    var result = await context.ControlScheduleQuery().GetByYearAndSeason(request.YearId, request.SeasonId);
                    var dto = _mapper.Map<List<ControlScheduleListDto>>(result);
                    return OperationDetails<List<ControlScheduleListDto>>.Success(dto);
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null)
                        return OperationDetails<List<ControlScheduleListDto>>.Failure(ex.InnerException.Message, ex.Source);
                    else
                        return OperationDetails<List<ControlScheduleListDto>>.Failure(ex.Message, ex.Source);
                }
            }
        }

        public async Task<OperationDetails<ControlScheduleDto>> GetByIdAsync(int id)
        {
            using (var context = _contextFactory.Create())
            {
                try
                {
                    var result = await context.ControlScheduleQuery().GetById(id);
                    if (result != null)
                    {
                        result.Year = context.Years.Find(result.YearId);
                        result.Season = context.TermSeasons.Find(result.SeasonId);
                        var dto = _mapper.Map<ControlScheduleDto>(result);
                        return OperationDetails<ControlScheduleDto>.Success(dto);
                    }
                    else
                        throw new Exception("Запись не найдена");
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null)
                        return OperationDetails<ControlScheduleDto>.Failure(ex.InnerException.Message, ex.Source);
                    else
                        return OperationDetails<ControlScheduleDto>.Failure(ex.Message, ex.Source);
                }
            }
        }

        public async Task<OperationDetails<ControlScheduleDto>> UpdateAsync(ControlScheduleDto request)
        {
            using (var context = _contextFactory.Create())
            {
                try
                {
                    await context.ControlScheduleQuery().Update(request.Id, request.UserId, request.DateStart.Value, request.DateEnd, request.Mod1DateStart, request.Mod1DateEnd,
                        request.Mod2DateStart, request.Mod2DateEnd, request.ItogDateStart, request.ItogDateEnd);
                    request.Year = context.Years.Find(request.YearId);
                    request.Season = context.TermSeasons.Find(request.SeasonId);
                    return OperationDetails<ControlScheduleDto>.Success(request);
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null)
                        return OperationDetails<ControlScheduleDto>.Failure(ex.InnerException.Message, ex.Source);
                    else
                        return OperationDetails<ControlScheduleDto>.Failure(ex.Message, ex.Source);
                }
            }
        }

    }
}
