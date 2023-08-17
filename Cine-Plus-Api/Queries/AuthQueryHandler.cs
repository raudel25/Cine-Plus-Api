using Cine_Plus_Api.Models;
using Cine_Plus_Api.Services;
using Microsoft.EntityFrameworkCore;

namespace Cine_Plus_Api.Queries;

public interface IAuthQueryHandler
{
    Task<User?> User(string email);

    Task<User?> User(int id);

    Task<Employ?> Employ(string name);

    Task<Employ?> Employ(int id);

    Task<Manager?> Manager(int id);
}

public class AuthQueryHandler : IAuthQueryHandler
{
    private readonly CinePlusContext _context;

    public AuthQueryHandler(CinePlusContext context)
    {
        this._context = context;
    }

    public async Task<User?> User(string email)
    {
        return await this._context.Users.SingleOrDefaultAsync(user => user.Email == email);
    }

    public async Task<User?> User(int id)
    {
        return await this._context.Users.SingleOrDefaultAsync(user => user.Id == id);
    }

    public async Task<Employ?> Employ(string name)
    {
        return await this._context.Employs.SingleOrDefaultAsync(employ => employ.Name == name);
    }

    public async Task<Employ?> Employ(int id)
    {
        return await this._context.Employs.SingleOrDefaultAsync(employ => employ.Id == id);
    }

    public async Task<Manager?> Manager(int id)
    {
        return await this._context.Managers.SingleOrDefaultAsync(manager => manager.Id == id);
    }
}