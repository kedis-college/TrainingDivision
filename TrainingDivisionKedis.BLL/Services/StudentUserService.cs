using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using TrainingDivisionKedis.BLL.Common;
using TrainingDivisionKedis.BLL.Contracts;
using TrainingDivisionKedis.BLL.DTO.User;
using TrainingDivisionKedis.Core.SPModels.User;
using TrainingDivisionKedis.DAL.Contracts;
using TrainingDivisionKedis.DAL.Extensions;

namespace TrainingDivisionKedis.BLL.Services
{
    public class StudentUserService : IUserService
    {
        private readonly IAppDbContextFactory _contextFactory;

        public StudentUserService(IAppDbContextFactory contextFactory)
        {
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
        }

        public async Task<OperationDetails<UserDto>> AuthenticateAsync(UserDto userDto)
        {
            using (var context = _contextFactory.Create())
            {
                try
                {
                    var result = await context.StudentsQuery().Authenticate(userDto.Login, userDto.Password);
                    if (result != null)
                    {
                        userDto.Roles.Add(result.Role);
                        userDto.Name = result.Name;
                        userDto.Login = result.Login;
                        userDto.Id = result.Id;
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

        public Task<OperationDetails<bool>> ChangeLoginAsync(ChangeUserLoginRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task<OperationDetails<bool>> ChangePasswordAsync(ChangeUserPasswordRequest request)
        {
            using (var context = _contextFactory.Create())
            {
                try
                {
                    await context.StudentsQuery().ChangePassword(request.Id, request.OldPassword, request.NewPassword);
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
