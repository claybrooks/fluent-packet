using System;

namespace Messaging.Serializer
{
    public class BoolSerializer : Serializer<bool>
    {
        public override bool Deserialize(ref bool value, byte[] data, int offset)
        {
            if (offset >= data.Length)
            {
                return false;
            }

            value = BitConverter.ToBoolean(data, offset);
            return true;
        }

        public override byte[] Serialize(bool value)
        {
            return new[] { (byte) (value ? 1 : 0) };
        }

        public override int Length()
        {
            return 1;
        }
    }
}