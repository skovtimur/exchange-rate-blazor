using StackExchange.Redis;

namespace ExchangeRate.Cache;

public class RedisManager(IConnectionMultiplexer connection, ILogger<RedisManager> logger) : IRedisManager
{
    public void FlushAll()
    {
        foreach (var endpoint in connection.GetEndPoints())
        {
            var server = connection.GetServer(endpoint);
            server.FlushDatabase();
        }
    }
}