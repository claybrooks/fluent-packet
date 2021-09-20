using System;

using Messaging.Types;
using Messaging.Interfaces;
using Messaging.Serializer;

namespace SimplePacket.Components
{
    public class CompositeInfo
    {
        public int CompositeId;
        public SiteInfo SiteInfo;
        public VendorInfo VendorInfo = new VendorInfo();
    }

    public class CompositeInfoSerializer : Serializer<CompositeInfo>
    {
        private readonly Lazy<ISerializer<SiteInfo>> _siteInfoSerializer;
        private readonly Lazy<ISerializer<VendorInfo>> _vendorInfoSerializer;

        public CompositeInfoSerializer()
        {
            _siteInfoSerializer = new Lazy<ISerializer<SiteInfo>>(() => _factory.Get<SiteInfo>());
            _vendorInfoSerializer = new Lazy<ISerializer<VendorInfo>>(() => _factory.Get<VendorInfo>());
        }

        public override bool Deserialize(ref CompositeInfo value, byte[] data, int offset)
        {
            if (data.Length - offset < Length())
            {
                return false;
            }

            value.CompositeId = BitConverter.ToInt32(data, offset);
            _siteInfoSerializer.Value.Deserialize(ref value.SiteInfo, data, offset + 4);
            _vendorInfoSerializer.Value.Deserialize(ref value.VendorInfo, data, offset + 4 + _siteInfoSerializer.Value.Length());

            return true;
        }

        public override byte[] Serialize(CompositeInfo value)
        {
            var data = new byte[Length()];

            var vendorBytes = _vendorInfoSerializer.Value.Serialize(value.VendorInfo);
            var siteBytes = _siteInfoSerializer.Value.Serialize(value.SiteInfo);

            BitConverter.GetBytes(value.CompositeId).CopyTo(data, 0);
            siteBytes.CopyTo(data, 4);
            vendorBytes.CopyTo(data, 4 + _siteInfoSerializer.Value.Length());

            return data;
        }

        public override int Length()
        {
            return 4 + _siteInfoSerializer.Value.Length() + _vendorInfoSerializer.Value.Length();
        }
    }

    /*
    public class CompositeInfoReferenceType : ReferenceType<CompositeInfo>
    {
        public CompositeInfoReferenceType(CompositeInfo info) : base(info, new CompositeInfoSerializer())
        {

        }

        public override void Clear()
        {
            Value.CompositeId = 0;
            Value.SiteInfo = new SiteInfo();
            Value.VendorInfo = new VendorInfo();
        }
    }
    */
}
