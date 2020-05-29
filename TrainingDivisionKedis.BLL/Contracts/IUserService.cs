using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TrainingDivisionKedis.BLL.Common;
using TrainingDivisionKedis.BLL.DTO.User;

namespace TrainingDivisionKedis.BLL.Contracts
{
    public interface IUserService
    {
        Task<OperationDetails<UserDto>> AuthenticateAsync(UserDto userDto);
        Task<OperationDetails<bool>> ChangePasswordAsync(ChangeUserPasswordRequest request);
        Task<OperationDetails<bool>> ChangeLoginAsync(ChangeUserLoginRequest request);
    }
}
