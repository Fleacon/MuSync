using backend.DB.DAO;
using backend.Models;

namespace backend.Services;

public class AccountService
{
    private readonly UsersDAO usersDao;
    private readonly OAuthTokensDAO oAuthTokensDao;

    public AccountService(UsersDAO usersDao, OAuthTokensDAO oAuthTokensDao)
    {
        this.usersDao = usersDao;
        this.oAuthTokensDao = oAuthTokensDao;
    }
    
    public async Task<(LoginResult result, User? user)> ValidateCredentials(string username, string password)
    {
        var user = await usersDao.GetUserByUsername(username);
        if (user is null)
            return (LoginResult.NotFound, null);

        if (!PasswordService.VerifyPassword(user.PasswordHash, password))
            return (LoginResult.Unauthorized, null);

        return (LoginResult.Success, user);
    }
    
    public async Task<User?> CreateAccount(string username, string password)
    {
        var existing = await usersDao.GetUserByUsername(username);
        if (existing is not null)
            return null;

        var hashedPw = PasswordService.HashPassword(password);
        return await usersDao.CreateUser(new(0, username, hashedPw));
    }
    
    public async Task<IReadOnlyList<Provider>> GetLinkedProviders(int userId)
    {
        var tokens = await oAuthTokensDao.GetOAuthTokenByUserId(userId);
        return tokens.Select(t => t.Provider).Distinct().ToList();
    }

    public async Task<bool> RemoveProvider(Provider provider, int userId)
    {
        return await oAuthTokensDao.DeleteOAuthTokenByUserId(provider, userId);
    }

    public async Task<bool> DeleteAccount(int userId)
    {
        return await usersDao.DeleteUserById(userId);
    }
}