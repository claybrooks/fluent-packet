using Messaging;
using SimplePacket.Components;

namespace SimplePacket
{
    public class SimplePacket : DefaultPacket
    {
        public enum Tags
        {
            DeviceId,
            DeviceEnabled,
            DeviceState,
            DeviceName,
            SiteInfo,
            VendorInfo,
            CompositeInfo,
            StatusArray,
            SiteInfoArray
        }

        public SimplePacket()
        {
            Register<SiteInfo>();
            Register<VendorInfo, VendorInfoSerializer>();
            Register<CompositeInfo, CompositeInfoSerializer>();
        }

        public int DeviceId
        {
            get => GetData<int>((int)Tags.DeviceId);
            set => SetData((int)Tags.DeviceId, value);
        }

        public bool DeviceEnabled
        {
            get => GetData<bool>((int)Tags.DeviceEnabled);
            set => SetData((int)Tags.DeviceEnabled, value);
        }

        public int DeviceState
        {
            get => GetData<int>((int)Tags.DeviceState);
            set => SetData((int)Tags.DeviceState, value);
        }

        public char[] DeviceName
        {
            get => GetData<char[]>((int)Tags.DeviceName);
            set => SetData((int)Tags.DeviceName, value);
        }

        public SiteInfo SiteInfo
        {
            get => GetData<SiteInfo>((int)Tags.SiteInfo);
            set => SetData((int)Tags.SiteInfo, value);
        }

        public VendorInfo VendorInfo
        {
            get => GetData<VendorInfo>((int)Tags.VendorInfo);
            set => SetData((int)Tags.VendorInfo, value);
        }

        public CompositeInfo CompositeInfo
        {
            get => GetData<CompositeInfo>((int)Tags.CompositeInfo);
            set => SetData((int)Tags.CompositeInfo, value);
        }

        public byte[] StatusArray
        {
            get => GetData<byte[]>((int)Tags.StatusArray);
            set => SetData((int)Tags.StatusArray, value);
        }

        public SiteInfo[] SiteInfoArray
        {
            get => GetData<SiteInfo[]>((int)Tags.SiteInfoArray);
            set => SetData((int)Tags.SiteInfoArray, value);
        }

        public SimplePacket WithDeviceId(int id)
        {
            DeviceId = id;
            return this;
        }

        public SimplePacket WithDeviceEnabled(bool enabled)
        {
            DeviceEnabled = enabled;
            return this;
        }

        public SimplePacket WithDeviceState(int state)
        {
            DeviceState = state;
            return this;
        }

        public SimplePacket WithDeviceName(char[] name)
        {
            DeviceName = name;
            return this;
        }

        public SimplePacket WithSiteInfo(SiteInfo info)
        {
            SiteInfo = info;
            return this;
        }

        public SimplePacket WithVendorInfo(VendorInfo info)
        {
            VendorInfo = info;
            return this;
        }

        public SimplePacket WithCompositeInfo(CompositeInfo info)
        {
            CompositeInfo = info;
            return this;
        }

        public SimplePacket WithStatusArray(byte[] array)
        {
            StatusArray = array;
            return this;
        }

        public SimplePacket WithSiteInfo(int index, SiteInfo info)
        {
            SiteInfoArray[index] = info;
            return this;
        }
    }
}
