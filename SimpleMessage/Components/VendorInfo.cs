using System;

using Messaging.Abstractions;
using Messaging.Data;
using Messaging.Interfaces;

namespace SimpleMessage.Components
{
    public class VendorInfo
    {
        public int VendorId;
        public bool Enabled;
    }

    public class VendorInfoSerializer : Serializer<VendorInfo>
    {
        public VendorInfoSerializer(ISerializerFactory factory) : base(factory)
        {

        }

        public override bool Deserialize(ref VendorInfo value, byte[] data, int offset)
        {
            if (data[offset + 4] != (byte)',')
            {
                return false;
            }

            if (data.Length - offset < 6)
            {
                return false;
            }

            value.VendorId = BitConverter.ToInt32(data, offset);
            value.Enabled = BitConverter.ToBoolean(data, 5);
            return true;
        }

        public override byte[] Serialize(VendorInfo value)
        {
            byte[] data = new byte[6];

            byte[] serializedVendorId = BitConverter.GetBytes(value.VendorId);
            serializedVendorId.CopyTo(data, 0);
            data[4] = Convert.ToByte(',');
            data[5] = Convert.ToByte(value.Enabled);

            return data;
        }

        public override int Length()
        {
            return 6;
        }
    }

    public class VendorInfoType : ReferenceType<VendorInfo, VendorInfoSerializer>
    {
        public VendorInfoType(ISerializerFactory factory) : this(factory, new VendorInfo())
        {

        }

        public VendorInfoType(ISerializerFactory factory, VendorInfo info) : base(factory, info ?? new VendorInfo())
        { 

        }
    }
}
