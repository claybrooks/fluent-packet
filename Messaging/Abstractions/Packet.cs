using System;
using System.Collections.Generic;
using System.Linq;

namespace Messaging.Abstractions
{
    public class Packet
    {
        readonly ICollection<Data> _data;
        readonly IDictionary<long, Data> _taggedData;

        public DataFactory DataFactory { get; }

        public SerializerFactory SerializerFactory { get; }

        public TypeFactory TypeFactory { get; }


        public Packet()
        {
            _data = new List<Data>();
            _taggedData = new Dictionary<long, Data>();

            // TODO Allow these to be injected or rethink their use
            SerializerFactory = new SerializerFactory();
            DataFactory = new DataFactory(SerializerFactory);
            TypeFactory = new TypeFactory();

            DataFactory.Register<string, Messaging.Data.String>();
        }

        public bool AddData<T>(T value)
        {
            _data.Add(DataFactory.Create(value));
            return true;
        }

        public bool AddData<T>(T value, long tag)
        {
            var data = DataFactory.Create(value);
            _data.Add(data);
            _taggedData.Add(tag, data);
            return true;
        }

        public T GetData<T>(long tag)
        {
            _taggedData.TryGetValue(tag, out var data);

            if (data == null)
            {
                throw new ArgumentException($"Unknown tag {tag}");
            }

            if (data is not Data<T> typedData)
            {
                throw new Exception($"Provided generic type is invalid for the type registered against tag {tag}");
            }

            return typedData.Value;
        }

        public bool SetData<T>(long tag, T value)
        {
            _taggedData.TryGetValue(tag, out var data);

            if (data == null)
            {
                return false;
            }

            if (data is not Data<T> typedData)
            {
                throw new Exception($"Provided generic type is invalid for the type registered against tag {tag}");
            }

            typedData.Value = value;
            return true;
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
    }
}
