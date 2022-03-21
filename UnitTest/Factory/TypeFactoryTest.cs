using System;
using FluentPacket.Factory;
using Xunit;

namespace UnitTest.Factory
{
    public class TypeFactoryUnitTest
    {
        internal class TestType
        {
        };

        [Fact]
        public void NonPrimitive_NotRegistered_Get_ExpectThrow()
        {
            var factory = GetTypeFactory();
            Assert.Throws<ArgumentException>(() =>
            {
                factory.Get(nameof(TestType));
            });
        }

        [Fact]
        public void NonPrimitive_NotRegistered_Register_ExpectNoThrow()
        {
            var factory = GetTypeFactory();

            factory.Register<TestType>();
            var type = factory.Get(nameof(TestType));
            Assert.NotNull(type);
        }

        [Fact]
        public void NonPrimitive_NotRegistered_RegisterDifferentName_ExpectNoThrow()
        {
            var factory = GetTypeFactory();
            const string name = "TypeName";

            factory.Register<TestType>(name);
            var type = factory.Get(name);
            Assert.NotNull(type);
        }

        [Fact]
        public void Primitive_NotRegistered_Get_ExpectNoThrow()
        {
            GetPrimitiveWithNoRegistrationAndAssertNotNull<byte>();
            GetPrimitiveWithNoRegistrationAndAssertNotNull<char>();
            GetPrimitiveWithNoRegistrationAndAssertNotNull<short>();
            GetPrimitiveWithNoRegistrationAndAssertNotNull<ushort>();
            GetPrimitiveWithNoRegistrationAndAssertNotNull<int>();
            GetPrimitiveWithNoRegistrationAndAssertNotNull<uint>();
            GetPrimitiveWithNoRegistrationAndAssertNotNull<long>();
            GetPrimitiveWithNoRegistrationAndAssertNotNull<ulong>();
            GetPrimitiveWithNoRegistrationAndAssertNotNull<float>();
            GetPrimitiveWithNoRegistrationAndAssertNotNull<double>();
            GetPrimitiveWithNoRegistrationAndAssertNotNull<decimal>();
        }

        [Fact]
        public void Primitive_NotRegistered_RegisterDifferentName_ExpectNoThrow()
        {
            var factory = GetTypeFactory();
            const string name = "TypeName";

            factory.Register<int>(name);
            var type = factory.Get(name);
            Assert.NotNull(type);
        }

        #region PrivateHelpers

        private static void GetPrimitiveWithNoRegistrationAndAssertNotNull<T>()
            where T : struct
        {
            var factory = GetTypeFactory();
            Assert.NotNull(factory.Get(typeof(T).FullName!));
        }

        private static TypeFactory GetTypeFactory()
        {
            return new TypeFactory();
        }

        #endregion
    }
}