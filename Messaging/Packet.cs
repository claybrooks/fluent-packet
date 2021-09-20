using System;
using System.Collections.Generic;
using System.Linq;
using Messaging.Exception;
using Messaging.Factory;
using Messaging.Types;
using Messaging.Interfaces;
using Messaging.Serializer;

namespace Messaging
{
    public class Packet
    {
        protected readonly IDictionary<int, Data> _taggedData = new Dictionary<int, Data>();
        protected readonly IDictionary<int, Type> _taggedTypeHelper = new Dictionary<int, Type>();
        protected readonly IList<Data> _data = new List<Data>();

        public DataFactory DataFactory { get; }

        public SerializerFactory SerializerFactory { get; }

        public TypeFactory TypeFactory { get; }

        public DefaultRegister DefaultRegister { get; }

        public Packet()
        {
            DefaultRegister = new DefaultRegister();
            
            SerializerFactory = new SerializerFactory();
            DataFactory = new DataFactory(SerializerFactory);
            TypeFactory = new TypeFactory();
        }

        public void Register<T>(Action<ISerializer<T>>? initializer = null)
            where T : struct
        {
            Register<T, ValueType<T>, ValueTypeSerializer<T>>(typeof(T).Name, initializer);
        }

        public void Register<T, TS>(Action<TS>? initializer = null)
            where T : class
            where TS : ISerializer<T>, new()
        {
            Register<T, ReferenceType<T>, TS>(typeof(T).Name, initializer);
        }

        public void Register<T, TD, TS>(string name, Action<TS>? initializer = null)
            where TD : Data<T>
            where TS : ISerializer<T>, new()
        {
            TypeFactory.Register<T>(name);
            DataFactory.Register<T, TD>();
            SerializerFactory.Register<T, TS>(initializer);
        }

        public bool AddData<T>(T value, ISerializer<T>? serializer = null)
        {
            DefaultRegister.HandleDefaultRegistration(value, ref serializer, DataFactory, SerializerFactory);

            _data.Add(DataFactory.Create(value, serializer));
            return true;
        }

        public bool AddData<T>(T value, int tag, ISerializer<T>? serializer = null)
        {
            DefaultRegister.HandleDefaultRegistration(value, ref serializer, DataFactory, SerializerFactory);

            var data = DataFactory.Create(value, serializer);
            _data.Add(data);
            _taggedData.Add(tag, data);
            _taggedTypeHelper.Add(tag, typeof(T));

            return true;
        }

        public bool SetData<T>(int tag, T value)
        {
            if (!_taggedData.TryGetValue(tag, out var data))
            {
                throw new KeyNotFoundException($"Unregistered tag {tag} was used");
            }

            if (data is not Data<T> typedData)
            {
                throw new InvalidCastForTagException<T>(tag, GetRegisteredTaggedType(tag));
            }

            typedData.Value = value;
            return true;

        }

        public T GetData<T>(int tag)
        {
            if (!_taggedData.TryGetValue(tag, out var data))
            {
                throw new KeyNotFoundException($"Tag {tag} is not registered");
            }

            if (data is not Data<T> typedData)
            {
                throw new InvalidCastForTagException<T>(tag, GetRegisteredTaggedType(tag));
            }

            return typedData.Value;
        }

        public int Length()
        {
            return _data.Sum(data => data.Length());
        }

        public bool Deserialize(byte[] bytes)
        {
            var offset = 0;
            foreach(var data in _data)
            {
                if (!data.Deserialize(bytes, offset))
                {
                    return false;
                }

                offset += data.Length();
            }

            return true;
        }

        public bool Serialize(out byte[] bytes)
        {
            bytes = new byte[Length()];

            var idx = 0;
            foreach (var data in _data)
            {
                idx += data.Serialize(bytes, idx);
            }
            return true;
        }

        public void ClearTagged()
        {
            foreach (Data data in _taggedData.Values)
            {
                data.Clear();
            }
        }

        #region PrivateHelper

        private Type? GetRegisteredTaggedType(int tag)
        {
            return _taggedTypeHelper.TryGetValue(tag, out var type) ? type : null;
        }

        #endregion
    }

    public class DefaultPacket : Packet
    {
        public DefaultPacket()
        {
            DefaultRegister.EnableDefaultArrayTypeRegistration = true;
            DefaultRegister.EnableDefaultValueTypeRegistration = true;
            SerializerFactory.Register<bool, BoolSerializer>();
        }
    }
}
