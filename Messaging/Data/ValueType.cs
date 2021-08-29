
using Messaging.Abstractions;
using Messaging.Interfaces;
using Messaging.Serializer;

namespace Messaging.Data
{
    public class ValueType<T, TS> : Data<T, TS> where T : struct where TS : ISerializer<T>
    {
        public ValueType(ISerializerFactory factory) : base(factory)
        {

        }

        public ValueType(ISerializerFactory factory, T value) : base(factory, value)
        {

        }
    }

    public class ValueType<T> : ValueType<T, ValueTypeSerializer<T>> where T : struct
    {
        public ValueType(ISerializerFactory factory) : base(factory)
        {

        }

        public ValueType(ISerializerFactory factory, T value) : base(factory, value)
        {

        }
    }
}
