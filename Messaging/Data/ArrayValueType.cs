using Messaging.Abstractions;

namespace Messaging.Data
{
    public class ArrayValueType<T> : Data<T[]> where T : struct
    {
        public ArrayValueType() : this(new T[]{})
        {

        }

        public ArrayValueType(T[] value) : base(value)
        {

        }
    }
}