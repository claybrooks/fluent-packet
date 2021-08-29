using System.Text;

using Messaging.Abstractions;
using Messaging.Interfaces;

namespace Messaging.Serializer
{
    public class StringSerializer : Serializer<string>
    {
        public int FixedLength;

        public StringSerializer(ISerializerFactory factory) : this(factory, 20)
        {

        }

        public StringSerializer(ISerializerFactory factory, int length) : base(factory)
        {
            FixedLength = length;
        }

        public override bool Deserialize(ref string value, byte[] data, int offset)
        {
            if (data.Length - offset < FixedLength)
            {
                return false;
            }

            value = Encoding.ASCII.GetString(data, offset, FixedLength);
            return true;
        }

        public override byte[] Serialize(string value)
        {
            byte[] data = new byte[FixedLength];
            for(int i = 0; i < data.Length; ++i)
            {
                data[i] = 0x20;
            }
            Encoding.ASCII.GetBytes(value).CopyTo(data, 0);

            return data;
        }

        public override int Length()
        {
            return FixedLength;
        }
    }
}
