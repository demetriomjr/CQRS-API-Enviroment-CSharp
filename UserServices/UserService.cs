
namespace UserServices
{
    public class UserService
    {
        public async Task<UserValidationResponse> ValidateAndGenerateCode(string username, string password)
        {
            using (var db = Database.SQL)
            {
                var user = await db.Users.FirstOrDefaultAsync(x => x.Username.Equals(username.ToLower().Trim()));

                if (user is null)
                {
                    var error = "Username not found";
                    return new(false, Guid.Empty, error);
                }

                await db.DisposeAsync();
                var UserCode = Guid.NewGuid();
                return new(true, UserCode, string.Empty);
            }

        }
    }
}
