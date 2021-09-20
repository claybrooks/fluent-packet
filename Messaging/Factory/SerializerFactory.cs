using System;
using System.Collections.Generic;
using Messaging.Interfaces;

namespace Messaging.Factory
{
    public class SerializerFactory
    {
        private readonly IDictionary<Type, ISerializer> _createdTypes = new Dictionary<Type, ISerializer>();

        public void Register<T, TS>(Action<TS>? initializer = null)
            where TS : ISerializer, new()
        {
            var keyType = typeof(T);
            var valueType = typeof(TS);

            if (_createdTypes.ContainsKey(keyType))
            {
                return;
            }

            if (valueType.IsAbstract || valueType.IsInterface)
            {
                throw new ArgumentException($"{nameof(T)} is not concrete");
            }

            _createdTypes.Add(keyType, new TS());

            if (initializer == null)
            {
                return;
            }

            if (Get<T>() is not TS s)
            {
                throw new InvalidCastException($"Registered serializer for type {nameof(T)} could not be case to {nameof(TS)}");
            }

            initializer.Invoke(s);
        }

        public ISerializer<T> Get<T>()
        {
            var typeKey = typeof(T);
            
            if (!_createdTypes.TryGetValue(typeKey, out var s))
            {
                throw new KeyNotFoundException($"Type {typeKey.Name} is not registered with {nameof(SerializerFactory)}");
            }

            if (s is not ISerializer<T> ts)
            {
                throw new InvalidCastException($"Registered serializer for type {nameof(T)} cannot be cast to type {nameof(ISerializer<T>)}incompatible with requested type");
            }

            ts.SetFactory(this);
            return ts;
        }
    }
}
