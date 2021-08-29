using Messaging.Interfaces;
using Messaging.Serializer;

namespace Messaging.Data
{
    public class String : ReferenceType<string, StringSerializer>
    {
        private readonly int _fixedLength = 20;

        public String(ISerializerFactory factory) : this(factory, "")
        {
            Serializer.FixedLength = _fixedLength;
        }

        public String(ISerializerFactory factory, string value) : base(factory, value)
        {
            Serializer.FixedLength = _fixedLength;
        }

        public String(ISerializerFactory factory, string value, int fixedLength) : base(factory, value)
        {
            _fixedLength = fixedLength;
            Serializer.FixedLength = _fixedLength;
        }
    }
}
