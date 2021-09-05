using System;

using Messaging.Abstractions;
using Messaging.Interfaces;

namespace Messaging.Serializer
{
    public class ArrayTypeSerializer<T> : Serializer<T[]>
    {
        private readonly Lazy<ISerializer<T>> _typSerializer;

        private int FixedSize { get; set; }

        public ArrayTypeSerializer(int fixedSize)
        {
            _typSerializer = new Lazy<ISerializer<T>>(() => _factory.Get<T>());
            FixedSize = fixedSize;
        }

        public override bool Deserialize(ref T[] value, byte[] data, int offset)
        {
            if (value.Length != FixedSize)
            {
                value = new T[FixedSize];
            }

            var tLength = _typSerializer.Value.Length();
            var tOffset = offset;
            for (var i = 0; i < value.Length; ++i)
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
            return FixedSize * _typSerializer.Value.Length();
        }
    }
}