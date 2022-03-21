using FluentPacket.Serializer;
using FluentPacket.Types;
using Xunit;

namespace UnitTest.Types
{
    public class ValueTypeTest
    {
        [Fact]
        public void VTWithDefaultSerializer_ClearShouldMakeEverythingZero()
        {
            ValueType<int> avt = new ValueType<int>(5);
            avt.Clear();
            Assert.Equal(default(int), avt.Value);
        }

        [Fact]
        public void VTWithProvidedSerializer_ClearShouldMakeEverythingZero()
        {
            ValueType<int> avt = new ValueType<int>(5, new ValueTypeSerializer<int>());
            avt.Clear();
            Assert.Equal(default(int), avt.Value);
        }
    }
}