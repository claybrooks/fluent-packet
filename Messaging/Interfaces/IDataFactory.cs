
namespace Messaging.Interfaces
{
    public interface IDataFactory
    {
        public void Register<T>() where T : struct;

        public void Register<TK, TV>() where TV : IData<TK>;

        public IData<T> Create<T>(T value);
    }
}
