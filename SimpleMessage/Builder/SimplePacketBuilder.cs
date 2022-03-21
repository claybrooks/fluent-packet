using FluentPacket;
using FluentPacket.Serializer;
using SimplePacket.Components;

namespace SimplePacket
{
    public class SimplePacketBuilder : Builder<SimplePacket>
    {
        public override void Assemble()
        {
            const char delimiter = '|';
            WithData<byte>(0x02)
                .WithData(delimiter)
                .WithData(0, (int)SimplePacket.Tags.DeviceId)
                .WithData(delimiter)
                .WithData(false, (int)SimplePacket.Tags.DeviceEnabled)
                .WithData(delimiter)
                .WithData(0, (int)SimplePacket.Tags.DeviceState)
                .WithData(delimiter)
                .WithData(new char[10], (int)SimplePacket.Tags.DeviceName, new ArrayTypeSerializer<char>(10))
                .WithData(delimiter)
                .WithData(new SiteInfo(), (int)SimplePacket.Tags.SiteInfo)
                .WithData(delimiter)
                .WithData(new VendorInfo(), (int)SimplePacket.Tags.VendorInfo)
                .WithData(delimiter)
                .WithData(new CompositeInfo(), (int)SimplePacket.Tags.CompositeInfo)
                .WithData(delimiter)
                .WithData(new byte[5], (int)SimplePacket.Tags.StatusArray, new ArrayTypeSerializer<byte>(5))
                .WithData(delimiter)
                .WithData(new SiteInfo[5], (int)SimplePacket.Tags.SiteInfoArray, new ArrayTypeSerializer<SiteInfo>(5))
                .WithData(delimiter)
                .WithData<byte>(0x03);
        }
    }
}
