
namespace Databases
{
    public class RedisDatabase : IDisposable
    {
        private ConnectionMultiplexer _connection;
        public RedisDatabase(string connectionString)
        {
            try
            {
                _connection = ConnectionMultiplexer.Connect(connectionString);
            }
            catch 
            {
                throw;
            }
        }

        public IDatabase Connect()
        {
            return _connection.GetDatabase();
        }

        public void Dispose()
        {
            _connection.Dispose();
        }
    }
}
