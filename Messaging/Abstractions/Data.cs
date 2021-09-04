using Messaging.Interfaces;

namespace Messaging.Abstractions
{
    public abstract class Data
    {
        public abstract int Length();

        public abstract int Serialize(byte[] data, int offset);

        public abstract bool Deserialize(byte[] data, int offset);
    }

    public class Data<T> : Data
    {
        protected ISerializer<T> Serializer { get; private set; } = null!;

        private T _value;

        public T Value { get => _value; set => _value =value; }
        
        public Data(T value)
        {
            _value = value;
        }

        public void SetSerializer(ISerializer<T> serializer)
        {
            Serializer = serializer;
        }

        public override int Length()
        {
            return Serializer.Length();
        }

        public override int Serialize(byte[] data, int offset)
        {
            return Serializer.Serialize(_value, data, offset);
        }

        public override bool Deserialize(byte[] data, int offset)
        {
            return Serializer.Deserialize(ref _value, data, offset);
        }
    }
}
