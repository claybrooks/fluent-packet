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
#
```csharp
// FluentPacket supports automatic serialization of value types
using FluentPacket.Builder;

struct MyValueType
{
    int a;
    short b;
    char c;
}

DefaultPacket message = new DefaultPacketBuilder()
    .WithData<MyValueType>(new MyValueType(){
        a=1,
        b=2,
        c=3,
    })
    .Build();

packet.Serialize(out var bytes);

// Prints 1,0,0,0,2,0,3
Console.WriteLine(string.Join(",",bytes));
```
#
```csharp
// FluentPacket supports custom serializers
using FluentPacket.Builder;

struct MyValueType
{
    int a;
    short b;
    char c;
}

public class MyValueTypeSerializer : Serializer<MyValueType>
{
    public MyValueTypeSerializer()
    {
    }

    public override bool Deserialize(out MyValueType value, byte[] data, int offset)
    {
        // Populate value with data
    }

    public override byte[] Serialize(MyValueType value)
    {
        // Build custom buffer from value
    }

    public override int Length()
    {
        // Report the length of the serialized buffer
    }
}

DefaultPacket message = new DefaultPacketBuilder()
    .WithData<MyValueType>(new MyValueType(){a=1,b=2,c=3}, new MyValueTypeSerializer())
    .Build();

packet.Serialize(out var bytes);
```
#
```csharp
// You can register a serializer to be reused
// Non value type classes must have a serializer registered

class MyReferenceType
{
    int a;
    short b;
    char c;
}

public class MyReferenceTypeSerializer : Serializer<MyReferenceType>
{
    public MyReferenceTypeSerializer()
    {
    }

    public override bool Deserialize(out MyReferenceType value, byte[] data, int offset)
    {
        // Populate value with data
    }

    public override byte[] Serialize(MyReferenceType value)
    {
        // Build custom buffer from value
    }

    public override int Length()
    {
        // Report the length of the serialized buffer
    }
}

public class SimplePacket : FluentPacket.DefaultPacket
{
    SimplePacket()
    {
        // Now, anywhere a "MyReferenceType" is to be used, the serializer will
        // be invoked
        Register<MyReferenceType, MyReferenceTypeSerializer>();
    }
}
```
#
```csharp
// If there is a reference type embedded within another type, a custom serializer
// must be created
class ReferenceTypeA
{
    int x;
}

// ReferenceTypeB must register a serializer and define how to (de)serialize ReferenceTypeA
class ReferenceTypeB
{
    ReferenceTypeA a;
}

public class ReferenceTypeASerializer : Serializer<ReferenceTypeA>
{
    public ReferenceTypeASerializer()
    {
    }

    public override bool Deserialize(out ReferenceTypeA value, byte[] data, int offset)
    {
        // Populate value with data
    }

    public override byte[] Serialize(ReferenceTypeA value)
    {
        // Build custom buffer from value
    }

    public override int Length()
    {
        // Report the length of the serialized buffer
    }
}

// If reference type A has a custom serializer, you can get access to it lazily
// It must be done lazily, ordering of serializer registering is not guaranteed
public class ReferenceTypeBSerializer : Serializer<ReferenceTypeB>
{
    private readonly Lazy<ISerializer<ReferenceTypeA>> _referenceTypeASerializer;

    public ReferenceTypeBSerializer()
    {
        _referenceTypeASerializer = new Lazy<ISerializer<ReferenceTypeA>>(() => _factory.Get<ReferenceTypeA>());
    }

    public override bool Deserialize(out ReferenceTypeB value, byte[] data, int offset)
    {
        // User _referenceTypeASerializer to facilitate deserializing
        _referenceTypeASerializer.Value.Deserialize(out value.a, data, 0 /*offset within the buffer*/);
    }

    public override byte[] Serialize(ReferenceTypeB value)
    {
        var data = new byte[Length()];

        byte[] referenceTypeABytes = _referenceTypeASerializer.Value.Serialize(value.a);
        referenceTypeABytes.CopyTo(data, 0 /*offset*/);

        return data;
    }

    public override int Length()
    {
        // Report the length of the serialized buffer
    }
}
```