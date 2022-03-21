**Fluent Packet**

#

Getting Started
```csharp
/**
 * Uses the default packet builder to easily build a data packet.
 */
using FluentPacket.Builder;

DefaultPacket message = new DefaultPacketBuilder()
    .WithData<byte>(9)
    .WithData(new byte[] { 3, 2, 1 })
    .WithData<short>(2)
    .Build();

packet.Serialize(out var bytes);

// Prints 9,3,2,1,2,0
Console.WriteLine(string.Join(",",bytes));
```
#

Defining Custom Packets
```csharp
/**
 * Now lets do something a little more robust
 */

// SimplePacket.cs
public class SimplePacket : FluentPacket.DefaultPacket
{
    // Tags are identifiers for data used in conjuction with SetData and GetData
    // The intent is to wrap your packet definitions in an implementation of DefaultPacket,
    // so usage of Tags should be relegated to the wrapping class.
    public enum Tags
    {
        DeviceId,
        DeviceEnabled
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
}

// SimplePacketBuilder.cs
public class SimplePacketBuilder : FluentPacket.Builder<SimplePacket>
{
    // SimplePacket Format: STX|DEVICE_ID|DEVICE_ENABLED|ETX

    // Define your packet specification here.  The hardcoded '|', 0x02, and 0x03
    // don't need to be exposed to the user of a SimplePacket.  Just bake them in
    // to the definition of the packet and forget about it.
    public override void Assemble()
    {
        const char delimiter = '|';
        WithData<byte>(0x02)
            .WithData(delimiter)
            .WithData(0, (int)SimplePacket.Tags.DeviceId)
            .WithData(delimiter)
            .WithData(false, (int)SimplePacket.Tags.DeviceEnabled)
            .WithData(delimiter)
            .WithData<byte>(0x03);
    }
}

// Program .cs
SimplePacket message = new SimplePacketBuilder()
    .Produce()
        .WithDeviceId(42)
        .WithDeviceEnabled(false)
        .Build();

packet.Serialize(out var bytes);

// Prints 2,124,42,0,0,0,124,0,124,3
Console.WriteLine(string.Join(",",bytes));
```