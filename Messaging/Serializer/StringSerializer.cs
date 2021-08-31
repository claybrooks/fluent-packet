using System.Text;

using Messaging.Abstractions;

namespace Messaging.Serializer
{
    public class StringSerializer : Serializer<string>
    {
        public int FixedLength { get; set; }

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
