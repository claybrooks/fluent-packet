using System;
using System.Collections.Generic;
using FluentPacket.Factory;
using FluentPacket.Serializer;
using FluentPacket.Types;
using Xunit;

namespace UnitTest.Factory
{
    public class DataFactoryUnitTest
    {
        internal class TestDataClass
        {

        }

        internal class TestDataClassSerializer : Serializer<TestDataClass>
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
        public void NewFactory_NullSerializer_ExpectThrow()
        {
            Assert.Throws<ArgumentNullException>(() => new DataFactory(null!));
        }

        [Fact]
        public void NewFactory_NonNullSerializer_ExpectNoThrow()
        {
            Assert.NotNull(GetDataFactory());
        }

        [Fact]
        public void Create_NullValue_ExpectThrow()
        {
            Assert.Throws<ArgumentNullException>(() => GetDataFactory().Create<string>(null));
        }

        [Fact]
        public void Create_NotRegistered_ExpectThrow()
        {
            Assert.Throws<KeyNotFoundException>(() => GetDataFactory().Create(""));
        }

        [Fact]
        public void Create_NotRegisteredInSerializerFactory_ExpectThrow()
        {
            var factory = GetDataFactory();

            factory.Register<TestDataClass, ReferenceType<TestDataClass>>();
            Assert.Throws<KeyNotFoundException>(() => factory.Create(""));
        }

        [Fact]
        public void Create_NotRegisteredInSerializerFactory_ExpectNoThrow()
        {
            var factory = GetDataFactory();

            factory.Register<TestDataClass, ReferenceType<TestDataClass>>();
            Assert.NotNull(factory.Create(new TestDataClass(), new TestDataClassSerializer()));
        }

        [Fact]
        public void Create_ExpectNoThrow()
        {
            var sFactory = GetSerializerFactory();
            var dFactory = GetDataFactory(sFactory);

            sFactory.Register<TestDataClass, TestDataClassSerializer>();
            dFactory.Register<TestDataClass, ReferenceType<TestDataClass>>();
            Assert.NotNull(dFactory.Create(new TestDataClass()));
            Assert.NotNull(dFactory.Create(new TestDataClass(), new TestDataClassSerializer()));
        }

        #region PrivateHelpers

        private static DataFactory GetDataFactory()
        {
            return GetDataFactory(GetSerializerFactory());
        }

        private static DataFactory GetDataFactory(SerializerFactory serializerFactory)
        {
            return new DataFactory(serializerFactory);
        }

        private static SerializerFactory GetSerializerFactory()
        {
            return new SerializerFactory();
        }

        #endregion
    }
}