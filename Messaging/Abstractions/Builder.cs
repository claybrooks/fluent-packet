using Messaging.Interfaces;

namespace Messaging.Abstractions
{

    public class Builder<TPacket> : IBuilder<TPacket> where TPacket : IPacket, new()
    {
        private TPacket _packet;

        public Builder()
        {
            _packet = new TPacket();
        }

        public virtual TPacket Build()
        {
            return _packet;
        }

        public IBuilder<TPacket> WithData<T>(T value)
        {
            _packet.AddData(value);
            return this;
        }

        public IBuilder<TPacket> WithData<T>(T value, long tag)
        {
            _packet.AddData(value, tag);
            return this;
        }

        public IBuilder<TPacket> WithData<T>()
        {
            return WithData(default(T));
        }

        public IBuilder<TPacket> WithData<T>(long tag)
        {
            return WithData(default(T), tag);
        }
    }
}
