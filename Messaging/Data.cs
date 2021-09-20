using Messaging.Interfaces;

namespace Messaging
{
    public abstract class Data
    {
        public abstract int Length();

        public abstract int Serialize(byte[] data, int offset);

        public abstract bool Deserialize(byte[] data, int offset);

        public abstract void Clear();
    }

    public abstract class Data<T> : Data
    {
        protected readonly ISerializer<T> _serializer;

        private T _value;

        public T Value { get => _value; set => _value =value; }

        protected Data(T value, ISerializer<T> serializer)
        {
            _value = value;
            _serializer = serializer;
        }

        public override int Length()
        {
            return _serializer.Length();
        }

        public override int Serialize(byte[] data, int offset)
        {
            return _serializer.Serialize(_value, data, offset);
        }

        public override bool Deserialize(byte[] data, int offset)
        {
            return _serializer.Deserialize(ref _value, data, offset);
        }
    }
}
