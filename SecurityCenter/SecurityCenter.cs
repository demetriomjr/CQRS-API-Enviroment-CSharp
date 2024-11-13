namespace SecurityHQ
{
    public static class SecurityCenter
    {
        public static TokenManagement Tokens(JwtSettings settings) => new(settings);
    }
}
