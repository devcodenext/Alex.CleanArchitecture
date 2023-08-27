using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CleanArchitecture.Application.Interfaces;
using CleanArchitecture.Application.Queries.Users.GetAll;
using CleanArchitecture.Application.Queries.Users.GetUserById;
using CleanArchitecture.Application.ViewModels.Users;
using CleanArchitecture.Domain.Commands.Users.ChangePassword;
using CleanArchitecture.Domain.Commands.Users.CreateUser;
using CleanArchitecture.Domain.Commands.Users.DeleteUser;
using CleanArchitecture.Domain.Commands.Users.LoginUser;
using CleanArchitecture.Domain.Commands.Users.UpdateUser;
using CleanArchitecture.Domain.Interfaces;

namespace CleanArchitecture.Application.Services;

public sealed class UserService : IUserService
{
    private readonly IMediatorHandler _bus;
    private readonly IUser _user;

    public UserService(IMediatorHandler bus, IUser user)
    {
        _bus = bus;
        _user = user;
    }

    public async Task<UserViewModel?> GetUserByUserIdAsync(Guid userId, bool isDeleted)
    {
        return await _bus.QueryAsync(new GetUserByIdQuery(userId, isDeleted));
    }

    public async Task<UserViewModel?> GetCurrentUserAsync()
    {
        return await _bus.QueryAsync(new GetUserByIdQuery(_user.GetUserId(), false));
    }

    public async Task<IEnumerable<UserViewModel>> GetAllUsersAsync()
    {
        return await _bus.QueryAsync(new GetAllUsersQuery());
    }

    public async Task<Guid> CreateUserAsync(CreateUserViewModel user)
    {
        var userId = Guid.NewGuid();

        await _bus.SendCommandAsync(new CreateUserCommand(
            userId,
            user.TenantId,
            user.Email,
            user.FirstName,
            user.LastName,
            user.Password));

        return userId;
    }

    public async Task UpdateUserAsync(UpdateUserViewModel user)
    {
        await _bus.SendCommandAsync(new UpdateUserCommand(
            user.Id,
            user.Email,
            user.FirstName,
            user.LastName,
            user.Role));
    }

    public async Task DeleteUserAsync(Guid userId)
    {
        await _bus.SendCommandAsync(new DeleteUserCommand(userId));
    }

    public async Task ChangePasswordAsync(ChangePasswordViewModel viewModel)
    {
        await _bus.SendCommandAsync(new ChangePasswordCommand(viewModel.Password, viewModel.NewPassword));
    }

    public async Task<string> LoginUserAsync(LoginUserViewModel viewModel)
    {
        return await _bus.QueryAsync(new LoginUserCommand(viewModel.Email, viewModel.Password));
    }
}