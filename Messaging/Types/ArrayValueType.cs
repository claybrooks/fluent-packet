using Messaging.Interfaces;
using Messaging.Serializer;

namespace Messaging.Types
{
    public class ArrayValueType<T> : Data<T[]>
        where T : struct
    {
        public ArrayValueType(T[] value) : base(value, new ArrayTypeSerializer<T>(value.Length))
        {

        }

        public ArrayValueType(T[] value, ISerializer<T[]> serializer) : base(value, serializer)
        {

        }
        
        public override void Clear()
        {
            for (var i = 0; i < Value.Length; ++i)
            {
                Value[i] = default;
            }
        }
    }
}