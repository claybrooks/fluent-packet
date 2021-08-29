using Messaging.Interfaces;

namespace Messaging.Abstractions
{
    public abstract class Data<T> : IData<T>
    {
        protected T _value;

        public T Value { get => _value; set => _value = value; }

        public abstract int Length();

        public abstract bool Deserialize(byte[] data, int offset);


        public abstract int Serialize(byte[] data, int offset);
    }

    public abstract class Data<T, TS> : Data<T> where TS : ISerializer<T>
    {
        protected TS Serializer;

        protected Data(ISerializerFactory factory)
        {
            factory.Register<T, TS>();
            Serializer = (TS)factory.Get<T>();
        }

        protected Data(ISerializerFactory factory, T value) : this(factory)
        {
            Value = value;
        }

        public override int Length()
        {
            return Serializer.Length();
        }

        public override bool Deserialize(byte[] data, int offset)
        {
            return Serializer.Deserialize(ref _value, data, offset);
        }

        public override int Serialize(byte[] data, int offset)
        {
            return Serializer.Serialize(_value, data, offset);
        }
    }
}
