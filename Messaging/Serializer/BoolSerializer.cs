using System;
using Messaging.Abstractions;

namespace Messaging.Serializer
{
    public class BoolSerializer : Serializer<bool>
    {
        public override bool Deserialize(ref bool value, byte[] data, int offset)
        {
            if (data.Length - offset < 1)
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