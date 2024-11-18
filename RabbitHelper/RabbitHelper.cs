using System.Diagnostics;
using RabbitMQ.Client;

namespace RabbitHelper
{
    public class RabbitHelper : IDisposable
    {
        private static IConnection? _connection;
        private readonly IChannel _channel;
        public enum QueueNames
        {
            User,
            Token
        }

        public RabbitHelper(IChannel channel)
        {
            _channel = channel;
        }

        public static async Task<RabbitHelper> CreateAsync(string hostname)
        {
            try
            {
                var factory = new ConnectionFactory() { HostName = hostname };
                if (_connection is null)
                    _connection = await factory.CreateConnectionAsync();

                var channel = await _connection.CreateChannelAsync();
                var result = new RabbitHelper(channel);

                return result;
            }
            catch (Exception)
            {
                if(Debugger.IsAttached)
                    throw;
                return null!;
            }
        }
        private async Task<bool> DeclareQueues(QueueNames[]? queues, bool wipePrevious)
        {
            try
            {
                if (_connection is null)
                    return false;

                if(queues is { Length: 0 })
                    queues = Enum.GetValues<QueueNames>();

                foreach(var queue in queues!)
                {
                    var queueNamesFormatted = GetQueueNames(queue);
                    await DeclareQueue(queueNamesFormatted.request);
                    await DeclareQueue(queueNamesFormatted.response);
                }

                return true;
            }
            catch (Exception)
            {
                if(Debugger.IsAttached)
                    throw;
                return false;
            }
        }
        private async Task DeclareQueue(string name)
        {
            await _channel.QueueDeclareAsync(queue: name,
                                       arguments: null,
                                       exclusive: false,
                                       autoDelete: false,
                                       durable: true);
        }
        public static (string request, string response) GetQueueNames(QueueNames queue)
        {
            return (request: $"{queue.ToString().ToLower()}Request",
                    response: $"{queue.ToString().ToLower()}Response");
        }
        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}
