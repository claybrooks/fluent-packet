
using SimpleMessage.Components;
using Messaging.Abstractions;

namespace SimpleMessage
{
    public class Builder : Builder<SimpleMessage>
    {
        public static SimpleMessage Construct(char delimiter = '|')
        {
            return new Builder<SimpleMessage>()
                .WithData<byte>(0x02)
                .WithData(delimiter)
                .WithData<int>((long)SimpleMessage.Tags.DeviceId)
                .WithData(delimiter)
                .WithData<bool>((long)SimpleMessage.Tags.DeviceEnabled)
                .WithData(delimiter)
                .WithData<int>((long)SimpleMessage.Tags.DeviceState)
                .WithData(delimiter)
                .WithData<string>((long)SimpleMessage.Tags.DeviceName)
                .WithData(delimiter)
                .WithData<SiteInfo>((long)SimpleMessage.Tags.SiteInfo)
                .WithData(delimiter)
                .WithData<VendorInfo>((long)SimpleMessage.Tags.VendorInfo)
                .WithData(delimiter)
                .WithData<CompositeInfo>((long)SimpleMessage.Tags.CompositeInfo)
                .WithData(delimiter)
                .WithData<byte>(0x03)
                .Build();
        }
        public static SimpleMessage ConstructHumanReadable(char delimiter = '|')
        {
            return new Builder<SimpleMessage>()
                .WithData("<STX>")
                .WithData(delimiter)
                .WithData("<DEVICE_ID>")
                .WithData(delimiter)
                .WithData("<DEVICE_ENABLED>")
                .WithData(delimiter)
                .WithData("<DEVICE_STATE>")
                .WithData(delimiter)
                .WithData("<DEVICE_NAME>")
                .WithData(delimiter)
                .WithData("<SITE_INFO>")
                .WithData(delimiter)
                .WithData("<VENDOR_INFO>")
                .WithData(delimiter)
                .WithData("<COMPOSITE_INFO>")
                .WithData(delimiter)
                .WithData("<ETX>")
                .WithData(delimiter)
                .Build();
        }
    }

    public class SimpleMessage : Packet
    {
        public enum Tags : long
        {
            DeviceId,
            DeviceEnabled,
            DeviceState,
            DeviceName,
            SiteInfo,
            VendorInfo,
            CompositeInfo,
        }

        public SimpleMessage()
        {
            DataFactory.Register<VendorInfo, VendorInfoType>();
            DataFactory.Register<CompositeInfo, CompositeInfoType>();
        }

        public int DeviceId => GetData<int>((long)Tags.DeviceId);
        public bool DeviceEnabled => GetData<bool>((long)Tags.DeviceEnabled);
        public int DeviceState => GetData<int>((long)Tags.DeviceState);
        public string DeviceName => GetData<string>((long)Tags.DeviceName);
        public SiteInfo SiteInfo => GetData<SiteInfo>((long)Tags.SiteInfo);
        public VendorInfo VendorInfo => GetData<VendorInfo>((long)Tags.VendorInfo);
        public CompositeInfo CompositeInfo => GetData<CompositeInfo>((long)Tags.CompositeInfo);

        public SimpleMessage WithDeviceId(int id)
        {
            SetData((long)Tags.DeviceId, id);
            return this;
        }

        public SimpleMessage WithDeviceEnabled(bool enabled)
        {
            SetData((long)Tags.DeviceEnabled, enabled);
            return this;
        }

        public SimpleMessage WithDeviceState(int state)
        {
            SetData((long)Tags.DeviceState, state);
            return this;
        }

        public SimpleMessage WithDeviceName(string name)
        {
            SetData((long)Tags.DeviceName, name);
            return this;
        }

        public SimpleMessage WithSiteInfo(SiteInfo info)
        {
            SetData((long)Tags.SiteInfo, info);
            return this;
        }

        public SimpleMessage WithVendorInfo(VendorInfo info)
        {
            SetData((long)Tags.VendorInfo, info);
            return this;
        }

        public SimpleMessage WithCompositeInfo(CompositeInfo info)
        {
            SetData((long)Tags.CompositeInfo, info);
            return this;
        }
    }
}
