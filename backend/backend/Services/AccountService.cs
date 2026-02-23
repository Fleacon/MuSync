using backend.DB.DAO;
using backend.Models;

namespace backend.Services;

public class AccountService
{
    private readonly UsersDAO usersDao;

    public AccountService(UsersDAO usersDao)
    {
        this.usersDao = usersDao;
    }
    
    public async Task<User?> ValidateCredentials(string username, string password)
    {
        var user = await usersDao.GetUserByUsername(username);
        if (user is null)
            return null;

        if (!PasswordService.VerifyPassword(user.PasswordHash, password))
            throw new UnauthorizedAccessException();

        return user;
    }
    
    public async Task<User?> CreateAccount(string username, string password)
    {
        var existing = await usersDao.GetUserByUsername(username);
        if (existing is not null)
            return null;

        var hashedPw = PasswordService.HashPassword(password);
        return await usersDao.CreateUser(new(0, username, hashedPw));
    }
}