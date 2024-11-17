namespace project_2lab.Data
{
    public class AuthenticationService
    {
        private const string AdminEmail = "aitisam@gmail.com"; // Admin Email
        private const string AdminPassword = "Aitisam@1234";       // Admin Password

        private bool isAuthenticated = false;
        private bool isAdmin = false;

        public Task<bool> LoginAsync(string email, string password)
        {
            // Validate email and password
            if (email == AdminEmail && password == AdminPassword)
            {
                isAuthenticated = true;
                isAdmin = true;
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }

        public void Logout()
        {
            isAuthenticated = false;
            isAdmin = false;
        }

        public bool IsAuthenticated => isAuthenticated;
        public bool IsAdmin => isAdmin;
    }
}
