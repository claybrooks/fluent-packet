using System;

using Messaging.Abstractions;
using Messaging.Data;
using Messaging.Interfaces;

namespace SimpleMessage.Components
{
    public class CompositeInfo
    {
        public int CompositeId;
        public SiteInfo SiteInfo;
        public VendorInfo VendorInfo = new VendorInfo();
    }

    public class CompositeInfoSerializer : Serializer<CompositeInfo>
    {
        public CompositeInfoSerializer(ISerializerFactory factory) : base(factory)
        {

        }

        public override bool Deserialize(ref CompositeInfo value, byte[] data, int offset)
        {
            if (data.Length - offset < Length())
            {
                return false;
            }

            value.CompositeId = BitConverter.ToInt32(data, offset);
            Factory.Get<SiteInfo>().Deserialize(ref value.SiteInfo, data, offset + 4);
            Factory.Get<VendorInfo>().Deserialize(ref value.VendorInfo, data, offset + 4 + Factory.Get<SiteInfo>().Length());

            return true;
        }

        public override byte[] Serialize(CompositeInfo value)
        {
            var vendorInfoSerializer = Factory.Get<VendorInfo>();
            var siteInfoSerializer = Factory.Get<SiteInfo>();

            byte[] data = new byte[Length()];

            byte[] vendorBytes = vendorInfoSerializer.Serialize(value.VendorInfo);
            byte[] siteBytes = siteInfoSerializer.Serialize(value.SiteInfo);

            BitConverter.GetBytes(value.CompositeId).CopyTo(data, 0);
            siteBytes.CopyTo(data, 4);
            vendorBytes.CopyTo(data, 4 + siteInfoSerializer.Length());

            return data;
        }

        public override int Length()
        {
            return 4 + Factory.Get<SiteInfo>().Length() + Factory.Get<VendorInfo>().Length();
        }
    }

    public class CompositeInfoType : ReferenceType<CompositeInfo, CompositeInfoSerializer>
    {
        public CompositeInfoType(ISerializerFactory factory) : this(factory, new CompositeInfo()) { }
        public CompositeInfoType(ISerializerFactory factory, CompositeInfo info) : base(factory, info ?? new CompositeInfo()) { }
    }
}
