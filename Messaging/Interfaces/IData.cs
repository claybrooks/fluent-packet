
namespace Messaging.Interfaces
{
    public interface IData
    {
        int Length();

        int Serialize(byte[] data, int offset);

        bool Deserialize(byte[] data, int offset);
    }

    public interface IData<T> : IData
    {
        T Value { get; set; }
    }
}
