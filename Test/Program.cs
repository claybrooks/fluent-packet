using System;
using Messaging;
using Messaging.Builder;
using SimplePacket.Components;

namespace Test
{
    internal class Program
    {
        private static void Main()
        {
            TestDefaultPacketBuilder();
            TestSimpleMessageBuilder();
            TestSimpleMessageConfigBuilder();
            
            Console.ReadKey();
        }

        private static void TestDefaultPacketBuilder()
        {
            var message = new DefaultPacketBuilder()
                .WithData<byte>(9)
                .WithData(new byte[] { 3, 2, 1 })
                .WithData<short>(2)
                .Build();

            PrintPacket(message);
        }

        private static void TestSimpleMessageBuilder()
        {
            SetSimplePacketDataAndPrint(new SimplePacket.Builder.Builder().Produce());
        }

        private static void TestSimpleMessageConfigBuilder()
        {
            SetSimplePacketDataAndPrint(new SimplePacket.Builder.ConfigBuilder("Configs/SimpleMessage.json").Produce());
        }

        private static void SetSimplePacketDataAndPrint(SimplePacket.SimplePacket packet)
        {
            packet.WithDeviceName("DEVICE_BBB".ToCharArray())
                .WithDeviceEnabled(true)
                .WithDeviceId(6)
                .WithDeviceState(2)
                .WithVendorInfo(new VendorInfo() { Enabled = false, VendorId = 0xDEAD })
                .WithSiteInfo(new SiteInfo() { SiteId = 0xBEEF, Enabled = true })
                .WithStatusArray(new byte[] { 5, 4, 3, 2, 1 })
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
            packet.ClearTagged();
        }

        private static void PrintPacket(Packet packet)
        {
            packet.Serialize(out var bytes);
            Console.WriteLine($"[{string.Join(",", bytes)}]");
        }
    }
}