using SimpleMessage;
using SimpleMessage.Components;

using System;
using System.Text;

namespace Messaging
{
    internal class Program
    {
        private static void Main()
        {
            var message = Builder.Construct('}')
                .WithDeviceName("DEVICE_BBB")
                .WithDeviceEnabled(true)
                .WithDeviceId(6)
                .WithDeviceState(2)
                .WithVendorInfo(new VendorInfo() { Enabled = false, VendorId = 0xDEAD })
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


            message.Serialize(out var bytes);

            var message2 = Builder.Construct();

            message2.Deserialize(bytes);


            Console.WriteLine($"[{string.Join(",", bytes)}]");
            Console.WriteLine($"{Encoding.ASCII.GetString(bytes)}");
            Console.ReadKey();
        }
    }
}