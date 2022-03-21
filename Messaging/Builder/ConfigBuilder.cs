using FluentPacket.Exception;
using FluentPacket.Helper;
using FluentPacket.Interfaces;
using System;
using System.Reflection;

namespace FluentPacket.Builder
{
    public class ConfigBuilder<P, C> : Builder<P>
        where P : Packet, new()
        where C : IConfigReader, new()
    {

        private readonly MethodInfo _withDataTValue;
        private readonly MethodInfo _withDataTValueTag;

        private string _filename = "";

        public ConfigBuilder()
        {
            var tbType = typeof(Builder<P>);

            foreach (var methodInfo in tbType.GetMethods())
            {
                if (!IsGenericWithData(methodInfo))
                {
                    continue;
                }

                switch (methodInfo.GetParameters().Length)
                {
                    case 3:
                        _withDataTValueTag = methodInfo;
                        break;
                    case 2:
                        _withDataTValue = methodInfo;
                        break;
                }
            }

            if (_withDataTValue == null || _withDataTValueTag == null)
            {
                throw new ConfigBuilderWithDataParseException(_withDataTValue, _withDataTValueTag);
            }
        }
        
        public ConfigBuilder<P, C> WithConfig(string filename)
        {
            _filename = filename;
            return this;
        }

        public override void Assemble()
        {
            base.Assemble();
            BuildFromFile(new C(), _filename);
        }

        #region Private Methods
        
        private static bool IsGenericWithData(MethodBase info)
        {
            return info.Name.Equals("WithData") && info.IsGenericMethod;
        }


        private void BuildFromFile(IConfigReader reader, string file)
        {
            foreach (IConfig config in reader.Read(file))
            {
                WithConfig(config);
            }
        }

        private bool WithConfig(IConfig config)
        {
            var type = GetType(config.TypeName);

            var tag = ResolveTag(config);
            
            ISerializer? serializer = null;
            var value = type.IsArray ? (object) WithConfig_Array(config, type, out serializer) : (object) WithConfig_Object(config, type);

            return tag switch
            {
                -1 => WithData(type, value, serializer),
                _ => WithData(type, value, tag, serializer)
            };
        }

        private static object WithConfig_Array(IConfig config, Type type, out ISerializer serializer)
        {
            if (config.TypeArgs is not { Length: 1 })
            {
                throw new ArgumentException(
                    "Config.TypeArgs must contain one parameter, which is the size of the array", nameof(config));
            }

            var size = Convert.ToInt32(config.TypeArgs[0]);

            var elementType = TypeHelper.GetConcreteArrayElementType(type);

            var value = Array.CreateInstance(elementType, size);

            var serializerClass = typeof(Serializer.ArrayTypeSerializer<>);
            var typedSerializerClass = serializerClass.MakeGenericType(elementType);

            if (Activator.CreateInstance(typedSerializerClass, size) is not ISerializer s)
            {
                throw new InvalidCastException($"Unable to cast {typedSerializerClass.Name} to {nameof(ISerializer)}");
            }

            serializer = s;

            return value;
        }

        private static object WithConfig_Object(IConfig config, Type type)
        {
            object[]? parameters = {};

            if (TypeHelper.HasConstructorArguments(config) && !TypeHelper.TryGetParameters(config, type, out parameters))
            {
                throw new ArgumentException("Unable to match constructor parameters for provided config");
            }

            var value = config.Value ?? Activator.CreateInstance(type, parameters);
            value = Convert.ChangeType(value, type);

            if (value == null)
            {
                throw new InvalidCastException($"Unable to change type");
            }
            return value;
        }

        private bool WithData(Type type, object value, ISerializer? serializer = null)
        {
            return WithData(_withDataTValue, type, value, serializer);
        }

        private bool WithData(Type type, object value, int tag, ISerializer? serializer = null)
        {
            return WithData(_withDataTValueTag, type, value, tag, serializer);
        }

        private bool WithData(MethodInfo method, Type type, params object?[]? parameters)
        {
            MethodInfo genericMethod = method.MakeGenericMethod(type);
            genericMethod.Invoke(this, parameters);
            return true;
        }

        private Type GetType(string name)
        {
            return _packet.TypeFactory.Get(name);
        }

        private static int ResolveTag(IConfig? config)
        {
            if (config == null)
            {
                return -1;
            }

            if (string.IsNullOrEmpty(config.Tag))
            {
                return -1;
            }

            return (int)Convert.ChangeType(config.Tag, typeof(int));
        }

        #endregion
    }
}
