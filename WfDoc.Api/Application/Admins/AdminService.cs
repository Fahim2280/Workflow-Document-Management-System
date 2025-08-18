using System.Data;
using BCrypt.Net;
using Dapper;
using WfDoc.Api.Common;
using WfDoc.Api.Domain;
using WfDoc.Api.Infrastructure;

namespace WfDoc.Api.Application.Admins;

public interface IAdminService
{
    Task<(Admin admin, string token)> LoginAsync(string username, string password);
    Task<int> CreateAdminAsync(int creatorAdminId, string username, string password, AccessLevel accessLevel);
    Task<IEnumerable<Admin>> GetAdminsAsync();
}

public sealed class AdminService : IAdminService
{
    private readonly ISqlConnectionFactory _connectionFactory;
    private readonly Application.Auth.IJwtTokenService _jwt;

    public AdminService(ISqlConnectionFactory connectionFactory, Application.Auth.IJwtTokenService jwt)
    {
        _connectionFactory = connectionFactory;
        _jwt = jwt;
    }

    public async Task<(Admin admin, string token)> LoginAsync(string username, string password)
    {
        await using var conn = _connectionFactory.Create();
        var admin = await conn.QueryFirstOrDefaultAsync<Admin>(
            "sp_Admin_GetByUsername",
            new { Username = username },
            commandType: CommandType.StoredProcedure);

        if (admin is null || !BCrypt.Net.BCrypt.Verify(password, admin.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        var token = _jwt.CreateToken(admin.Id, admin.Username, admin.AccessLevel.ToString());
        return (admin, token);
    }

    public async Task<int> CreateAdminAsync(int creatorAdminId, string username, string password, AccessLevel accessLevel)
    {
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
        await using var conn = _connectionFactory.Create();
        var id = await conn.ExecuteScalarAsync<int>(
            "sp_Admin_Create",
            new
            {
                Username = username,
                PasswordHash = passwordHash,
                AccessLevel = (int)accessLevel,
                CreatedByAdminId = creatorAdminId
            },
            commandType: CommandType.StoredProcedure);

        return id;
    }

    public async Task<IEnumerable<Admin>> GetAdminsAsync()
    {
        await using var conn = _connectionFactory.Create();
        var admins = await conn.QueryAsync<Admin>("sp_Admin_List", commandType: CommandType.StoredProcedure);
        return admins;
    }
}

