using System.Collections.Generic;

namespace Messaging.Interfaces
{
    public interface IConfigReader
    {
        IEnumerable<IConfig> Read(string file);
    }
}
