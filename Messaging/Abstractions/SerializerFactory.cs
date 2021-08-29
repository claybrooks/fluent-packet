using System;
using System.Collections.Generic;
using Messaging.Interfaces;

namespace Messaging.Abstractions
{
    public class SerializerFactory : ISerializerFactory
    {
        readonly IDictionary<Type, ISerializer> _createdTypes = new Dictionary<Type, ISerializer>();

        public void Register<TK, TV>() where TV : ISerializer<TK>
        {
            var keyType = typeof(TK);
            var valueType = typeof(TV);

            if (_createdTypes.ContainsKey(keyType))
            {
                return;
            }

            if (valueType.IsAbstract || valueType.IsInterface)
            {
                throw new ArgumentException("Cannot create instance of interface or abstract class");
            }

            _createdTypes.Add(keyType, (ISerializer)Activator.CreateInstance(valueType, this));
        }

        public ISerializer<T> Get<T>()
        {
            var typeKey = typeof(T);

            if (_createdTypes.TryGetValue(typeKey, out ISerializer s))
            {
                return s as ISerializer<T>;
            }

            throw new Exception($"Type not registered with factory");
        }
    }
}
