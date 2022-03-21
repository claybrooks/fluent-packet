using System;
using FluentPacket.Serializer;
using Xunit;

namespace UnitTest.Serializer
{
    public class ValueTypeSerializerTest
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

        [Fact]
        public void Deserialize_SerializeTrue_ExpectTrue()
        {
            BoolSerializer serializer = new BoolSerializer();
            byte[] data = new byte[] { 1 };

            var serialized = serializer.Deserialize(out bool value, data, 0);

            Assert.True(value);
            Assert.True(serialized);
        }

        [Fact]
        public void Deserialize_SerializeFalse_ExpectTrue()
        {
            BoolSerializer serializer = new BoolSerializer();
            byte[] data = new byte[] { 0 };

            var serialized = serializer.Deserialize(out bool value, data, 0);

            Assert.False(value);
            Assert.True(serialized);
        }
    }
}