using FluentPacket.Interfaces;
using System;
using System.Collections.Generic;

namespace FluentPacket.Factory
{
    public class SerializerFactory
    {
        private readonly IDictionary<Type, Type> _typeMap = new Dictionary<Type, Type>();
        private readonly IDictionary<Type, object[]> _argMap = new Dictionary<Type, object[]>();

        public void Register<T, S>(params object[] constructorArgs)
            where S : ISerializer
        {
            var keyType = typeof(T);

            if (_typeMap.ContainsKey(keyType))
            {
                return;
            }

            _typeMap.Add(keyType, typeof(S));

            if (constructorArgs.Length > 0)
            {
                _argMap.Add(keyType, constructorArgs);
            }
        }

        public ISerializer<T> Get<T>()
        {
            var typeKey = typeof(T);
            
            if (!_typeMap.ContainsKey(typeKey))
            {
                throw new KeyNotFoundException($"Type {typeKey.Name} is not registered with {nameof(SerializerFactory)}");
            }

            object[] constructorArgs = {};
            if (_argMap.ContainsKey(typeKey))
            {
                constructorArgs = _argMap[typeKey];
            }

            var s = Activator.CreateInstance(_typeMap[typeKey], constructorArgs);

            if (s is not ISerializer<T> ts)
            {
                throw new InvalidCastException($"Registered serializer for type {nameof(T)} cannot be cast to type {nameof(ISerializer<T>)}");
            }
            ts.SetFactory(this);

            return ts;
        }
    }
}
