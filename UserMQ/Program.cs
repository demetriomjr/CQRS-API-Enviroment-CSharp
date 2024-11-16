var factory = new ConnectionFactory() { HostName = "localhost" };
using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

var requestQueue = await channel.QueueDeclareAsync(queue: "users_requests",
                                durable: false,
                                exclusive: false,
                                autoDelete: false,
                                arguments: null);

var responseQueue = await channel.QueueDeclareAsync(queue: "users_responses",
                                durable: false,
                                exclusive: false,
                                autoDelete: false,
                                arguments: null);


var consumer = new AsyncEventingBasicConsumer(channel);

consumer.ReceivedAsync += async (model, ea) =>
{
    await Task.Delay(1000);
};