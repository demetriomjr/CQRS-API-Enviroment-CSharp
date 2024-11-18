var factory = new ConnectionFactory() { HostName = "localhost"};
using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

var UserRequestQueue = await DeclareQueue("user_requests");
var UserResponseQueue = await DeclareQueue("user_responses");

