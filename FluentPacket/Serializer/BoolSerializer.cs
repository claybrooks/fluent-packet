using System;

namespace FluentPacket.Serializer
{
    public class BoolSerializer : Serializer<bool>
    {
        public override bool Deserialize(out bool value, byte[] data, int offset)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (offset >= data.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(data));
            }

            value = BitConverter.ToBoolean(data, offset);
            return true;
        }

        public override byte[] Serialize(bool value)
        {
            return new[] { (byte)(value ? 1 : 0) };
        }

        public override int Length()
        {
            return 1;
        }
    }
}