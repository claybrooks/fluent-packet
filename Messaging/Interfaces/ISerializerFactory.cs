
namespace Messaging.Interfaces
{
    public interface ISerializerFactory
    {
        public void Register<TK, TV>() where TV : ISerializer<TK>;

        public ISerializer<T> Get<T>();
    }
}
