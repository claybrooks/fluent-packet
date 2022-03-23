using FluentPacket.Factory;
using FluentPacket.Interfaces;
using System;

namespace FluentPacket.Serializer
{
    public abstract class Serializer<T> : ISerializer<T>
    {
        protected SerializerFactory _factory = null!;

        public int Serialize(T value, byte[] data, int offset)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (data.Length - offset < Length())
            {
                throw new ArgumentOutOfRangeException(nameof(offset));
            }

            var b = Serialize(value);
            b.CopyTo(data, offset);
            return b.Length;
        }

        public void SetFactory(SerializerFactory factory)
        {
            _factory = factory;
        }

        public abstract bool Deserialize(out T value, byte[] data, int offset);
        public abstract byte[] Serialize(T value);
        public abstract int Length();
    }
}
