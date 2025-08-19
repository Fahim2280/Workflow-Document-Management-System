using Workflow___Document_Management_System.Models;

namespace Workflow___Document_Management_System.SERVICE
{
    public class SessionService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SessionService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void SetAdminSession(Admin admin)
        {
            var session = _httpContextAccessor.HttpContext.Session;
            session.SetInt32("AdminId", admin.AdminId);
            session.SetString("Username", admin.Username);
            session.SetString("AccessLevel", admin.AccessLevel);
            session.SetString("IsLoggedIn", "true");
        }

        public bool IsLoggedIn()
        {
            var session = _httpContextAccessor.HttpContext.Session;
            return session.GetString("IsLoggedIn") == "true";
        }

        public int? GetCurrentAdminId()
        {
            var session = _httpContextAccessor.HttpContext.Session;
            return session.GetInt32("AdminId");
        }

        public string GetCurrentUsername()
        {
            var session = _httpContextAccessor.HttpContext.Session;
            return session.GetString("Username");
        }

        public string GetCurrentAccessLevel()
        {
            var session = _httpContextAccessor.HttpContext.Session;
            return session.GetString("AccessLevel");
        }

        public bool HasWriteAccess()
        {
            return GetCurrentAccessLevel() == "Read-Write";
        }

        public void ClearSession()
        {
            var session = _httpContextAccessor.HttpContext.Session;
            session.Clear();
        }
    }

}
