
namespace Messaging.Interfaces
{
    public interface IBuilder<out TM> where TM : IPacket
    {
        IBuilder<TM> WithData<T>(long tag);

        IBuilder<TM> WithData<T>(T value);

        TM Build();
    }
}
