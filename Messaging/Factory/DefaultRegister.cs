using FluentPacket.Helper;
using FluentPacket.Interfaces;
using FluentPacket.Serializer;
using FluentPacket.Types;
using System;

namespace FluentPacket.Factory
{
    public class DefaultRegister
    {
        public bool EnableDefaultArrayTypeRegistration { get; set; }

        public bool EnableDefaultValueTypeRegistration { get; set; }

        public void HandleDefaultRegistration<T>(T value, ref ISerializer<T>? serializer, DataFactory dataFactory, SerializerFactory serializerFactory)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (dataFactory == null)
            {
                throw new ArgumentNullException(nameof(dataFactory));
            }

            if (serializerFactory == null)
            {
                throw new ArgumentNullException(nameof(serializerFactory));
            }

            TryRegisterHelperRegistration(value, ref serializer, dataFactory, serializerFactory);
        }

        #region Private Helpers

        private bool ShouldDefaultArrayValueTypeRegistration<T>()
        {
            return EnableDefaultArrayTypeRegistration && TypeHelper.IsArrayValueType<T>();
        }

        private bool ShouldDefaultValueTypeRegistration<T>()
        {
            return EnableDefaultValueTypeRegistration && TypeHelper.IsValueType<T>();
        }

        private void TryRegisterHelperRegistration<T>(T value, ref ISerializer<T>? serializer, DataFactory dataFactory, SerializerFactory serializerFactory)
        {
            RegisterHelper? helper = null;

            if (ShouldDefaultArrayValueTypeRegistration<T>())
            {
                helper = GetArrayValueTypeRegistrationHelper<T>();
                serializer = ArrayTypeSerializer<T>.CreateArrayValueTypeSerializer(value);

                // TODO I don't like this here but I haven't found a better spot to put this yet
                serializer.SetFactory(serializerFactory);
            }
            else if (ShouldDefaultValueTypeRegistration<T>())
            {
                helper = GetValueTypeRegistrationHelper<T>();
            }

            helper?.Register(dataFactory, serializerFactory);
        }

        private static RegisterHelper GetArrayValueTypeRegistrationHelper<T>()
        {
            var elementType = TypeHelper.GetConcreteArrayElementType<T>();

            var helperType = typeof(ArrayValueTypeRegisterHelper<,>).MakeGenericType(typeof(T), elementType);

            if (Activator.CreateInstance(helperType) is not RegisterHelper r)
            {
                throw new InvalidCastException($"Unable to get RegisterHelper for array");
            }

            return r;
        }

        private static RegisterHelper GetValueTypeRegistrationHelper<T>()
        {
            var helperType = typeof(ValueTypeRegisterHelper<>).MakeGenericType(typeof(T));

            if (Activator.CreateInstance(helperType) is not RegisterHelper r)
            {
                throw new InvalidCastException($"Unable to create RegisterHelper");
            }

            return r;
        }

        internal abstract class RegisterHelper
        {
            public abstract void Register(DataFactory dataFactory, SerializerFactory serializerFactory);
        }

        internal class ValueTypeRegisterHelper<T> : RegisterHelper where T : struct
        {
            public override void Register(DataFactory dataFactory, SerializerFactory serializerFactory)
            {
                dataFactory.Register<T, ValueType<T>>();
                serializerFactory.Register<T, ValueTypeSerializer<T>>();
            }
        }

        internal class ArrayValueTypeRegisterHelper<T, TE> : RegisterHelper where TE : struct
        {
            public override void Register(DataFactory dataFactory, SerializerFactory serializerFactory)
            {
                dataFactory.Register<T, ArrayValueType<TE>>();
            }
        }

        #endregion
    }
}
