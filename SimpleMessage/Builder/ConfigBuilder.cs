using Messaging.Builder;

namespace SimplePacket.Builder
{
    public class ConfigBuilder : ConfigBuilder<SimplePacket, JsonConfigReader>
    {
        public ConfigBuilder(string filename)
        {
            WithConfig(filename);
        }
    }
}
