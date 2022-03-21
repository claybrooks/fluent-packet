using System;
using System.Runtime.InteropServices;
using FluentPacket.Serializer;
using Moq;
using Xunit;

namespace UnitTest.Serializer
{
    public class SerializerTest
    {
        [Fact]
        public void Serialize_NullBuffer_ExpectThrow()
        {
            var mockedSerializer = GetSerializer<int>();
            Assert.Throws<ArgumentNullException>(() => { mockedSerializer.Object.Serialize(5, null, 0); });
        }

        [Theory]
        [InlineData(3, 0)]
        [InlineData(4, 1)]
        public void Serialize_BadOffset_ExpectThrow(int size, int offset)
        {
            var mockedSerializer = GetSerializer<int>();
            var bytes = new byte[size];
            Assert.Throws<ArgumentOutOfRangeException>(() => { mockedSerializer.Object.Serialize(1, bytes, offset); });
        }

        [Fact]
        public void Serialize()
        {
            var mockedSerializer = GetSerializer<int>();
            mockedSerializer.Setup(s => s.Serialize(It.IsAny<int>())).Returns(new byte[] { 0, 0, 0, 1 });
            var bytes = new byte[] { 0, 0, 0, 0, 0, 0 };

            var serializedLength = mockedSerializer.Object.Serialize(1, bytes, 1);

            Assert.Equal(mockedSerializer.Object.Length(), serializedLength);
            Assert.Equal(new byte[] { 0, 0, 0, 0, 1, 0 }, bytes);
        }

        #region TestInternals

        Mock<Serializer<T>> GetSerializer<T>()
            where T : struct
        {
            Mock<Serializer<T>> serializer = new Mock<Serializer<T>>();
            serializer.Setup(s => s.Length()).Returns(Marshal.SizeOf(default(T)));

            return serializer;
        }

        #endregion
    }
}