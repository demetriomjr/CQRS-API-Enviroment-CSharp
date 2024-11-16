namespace Models.Users
{
    public class User : BaseModel
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string UserCode { get; set; } = string.Empty;

        public User() : base()
        {

        }

        public User(string fullName, string username, string password) : base()
        {
            FullName = fullName;
            Username = username;
            Password = password;
        }
    }
}
