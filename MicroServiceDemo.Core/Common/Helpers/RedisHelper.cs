using StackExchange.Redis;

namespace MicroServiceDemo.Core.Common.Helpers
{
    public class RedisHelper
    {
        private static readonly object _syncObject = new object();

        private readonly string _connectionString;
        private ConnectionMultiplexer _connection;

        public RedisHelper()
        {
            _connectionString = ConfigHelper.RedisUrl;
        }

        protected ConnectionMultiplexer GetConnection()
        {
            if (_connection != null && _connection.IsConnected) return _connection;

            lock (_syncObject)
            {
                if (_connection != null && _connection.IsConnected) return _connection;

                _connection?.Dispose();

                _connection = ConnectionMultiplexer.Connect(_connectionString);
            }

            return _connection;
        }

        public IDatabase GetDatabase(int? db = null)
        {
            return GetConnection().GetDatabase(db ?? -1);
        }

        public void SubscribeMessage(string channel, System.Action<RedisChannel, RedisValue> action)
        {
            ISubscriber sub = this.GetConnection().GetSubscriber();
            sub.Subscribe(channel, action);
        }
    }
}