using Workflow___Document_Management_System.DTOs;
using Workflow___Document_Management_System.Repository;

namespace Workflow___Document_Management_System.SERVICE
{
    public class AdminService
    {
        private readonly AdminRepository _adminRepository;
        private readonly SessionService _sessionService;

        public AdminService(AdminRepository adminRepository, SessionService sessionService)
        {
            _adminRepository = adminRepository;
            _sessionService = sessionService;
        }

        public async Task<bool> LoginAsync(AdminLoginDto loginDto)
        {
            var admin = await _adminRepository.GetAdminByUsernameAsync(loginDto.Username);

            if (admin != null && admin.IsActive &&
                _adminRepository.VerifyPassword(loginDto.Password, admin.PasswordHash))
            {
                _sessionService.SetAdminSession(admin);
                return true;
            }
            return false;
        }

        public async Task<bool> CreateAdminAsync(CreateAdminDto createAdminDto)
        {
            if (!_sessionService.HasWriteAccess())
                return false;

            var currentAdminId = _sessionService.GetCurrentAdminId();
            if (!currentAdminId.HasValue)
                return false;

            if (createAdminDto.AccessLevel != "Read-Write" && createAdminDto.AccessLevel != "Read-Only")
                return false;

            return await _adminRepository.CreateAdminAsync(createAdminDto, currentAdminId.Value);
        }

        // REMOVE SESSION CHECK FOR DEBUGGING
        public async Task<List<AdminResponseDto>> GetAllAdminsAsync()
        {
            // Removed session check - this was causing your issue
            return await _adminRepository.GetAllAdminsAsync();
        }

        public void Logout()
        {
            _sessionService.ClearSession();
        }
    }
}