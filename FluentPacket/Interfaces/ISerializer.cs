using FluentPacket.Factory;

namespace FluentPacket.Interfaces
{
    public interface ISerializer
    {
        int Length();

        void SetFactory(SerializerFactory factory);
    }

    public interface ISerializer<T> : ISerializer
    {
        byte[] Serialize(T value);

        int Serialize(T value, byte[] data, int offset);

        bool Deserialize(out T value, byte[] data, int offset);
    }
}