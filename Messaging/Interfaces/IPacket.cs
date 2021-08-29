
namespace Messaging.Interfaces
{
    public interface IPacket
    {
        bool Serialize(out byte[] data);

        bool Deserialize(byte[] data);

        bool AddData<T>(T value);

        bool AddData<T>(T value, long tag);

        T GetData<T>(long tag);

        bool SetData<T>(long tag, T value);

        IDataFactory DataFactory { get; }

        ISerializerFactory SerializerFactory { get; }
    }
}
