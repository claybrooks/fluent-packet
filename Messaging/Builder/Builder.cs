using Messaging.Interfaces;

namespace Messaging.Builder
{
    public class Builder<TP>
        where TP : Packet, new()
    {
        protected TP _packet;

        public Builder()
        {
            _packet = new TP();
        }

        public TP Produce()
        {
            _packet = new TP();
            Assemble();
            return Build();
        }

        public TP Build()
        {
            return _packet;
        }

        public virtual void Assemble() {}

        public Builder<TP> WithData<T>(T value, ISerializer<T>? serializer = null)
        {
            _packet.AddData(value, serializer);
            return this;
        }

        public Builder<TP> WithData<T>(T value, int tag, ISerializer<T>? serializer = null)
        {
            _packet.AddData(value, tag, serializer);
            return this;
        }
    }

    public class DefaultPacketBuilder : Builder<DefaultPacket>
    {
    }
}
