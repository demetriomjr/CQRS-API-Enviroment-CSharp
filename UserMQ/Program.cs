var factory = new ConnectionFactory() { HostName = "localhost" };
using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

var queueName = (await channel.QueueDeclareAsync(queue: "users",
                                durable: false,
                                exclusive: false,
                                autoDelete: false,
                                arguments: null)).QueueName;


var consumer = new AsyncEventingBasicConsumer(channel);

consumer.ReceivedAsync += async (model, ea) =>
{
    await Task.Delay(1000);
};