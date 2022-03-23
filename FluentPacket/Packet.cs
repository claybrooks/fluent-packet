using FluentPacket.Exception;
using FluentPacket.Factory;
using FluentPacket.Interfaces;
using FluentPacket.Serializer;
using FluentPacket.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FluentPacket
{
    public class Packet
    {
        private readonly IDictionary<int, Data> _taggedData = new Dictionary<int, Data>();
        private readonly IDictionary<int, Type> _taggedTypeHelper = new Dictionary<int, Type>();
        private readonly IList<Data> _data = new List<Data>();

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

        public void Register<T>()
            where T : struct
        {
            Register<T, ValueType<T>, ValueTypeSerializer<T>>(typeof(T).Name);
        }

        public void Register<T, S>(params object[] serializerConstructorParams)
            where T : class
            where S : ISerializer<T>
        {
            Register<T, ReferenceType<T>, S>(typeof(T).Name, serializerConstructorParams);
        }

        public void Register<T, D, S>(string name, params object[] serializerConstructorParams)
            where D : Data<T>
            where S : ISerializer<T>
        {
            TypeFactory.Register<T>(name);
            DataFactory.Register<T, D>();
            SerializerFactory.Register<T, S>(serializerConstructorParams);
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
            foreach (var data in _data)
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
        public DefaultPacket() : base()
        {
            DefaultRegister.EnableDefaultArrayTypeRegistration = true;
            DefaultRegister.EnableDefaultValueTypeRegistration = true;
            SerializerFactory.Register<bool, BoolSerializer>();
        }
    }
}
