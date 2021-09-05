using System.Collections.Generic;
using System.IO;
using Messaging.Interfaces;
using Newtonsoft.Json;

namespace Messaging.Abstractions
{
    public class JsonConfigEntry : IConfig
    {
        public string TypeName { get; set; } = "";

        public object[]? TypeArgs { get; set; } = null;
               
        public string? Value { get; set; } = null;
               
        public string? Tag { get; set; } = null;
    }

    public class JsonConfigReader : IConfigReader
    {
        public IEnumerable<IConfig> Read(string file)
        {
            using StreamReader r = new(file);
            string json = r.ReadToEnd();

            var data = JsonConvert.DeserializeObject<List<JsonConfigEntry>>(json);

            if (data == null)
            {
                return new List<IConfig>();
            }
            return data;
        }
    }
}
