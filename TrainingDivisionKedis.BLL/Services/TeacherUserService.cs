using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TrainingDivisionKedis.BLL.DTO.User;
using TrainingDivisionKedis.BLL.Common;
using TrainingDivisionKedis.DAL.Contracts;
using TrainingDivisionKedis.Core.SPModels.User;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;
using System.Linq;
using TrainingDivisionKedis.BLL.Contracts;
using TrainingDivisionKedis.DAL.Extensions;

namespace TrainingDivisionKedis.BLL.Services
{
    public class TeacherUserService : IUserService
    {
        private readonly IAppDbContextFactory _contextFactory;

        public TeacherUserService(IAppDbContextFactory contextFactory)
        {
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
        }

        public async Task<OperationDetails<UserDto>> AuthenticateAsync(UserDto userDto)
        {
            using (var context = _contextFactory.Create())
            {
                try
                {
                    var results = await context.TeachersQuery().Authenticate(userDto.Login, userDto.Password);
                    if (results.Count > 0)
                    {
                        foreach (var result in results)
                        {
                            userDto.Roles.Add(result.Role);
                        }
                        userDto.Name = results.First().Name;
                        userDto.Login = results.First().Login;
                        userDto.Id = results.First().Id;
                        userDto.Password = null;
                        return OperationDetails<UserDto>.Success(userDto);
                    }
                    else
                        return OperationDetails<UserDto>.Failure("Неверный логин или пароль", "");
                }
                catch (Exception ex)
                {
                    return OperationDetails<UserDto>.Failure(ex.Message, ex.Source);
                }
            }
        }

        public async Task<OperationDetails<bool>> ChangeLoginAsync(ChangeUserLoginRequest request)
        {
            using (var context = _contextFactory.Create())
            {
                try
                {
                    var affectedRows = await context.TeachersQuery().ChangeLogin(request.Id, request.NewLogin);
                    if (affectedRows == 0)
                        throw new Exception("Запись не найдена");
                    return OperationDetails<bool>.Success(true);
                }
                catch (Exception ex)
                {
                    return OperationDetails<bool>.Failure(ex.Message, ex.Source);
                }
            }
        }

        public async Task<OperationDetails<bool>> ChangePasswordAsync(ChangeUserPasswordRequest request)
        {
            using (var context = _contextFactory.Create())
            {
                try
                {
                    var affectedRows = await context.TeachersQuery().ChangePassword(request.Id, request.OldPassword, request.NewPassword);
                    if (affectedRows == 0)
                        throw new Exception("Запись не найдена");
                    return OperationDetails<bool>.Success(true);
                }
                catch (Exception ex)
                {
                    return OperationDetails<bool>.Failure(ex.Message, ex.Source);
                }
            }
        }
    }

}
