using System;
using System.Linq;
using System.Text;

namespace FluentPacket.Serializer
{
    public class StringSerializer : Serializer<string>
    {
        private readonly int _fixedLength;

        public StringSerializer(int fixedLength)
        {
            _fixedLength = fixedLength;
        }

        public override bool Deserialize(out string value, byte[] data, int offset)
        {
            if (data.Length - offset < _fixedLength)
            {
                throw new ArgumentOutOfRangeException(nameof(offset));
            }

            value = Encoding.ASCII.GetString(data, offset, _fixedLength);
            return true;
        }

        public override byte[] Serialize(string value)
        {
            byte[] data = Enumerable.Repeat<byte>(0x20, _fixedLength).ToArray();
            Encoding.ASCII.GetBytes(value).CopyTo(data, 0);

            return data;
        }

        public override int Length()
        {
            return _fixedLength;
        }
    }
}
