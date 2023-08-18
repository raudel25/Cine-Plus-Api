using Cine_Plus_Api.Models;
using Cine_Plus_Api.Services;
using Microsoft.EntityFrameworkCore;

namespace Cine_Plus_Api.Queries;

public interface IAuthQueryHandler
{
    Task<User?> User(string email);

    Task<User?> User(int id);

    Task<int> MaxEmploy();

    Task<int> MaxManager();

    Task<Employ?> Employ(int id);

    Task<Manager?> Manager(int id);

    Task<Employ?> Employ(string user);

    Task<Manager?> Manager(string user);
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

    public async Task<int> MaxEmploy()
    {
        if (!await _context.Employs.AnyAsync()) return 0;
        return await this._context.Employs.Select(employ => employ.Id).MaxAsync();
    }

    public async Task<int> MaxManager()
    {
        if (!await _context.Managers.AnyAsync()) return 0;
        return await this._context.Managers.Select(manager => manager.Id).MaxAsync();
    }

    public async Task<Employ?> Employ(int id)
    {
        return await this._context.Employs.SingleOrDefaultAsync(employ => employ.Id == id);
    }

    public async Task<Manager?> Manager(int id)
    {
        return await this._context.Managers.SingleOrDefaultAsync(manager => manager.Id == id);
    }

    public async Task<Employ?> Employ(string user)
    {
        return await this._context.Employs.SingleOrDefaultAsync(employ => employ.User == user);
    }

    public async Task<Manager?> Manager(string user)
    {
        return await this._context.Managers.SingleOrDefaultAsync(manager => manager.User == user);
    }
}