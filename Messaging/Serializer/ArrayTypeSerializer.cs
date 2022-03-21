using FluentPacket.Helper;
using FluentPacket.Interfaces;
using System;

namespace FluentPacket.Serializer
{
    public class ArrayTypeSerializer<T> : Serializer<T[]>
    {
        private readonly Lazy<ISerializer<T>> _typeSerializer;

        public int FixedSize { get; set; }

        public ArrayTypeSerializer(int fixedSize)
        {
            _typeSerializer = new Lazy<ISerializer<T>>(() => _factory.Get<T>());
            FixedSize = fixedSize;
        }

        public override bool Deserialize(out T[] value, byte[] data, int offset)
        {
            value = new T[FixedSize];

            var tLength = _typeSerializer.Value.Length();
            var tOffset = offset;
            for (var i = 0; i < value.Length; ++i)
            {
                if (!_typeSerializer.Value.Deserialize(out value[i], data, tOffset))
                {
                    return false;
                }

                tOffset += tLength;
            }

            return true;
        }

        public override byte[] Serialize(T[] value)
        {
            var tLength = _typeSerializer.Value.Length();
            var byteArray = new byte[Length()];

            if (value.Length == 0)
            {
                return byteArray;
            }

            var offset = 0;
            foreach (var v in value)
            {
                _typeSerializer.Value.Serialize(v, byteArray, offset);
                offset += tLength;
            }

            return byteArray;
        }

        public override int Length()
        {
            return FixedSize * _typeSerializer.Value.Length();
        }

        public static ISerializer<T> CreateArrayValueTypeSerializer(T value)
        {
            var elementType = TypeHelper.GetConcreteArrayElementType<T>();
            var serializerType = typeof(ArrayTypeSerializer<>);

            var s = serializerType.MakeGenericType(elementType);

            if (value is not Array valueArray)
            {
                throw new InvalidCastException($"Unable to cast {typeof(T).Name} to Array");
            }

            if (Activator.CreateInstance(s, valueArray.Length) is not ISerializer<T> serializer)
            {
                throw new InvalidCastException($"Unable to cast {s.Name} to {typeof(ISerializer<T>).Name}");
            }

            return serializer;
        }
    }
}