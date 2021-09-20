using Messaging.Factory;
using Messaging.Interfaces;

namespace Messaging.Serializer
{
    public abstract class Serializer<T> : ISerializer<T>
    {
        protected SerializerFactory _factory = null!;
        
        public int Serialize(T value, byte[] data, int offset)
        {
            var b = Serialize(value);
            b.CopyTo(data, offset);
            return b.Length;
        }

        public void SetFactory(SerializerFactory factory)
        {
            _factory = factory;
        }

        public abstract bool Deserialize(ref T value, byte[] data, int offset);
        public abstract byte[] Serialize(T value);
        public abstract int Length();
    }
}
