using System.Collections.Generic;

namespace FluentPacket.Interfaces
{
    public interface IConfigReader
    {
        IEnumerable<IConfig> Read(string file);
    }
}
