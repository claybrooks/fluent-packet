using System;

using Messaging.Abstractions;
using Messaging.Interfaces;

namespace Messaging.Serializer
{
    public class ArrayValueTypeSerializer<T> : Serializer<T[]> where T : struct
    {
        private readonly Lazy<ISerializer<T>> _typSerializer;
        // TODO make this configurable
        private readonly int _fixedSize = 5;

        public ArrayValueTypeSerializer()
        {
            _typSerializer = new Lazy<ISerializer<T>>(() => _factory.Get<T>());
        }

        public override bool Deserialize(ref T[] value, byte[] data, int offset)
        {
            if (value.Length != _fixedSize)
            {
                value = new T[_fixedSize];
            }

            var tLength = _typSerializer.Value.Length();
            var tOffset = offset;
            for (var i = 0; i < _fixedSize; ++i)
            {
                if (!_typSerializer.Value.Deserialize(ref value[i], data, tOffset))
                {
                    return false;
                }

                tOffset += tLength;
            }

            return true;
        }

        public override byte[] Serialize(T[] value)
        {
            var tLength = _typSerializer.Value.Length();
            var byteArray = new byte[Length()];

            if (value.Length == 0)
            {
                return byteArray;
            }
            
            var offset = 0;
            foreach (var v in value)
            {
                _typSerializer.Value.Serialize(v, byteArray, offset);
                offset += tLength;
            }

            return byteArray;
        }

        public override int Length()
        {
            return _fixedSize * _typSerializer.Value.Length();
        }
    }
}