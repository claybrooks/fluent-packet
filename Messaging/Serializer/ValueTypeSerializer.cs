using System.Runtime.InteropServices;

using Messaging.Abstractions;
using Messaging.Interfaces;

namespace Messaging.Serializer
{
    public class ValueTypeSerializer<T> : Serializer<T> where T : struct
    {
        public ValueTypeSerializer(ISerializerFactory factory) : base(factory)
        {

        }

        public override bool Deserialize(ref T value, byte[] data, int offset)
        {
            var length = Marshal.SizeOf(value);

            if (data.Length - offset < length)
            {
                return false;
            }

            var i = Marshal.AllocHGlobal(length);
            Marshal.Copy(data, offset, i, length);
            
            var type = value.GetType();

            var obj = Marshal.PtrToStructure(i, type);

            if (obj == null)
            {
                return false;
            }

            value = (T) obj;

            Marshal.FreeHGlobal(i);

            return true;
        }

        public override byte[] Serialize(T value)
        {
            var len = Marshal.SizeOf(value);
            var arr = new byte[len];

            var ptr = Marshal.AllocHGlobal(len);

            Marshal.StructureToPtr(value, ptr, true);
            Marshal.Copy(ptr, arr, 0, len);
            Marshal.FreeHGlobal(ptr);

            return arr;
        }

        public override int Length()
        {
            return Marshal.SizeOf(default(T));
        }
    }
}
