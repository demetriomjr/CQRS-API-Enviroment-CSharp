namespace Databases
{
    public static class Database
    {
        private static readonly string SQLConnectionString = new MySqlConnectionStringBuilder()
        {
            Server = "localhost",
            Port = 3306,
            Database = "cqrs api",
            UserID = "root",
            Password = "123321",
            SslMode = MySqlSslMode.None,
        }.ConnectionString;

        public static SQLDatabase SQL = new(new DbContextOptionsBuilder<SQLDatabase>().UseMySql(SQLConnectionString,
                                                ServerVersion.AutoDetect(SQLConnectionString)).Options);
        public static RedisDatabase Redis = new();
        public static MongoDatabase Mongo = new();
    }
}
