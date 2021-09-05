using Messaging.Abstractions;

namespace Messaging.Data
{
    public class ValueType<T> : Data<T> where T : struct
    {
        public ValueType() : this(default(T))
        {

        }

        public ValueType(T value) : base(value)
        {

        }

        public override void Clear()
        {
            Value = default(T);
        }
    }
}
