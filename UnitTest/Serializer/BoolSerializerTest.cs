using System;
using FluentPacket.Serializer;
using Xunit;

namespace UnitTest.Serializer
{
    public class BoolSerializerTest
    {
        [Fact]
        public void Deserialize_NullBuffer_ExpectThrow()
        {
            BoolSerializer serializer = new BoolSerializer();
            Assert.Throws<ArgumentNullException>(() => { serializer.Deserialize(out bool value, null, 1); });
        }

        [Fact]
        public void Deserialize_OffsetBeyondLength_ExpectThrow()
        {
            BoolSerializer serializer = new BoolSerializer();
            byte[] data = new byte[] { 1 };

            Assert.Throws<ArgumentOutOfRangeException>(() => { serializer.Deserialize(out bool value, data, 1); });
        }

        [Theory]
        [InlineData(1)]
        [InlineData(0)]
        public void Deserialize(byte value)
        {
            BoolSerializer serializer = new BoolSerializer();
            byte[] data = new byte[] { value };

            var serialized = serializer.Deserialize(out bool deserializedValue, data, 0);

            Assert.Equal(value == 1 ? true : false, deserializedValue);
            Assert.True(serialized);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Serialize(bool value)
        {
            BoolSerializer serializer = new BoolSerializer();
            var bytes = serializer.Serialize(value);
            Assert.Single(bytes);
            Assert.Equal(value ? 1 : 0, bytes[0]);
        }

        [Fact]
        public void Length()
        {
            BoolSerializer serializer = new BoolSerializer();
            Assert.Equal(1, serializer.Length());
        }
    }
}