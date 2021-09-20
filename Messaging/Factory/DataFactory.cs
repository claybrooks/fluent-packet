using System;
using System.Collections.Generic;
using Messaging.Exception;
using Messaging.Types;
using Messaging.Interfaces;

namespace Messaging.Factory
{
    public class DataFactory
    {
        private readonly IDictionary<Type, Type> _registeredType = new Dictionary<Type, Type>();

        private readonly SerializerFactory _serializerFactory;

        public DataFactory(SerializerFactory factory)
        {
            _serializerFactory = factory ?? throw new ArgumentNullException(nameof(factory));
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

            if (_registeredType.ContainsKey(keyType))
            {
                return;
            }

            _registeredType.Add(keyType, valueType);
        }

        public Data<T> Create<T>(T value, ISerializer<T>? serializer = null)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (!TryGetType<T>(out var type))
            {
                throw new KeyNotFoundException($"{typeof(T).Name} is not registered with {nameof(DataFactory)}");
            }

            if (type == null)
            {
                throw new NullValueRegisteredException(nameof(DataFactory), typeof(T).Name);
            }

            serializer ??= _serializerFactory.Get<T>();

            if (Activator.CreateInstance(type, value, serializer) is not Data<T> o)
            {
                throw new InvalidCastException($"Unable to cast {type.Name} to {nameof(Data<T>)}");
            }

            return o;
        }

        private bool TryGetType<T>(out Type? type)
        {
            return _registeredType.TryGetValue(typeof(T), out type);
        }
    }
}
