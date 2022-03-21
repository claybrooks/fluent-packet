using FluentPacket.Serializer;
using System;

namespace SimplePacket.Components
{
    public class VendorInfo
    {
        public VendorInfo()
        {

        }
        public VendorInfo(int vendorId, bool enabled)
        {
            VendorId = vendorId;
            Enabled = enabled;
        }

        public int VendorId;
        public bool Enabled;
    }

    public class VendorInfoSerializer : Serializer<VendorInfo>
    {
        public override bool Deserialize(out VendorInfo value, byte[] data, int offset)
        {
            value = new VendorInfo();

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
            var data = new byte[6];

            var serializedVendorId = BitConverter.GetBytes(value.VendorId);
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
}
