namespace Database
{
    public static class Database
    {
        public static SQLDatabase SQL = new(null!);
        public static RedisDatabase Redis = new();
        public static MongoDatabase Mongo = new();
    }
}
