
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

        public int DeviceId
        {
            get => GetData<int>((long) Tags.DeviceId);
            set => SetData((long) Tags.DeviceId, value);
        }

        public bool DeviceEnabled
        {
            get => GetData<bool>((long)Tags.DeviceEnabled);
            set => SetData((long)Tags.DeviceEnabled, value);
        }

        public int DeviceState
        {
            get => GetData<int>((long)Tags.DeviceState);
            set => SetData((long)Tags.DeviceState, value);
        }

        public string DeviceName
        {
            get => GetData<string>((long)Tags.DeviceName);
            set => SetData((long)Tags.DeviceName, value);
        }

        public SiteInfo SiteInfo
        {
            get => GetData<SiteInfo>((long)Tags.SiteInfo);
            set => SetData((long)Tags.SiteInfo, value);
        }

        public VendorInfo VendorInfo
        {
            get => GetData<VendorInfo>((long)Tags.VendorInfo);
            set => SetData((long)Tags.VendorInfo, value);
        }

        public CompositeInfo CompositeInfo
        {
            get => GetData<CompositeInfo>((long)Tags.CompositeInfo);
            set => SetData((long)Tags.CompositeInfo, value);
        }

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
