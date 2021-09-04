using System;
using System.Collections.Generic;
using Messaging.Data;

namespace Messaging.Abstractions
{

    public class DataFactory
    {
        readonly IDictionary<Type, Type> _registeredType = new Dictionary<Type, Type>();

        readonly SerializerFactory _serializerFactory;

        public DataFactory(SerializerFactory factory)
        {
            _serializerFactory = factory;
        }

        public void Register<T>() where T : struct
        {
            Register<T, ValueType<T>>();
        }

        public void Register<TK, TV>() where TV : Data
        {
            var keyType = typeof(TK);
            var valueType = typeof(TV);

            if (valueType.IsAbstract || valueType.IsInterface)
            {
                throw new ArgumentException("Cannot create instance of interface or abstract class");
            }

            _registeredType.Add(keyType, valueType);
        }

        public Data<T> Create<T>(T value)
        {
            if (value == null)
            {
                throw new Exception("Can't create with null value");
            }

            if (!TryGetType<T>(out var type))
            {
                throw new Exception("Type not registered");
            }

            var o = Activator.CreateInstance(type, value);
            if (o == null)
            {
                throw new Exception($"Unable to create type");
            }

            if (o is not Data<T> d)
            {
                throw new Exception($"Registered type incompatible with requested type");
            }

            d.SetSerializer(_serializerFactory.Get<T>());

            return d;
        }

        private bool TryGetType<T>(out Type type)
        {
            var t = typeof(T);
            
            if (_registeredType.ContainsKey(t))
            {
                return _registeredType.TryGetValue(t, out type!);
            }

            if (t.IsValueType)
            {
                GetValueTypeRegisterHelper<T>().Register(this);
            }
            else if (t.IsArray)
            {
                GetArrayValueTypeRegisterHelper<T>().Register(this);
            }

            return _registeredType.TryGetValue(t, out type!);
        }

        private static RegisterHelper GetValueTypeRegisterHelper<T>()
        {
            var helperType = typeof(ValueTypeRegisterHelper<>).MakeGenericType(typeof(T));

            var o = Activator.CreateInstance(helperType);
            if (o == null)
            {
                throw new Exception($"Unable to create helper");
            }

            if (o is not RegisterHelper r)
            {
                throw new Exception($"Unable to create helper");
            }

            return r;
        }

        private static RegisterHelper GetArrayValueTypeRegisterHelper<T>()
        {
            var elementType = typeof(T).GetElementType();
            if (elementType == null)
            {
                throw new Exception($"Unable to create helper");
            }

            var helperType = typeof(ArrayValueTypeRegisterHelper<,>).MakeGenericType(typeof(T), elementType);

            var o = Activator.CreateInstance(helperType);
            if (o == null)
            {
                throw new Exception($"Unable to create helper");
            }

            if (o is not RegisterHelper r)
            {
                throw new Exception($"Unable to create helper");
            }

            return r;
        }
    }

    internal abstract class RegisterHelper
    {
        public abstract void Register(DataFactory factory);
    }

    internal class ValueTypeRegisterHelper<T> : RegisterHelper where T : struct
    {
        public override void Register(DataFactory factory)
        {
            factory.Register<T>();
        }
    }

    internal class ArrayValueTypeRegisterHelper<T, TE> : RegisterHelper where TE : struct
    {
        public override void Register(DataFactory factory)
        {
            factory.Register<T, ArrayValueType<TE>>();
        }
    }
}
