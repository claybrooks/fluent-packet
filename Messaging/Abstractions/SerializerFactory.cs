using System;
using System.Collections.Generic;
using Messaging.Interfaces;
using Messaging.Serializer;

namespace Messaging.Abstractions
{
    public class SerializerFactory
    {
        private readonly IDictionary<Type, ISerializer> _createdTypes = new Dictionary<Type, ISerializer>();

        public void Register<T, TS>(Action<ISerializer<T>> initializer) where TS : ISerializer<T>, new()
        {
            Register<T, TS>();
            
            initializer.Invoke(Get<T>());
        }

        public void Register<T, TS>() where TS : ISerializer<T>, new()
        {
            var keyType = typeof(T);
            var valueType = typeof(TS);

            if (_createdTypes.ContainsKey(keyType))
            {
                return;
            }

            if (valueType.IsAbstract || valueType.IsInterface)
            {
                throw new ArgumentException("Cannot create instance of interface or abstract class");
            }

            _createdTypes.Add(keyType, new TS());
        }

        public ISerializer<T> Get<T>()
        {
            var typeKey = typeof(T);

            if (!_createdTypes.ContainsKey(typeKey) && typeKey.IsValueType)
            {
                var helper = GetValueTypeRegisterHelper<T>();
                helper.Register(this);
            }

            if (!_createdTypes.TryGetValue(typeKey, out var s))
            {
                throw new Exception($"Type not registered with factory");
            }

            if (s is not ISerializer<T> ts)
            {
                throw new Exception("Registered serializer type incompatible with requested type");
            }

            ts.SetFactory(this);
            return ts;
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

        internal abstract class RegisterHelper
        {
            public abstract void Register(SerializerFactory factory);
        }

        internal class ValueTypeRegisterHelper<T> : RegisterHelper where T : struct
        {
            public override void Register(SerializerFactory factory)
            {
                factory.Register<T, ValueTypeSerializer<T>>();
            }
        }
    }
}
