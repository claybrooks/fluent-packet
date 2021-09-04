
using SimpleMessage.Components;
using Messaging.Abstractions;
using Messaging.Data;
using Messaging.Serializer;

namespace SimpleMessage
{
    public class SimpleMessageBuilder : Builder<SimpleMessage>
    {
        public static SimpleMessage ConstructFromConfig(string file, char delimiter = '|')
        {
            return new Builder<SimpleMessage>()
                .FromConfig(new JsonConfigReader(), file);
        }

        public static SimpleMessage Construct(char delimiter = '|')
        {
            return new Builder<SimpleMessage>()
                .WithData<byte>(0x02)
                .WithData(delimiter)
                .WithData(0, (long)SimpleMessage.Tags.DeviceId)
                .WithData(delimiter)
                .WithData(false, (long)SimpleMessage.Tags.DeviceEnabled)
                .WithData(delimiter)
                .WithData(0, (long)SimpleMessage.Tags.DeviceState)
                .WithData(delimiter)
                .WithData("", (long)SimpleMessage.Tags.DeviceName)
                .WithData(delimiter)
                .WithData(new SiteInfo(), (long)SimpleMessage.Tags.SiteInfo)
                .WithData(delimiter)
                .WithData(new VendorInfo(), (long)SimpleMessage.Tags.VendorInfo)
                .WithData(delimiter)
                .WithData(new CompositeInfo(), (long)SimpleMessage.Tags.CompositeInfo)
                .WithData(delimiter)
                .WithData(new byte[] {1,2,3,4,5}, (long)SimpleMessage.Tags.StatusArray)
                .WithData(delimiter)
                .WithData<byte>(0x03)
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
            StatusArray
        }

        public SimpleMessage()
        {
            Register<SiteInfo>();
            Register<VendorInfo, ReferenceType<VendorInfo>, VendorInfoSerializer>();
            Register<CompositeInfo, ReferenceType<CompositeInfo>, CompositeInfoSerializer>();
            SerializerFactory.Register<string, StringSerializer>(s =>
            {
                s.FixedLength = 10;
            });
            SerializerFactory.Register<bool, BoolSerializer>();
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

        public byte[] StatusArray
        {
            get => GetData<byte[]>((long) Tags.StatusArray);
            set => SetData((long) Tags.StatusArray, value);
        }

        public SimpleMessage WithDeviceId(int id)
        {
            DeviceId = id;
            return this;
        }

        public SimpleMessage WithDeviceEnabled(bool enabled)
        {
            DeviceEnabled = enabled;
            return this;
        }

        public SimpleMessage WithDeviceState(int state)
        {
            DeviceState = state;
            return this;
        }

        public SimpleMessage WithDeviceName(string name)
        {
            DeviceName = name;
            return this;
        }

        public SimpleMessage WithSiteInfo(SiteInfo info)
        {
            SiteInfo = info;
            return this;
        }

        public SimpleMessage WithVendorInfo(VendorInfo info)
        {
            VendorInfo = info;
            return this;
        }

        public SimpleMessage WithCompositeInfo(CompositeInfo info)
        {
            CompositeInfo = info;
            return this;
        }

        public SimpleMessage WithStatusArray(byte[] array)
        {
            StatusArray = array;
            return this;
        }
    }
}
