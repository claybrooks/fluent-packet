using System;
using System.Collections.Generic;
using Messaging.Data;
using Messaging.Interfaces;

namespace Messaging.Abstractions
{

    public class DataFactory : IDataFactory
    {
        readonly IDictionary<Type, Type> _registeredType = new Dictionary<Type, Type>();
        readonly ISerializerFactory _factory;

        public DataFactory(ISerializerFactory factory)
        {
            _factory = factory;
        }

        public void Register<T>() where T : struct
        {
            Register<T, ValueType<T>>();
        }

        public void Register<TK, TV>() where TV : IData<TK>
        {
            var keyType = typeof(TK);
            var valueType = typeof(TV);

            if (valueType.IsAbstract || valueType.IsInterface)
            {
                throw new ArgumentException("Cannot create instance of interface or abstract class");
            }

            _registeredType.Add(keyType, valueType);
        }

        public IData<T> Create<T>()
        {
            return CreateWithArgs<T>(_factory);
        }

        public IData<T> Create<T>(T value)
        {
            return CreateWithArgs<T>(_factory, value);
        }

        private IData<T> CreateWithArgs<T>(params object[] args)
        {
            var typeKey = typeof(T);

            if (!_registeredType.ContainsKey(typeKey) && typeKey.IsValueType)
            {
                GetValueTypeRegisterHelper<T>().Register(this);
            }

            if (!_registeredType.TryGetValue(typeKey, out Type type))
            {
                throw new Exception($"Type not registered with factory");
            }


            return (IData<T>)Activator.CreateInstance(type, args);
        }

        private static RegisterHelper GetValueTypeRegisterHelper<T>()
        {
            var helperType = typeof(ValueTypeRegisterHelper<>).MakeGenericType(typeof(T));
            var ret = (RegisterHelper) Activator.CreateInstance(helperType);

            return ret;
        }
    }

    internal abstract class RegisterHelper
    {
        public abstract void Register(IDataFactory factory);
    }

    internal class ValueTypeRegisterHelper<T> : RegisterHelper where T : struct
    {
        public override void Register(IDataFactory factory)
        {
            factory.Register<T>();
        }
    }
}
