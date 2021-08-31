
namespace Messaging.Interfaces
{
    public interface IConfig
    {
        public string TypeName { get; }
        public string Value { get; }
        public string Tag { get; }
    }
}
