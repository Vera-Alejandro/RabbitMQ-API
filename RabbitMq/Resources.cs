using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Interstates.Control.MessageBus.RabbitMq
{
    public static class Resources
    {
        public static JsonSerializerSettings DefaultJsonSerializerSettings => new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy
                {
                    OverrideSpecifiedNames = false
                }
            },
            Formatting = Formatting.Indented
        };
    }
}
