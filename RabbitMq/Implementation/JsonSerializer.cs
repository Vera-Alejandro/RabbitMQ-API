using Newtonsoft.Json;
using System;
using System.Text;

namespace Interstates.Control.MessageBus.RabbitMq.Implementation
{
    internal sealed class JsonSerializer : ISerializer
    {
        private readonly Encoding _encoding;
        private readonly JsonSerializerSettings _settings;

        public JsonSerializer(Encoding encoding, JsonSerializerSettings settings)
        {
            _encoding = encoding ?? throw new ArgumentNullException(nameof(encoding));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public JsonSerializer(JsonSerializerSettings settings)
            : this(Encoding.UTF8, settings)
        {
        }

        public TValue Deserialize<TValue>(byte[] bytes) =>
            JsonConvert.DeserializeObject<TValue>(_encoding.GetString(bytes), _settings);

        public byte[] Serialize<TValue>(TValue value) => 
            _encoding.GetBytes(JsonConvert.SerializeObject(value, _settings));
    }
}
