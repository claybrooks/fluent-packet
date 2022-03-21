using FluentPacket.Serializer;
using FluentPacket.Types;
using Moq;
using System;
using Xunit;

namespace UnitTest.Types
{
    public class ReferenceTypeTest
    {
        internal class TestType
        { }

        internal class TestTypeSerializer : Serializer<TestType>
        {
            public override bool Deserialize(out TestType value, byte[] data, int offset)
            {
                value = new TestType();
                return true;
            }

            public override int Length()
            {
                return 0;
            }

            public override byte[] Serialize(TestType value)
            {
                return null;
            }
        }


        [Fact]
        public void RTW_ClearShouldInvokeClearStrategyOnlyOnce()
        {
            ReferenceType<TestType> avt = new ReferenceType<TestType>(new TestType(), new TestTypeSerializer());

            Mock<Action> clearStrategyMock = new Mock<Action>();
            clearStrategyMock.Setup(m => m.Invoke());
            avt.ClearStrategy = clearStrategyMock.Object;

            avt.Clear();

            clearStrategyMock.Verify(m => m.Invoke(), Times.Once);
        }
    }
}