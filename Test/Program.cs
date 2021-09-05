using System;
using Messaging.Abstractions;
using SimpleMessage;
using SimpleMessage.Components;

namespace Test
{
    internal class Program
    {
        private static void Main()
        {
            TestCodeBuilder();
            TestConfigBuilder();
            
            Console.ReadKey();
        }

        private static void TestCodeBuilder()
        {
            var packet = SimpleMessageBuilder.Construct('}')
                .WithDeviceName("DEVICE_BBB".ToCharArray())
                .WithDeviceEnabled(true)
                .WithDeviceId(6)
                .WithDeviceState(2)
                .WithVendorInfo(new VendorInfo() { Enabled = false, VendorId = 0xDEAD })
                .WithSiteInfo(new SiteInfo() { SiteId = 0xBEEF, Enabled = true })
                .WithStatusArray(new byte[5] {5,4,3,2,1})
                .WithSiteInfo(2, new SiteInfo() {  SiteId = 9, Enabled = true })
                .WithCompositeInfo(new CompositeInfo()
                {
                    CompositeId = 0xDEAD,
                    SiteInfo = new SiteInfo()
                    {
                        Enabled = true,
                        SiteId = 0x01
                    },
                    VendorInfo = new VendorInfo()
                    {
                        Enabled = false,
                        VendorId = 0x02
                    }
                });

            PrintPacket(packet);

            packet.Clear();

            PrintPacket(packet);
        }

        private static void TestConfigBuilder()
        {
            var packet = SimpleMessageBuilder.ConstructFromConfig("Configs/SimpleMessage.json", ':')
                .WithDeviceName("DEVICE_BBB".ToCharArray())
                .WithDeviceEnabled(true)
                .WithDeviceId(6)
                .WithDeviceState(2)
                .WithSiteInfo(new SiteInfo() { SiteId = 0xBEEF, Enabled = true })
                .WithCompositeInfo(new CompositeInfo()
                {
                    CompositeId = 0xDEAD,
                    SiteInfo = new SiteInfo()
                    {
                        Enabled = true,
                        SiteId = 0x01
                    },
                    VendorInfo = new VendorInfo()
                    {
                        Enabled = false,
                        VendorId = 0x02
                    }
                });

            PrintPacket(packet);
        }

        private static void PrintPacket(Packet packet)
        {
            packet.Serialize(out var bytes);
            Console.WriteLine($"[{string.Join(",", bytes)}]");
        }
    }
}