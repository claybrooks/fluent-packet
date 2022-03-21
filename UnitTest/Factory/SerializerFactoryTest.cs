using System;
using System.Collections.Generic;
using FluentPacket.Factory;
using FluentPacket.Interfaces;
using FluentPacket.Serializer;
using Moq;
using Xunit;

namespace UnitTest.Factory
{

    public class SerializerFactoryUnitTest
    {
        public class TestDataClass
        {

        }

        public class TestDataClassSerializer : Serializer<TestDataClass>
        {
            public override bool Deserialize(out TestDataClass value, byte[] data, int offset)
            {
                value = new TestDataClass();
                return true;
            }

            public override byte[] Serialize(TestDataClass value)
            {
                return new byte[] { 0 };
            }

            public override int Length()
            {
                return 1;
            }
        }

        [Fact]
        public void NewFactory_ExpectNoThrow()
        {
            Assert.NotNull(GetSerializerFactory());
        }

        [Fact]
        public void Factory_NotRegistered_Get_ExpectThrow()
        {
            Assert.Throws<KeyNotFoundException>(() => GetSerializerFactory().Get<int>());
        }

        [Fact]
        public void Factory_Register_ExpectNoThrow()
        {
            var factory = GetSerializerFactory();

            factory.Register<TestDataClass, TestDataClassSerializer>();
            Assert.NotNull(factory.Get<TestDataClass>());
        }

        [Fact]
        public void Factory_RegisterTwice_ExpectNotThrowAndInitializerActionNotCalledOnSecondRegister()
        {
            var factory = GetSerializerFactory();

            factory.Register<TestDataClass, TestDataClassSerializer>();
            Assert.NotNull(factory.Get<TestDataClass>());

            factory.Register<TestDataClass, TestDataClassSerializer>();
        }

        #region PrivateHelpers

        private static SerializerFactory GetSerializerFactory()
        {
            return new SerializerFactory();
        }

        private static Mock<Action<ISerializer<T>>> GetSerializerInitializerMock<T>()
         where T : class, new()
        {
            var mock = new Mock<Action<ISerializer<T>>>();
            mock.Setup(x => x.Invoke(It.IsAny<ISerializer<T>>()));
            return mock;
        }

        private static void AssertSerializerInitializerInvokedOnce<T>(Mock<Action<ISerializer<T>>> mock)
            where T : class
        {
            mock.Verify(x => x.Invoke(It.IsAny<ISerializer<T>>()), Times.Once());
        }

        private static void AssertSerializerInitializerNotInvoked<T>(Mock<Action<ISerializer<T>>> mock)
            where T : class
        {
            mock.Verify(x => x.Invoke(It.IsAny<ISerializer<T>>()), Times.Never());
        }

        #endregion
    }
}