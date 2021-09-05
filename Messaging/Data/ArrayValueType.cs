using Messaging.Abstractions;

namespace Messaging.Data
{
    public class ArrayValueType<T> : Data<T[]> where T : struct
    {
        public ArrayValueType(T[] value) : base(value)
        {

        }

        public override void Clear()
        {
            for (int i = 0; i < Value.Length; ++i)
            {
                Value[i] = default(T);
            }
        }
    }
}