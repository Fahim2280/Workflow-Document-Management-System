using System.Data;
using System.Data.SqlClient;
using Workflow___Document_Management_System.DTOs;
using Workflow___Document_Management_System.Models;

namespace Workflow___Document_Management_System.Repository
{
    public class AdminRepository
    {
        private readonly string _connectionString;

        public AdminRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<Admin> GetAdminByUsernameAsync(string username)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_GetAdminByUsername", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@Username", username);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new Admin
                {
                    AdminId = reader.GetInt32("AdminId"),
                    Username = reader.GetString("Username"),
                    Email = reader.GetString("Email"),
                    PasswordHash = reader.GetString("PasswordHash"), // Now stores plain password
                    AccessLevel = reader.GetString("AccessLevel"),
                    CreatedByAdminId = reader.GetInt32("CreatedByAdminId"),
                    CreatedDate = reader.GetDateTime("CreatedDate"),
                    IsActive = reader.GetBoolean("IsActive")
                };
            }
            return null;
        }

        public async Task<bool> CreateAdminAsync(CreateAdminDto createAdminDto, int createdByAdminId)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_CreateAdmin", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@Username", createAdminDto.Username);
            command.Parameters.AddWithValue("@Email", createAdminDto.Email);
            command.Parameters.AddWithValue("@PasswordHash", createAdminDto.Password); // Store plain password
            command.Parameters.AddWithValue("@AccessLevel", createAdminDto.AccessLevel);
            command.Parameters.AddWithValue("@CreatedByAdminId", createdByAdminId);

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result) > 0;
        }

        public async Task<List<AdminResponseDto>> GetAllAdminsAsync()
        {
            var admins = new List<AdminResponseDto>();
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_GetAllAdmins", connection);
            command.CommandType = CommandType.StoredProcedure;

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                admins.Add(new AdminResponseDto
                {
                    AdminId = reader.GetInt32("AdminId"),
                    Username = reader.GetString("Username"),
                    Email = reader.GetString("Email"),
                    AccessLevel = reader.GetString("AccessLevel"),
                    CreatedDate = reader.GetDateTime("CreatedDate"),
                    IsActive = reader.GetBoolean("IsActive")
                });
            }
            return admins;
        }

        // Simple password verification (no hashing)
        public bool VerifyPassword(string password, string storedPassword)
        {
            return password == storedPassword;
        }
    }
}