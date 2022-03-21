using System;
using System.Runtime.InteropServices;

namespace FluentPacket.Serializer
{
    public class ValueTypeSerializer<T> : Serializer<T>
        where T : struct
    {
        public override bool Deserialize(out T value, byte[] data, int offset)
        {
            var length = Length();

            if (data.Length - offset < length)
            {
                throw new ArgumentOutOfRangeException(nameof(offset));
            }

            var i = Marshal.AllocHGlobal(length);
            Marshal.Copy(data, offset, i, length);

            var obj = Marshal.PtrToStructure(i, typeof(T));

            if (obj == null)
            {
                throw new InvalidCastException($"Unable to cast byte data to {nameof(T)}");
            }

            value = (T)obj;

            Marshal.FreeHGlobal(i);

            return true;
        }

        public override byte[] Serialize(T value)
        {
            var len = Length();
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
