using FluentPacket.Interfaces;

namespace FluentPacket
{
    public class Builder<T>
        where T : Packet, new()
    {
        protected T _packet;

        public Builder()
        {
            _packet = new T();
        }

        public T Produce()
        {
            _packet = new T();
            Assemble();
            return Build();
        }

        public T Build()
        {
            return _packet;
        }

        public virtual void Assemble() {}

        public Builder<T> WithData<U>(U value, ISerializer<U>? serializer = null)
        {
            _packet.AddData(value, serializer);
            return this;
        }

        public Builder<T> WithData<U>(U value, int tag, ISerializer<U>? serializer = null)
        {
            _packet.AddData(value, tag, serializer);
            return this;
        }
    }

    public class DefaultPacketBuilder : Builder<DefaultPacket>
    {
    }
}
