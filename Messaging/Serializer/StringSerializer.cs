using System.Linq;
using System.Text;

namespace Messaging.Serializer
{
    public class StringSerializer : Serializer<string>
    {
        private readonly int _fixedLength;

        public StringSerializer(int fixedLength)
        {
            _fixedLength = fixedLength;
        }

        public override bool Deserialize(ref string value, byte[] data, int offset)
        {
            if (data.Length - offset < _fixedLength)
            {
                return false;
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
