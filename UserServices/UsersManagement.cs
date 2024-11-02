using Microsoft.EntityFrameworkCore;

namespace UserServices
{
    public static class UsersManagement
    {
        public static async Task<(bool result, Guid userCode, string error)> ValidateAndGenerateCode(string username, string password)
        {
            using(var db = Database.SQL)
            {
                var user = await db.Users.FirstOrDefaultAsync(x => x.Username.ToLower().Equals(username.ToLower().Trim()));

                if (user is null)
                {
                    var error = "Username not found";
                    return (false, Guid.Empty, error);
                }

                await db.DisposeAsync();
                var UserCode = Guid.NewGuid();
                return (true, UserCode, string.Empty);
            }
            
        }
    }
}
