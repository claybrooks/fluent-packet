using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Messaging.Interfaces;

namespace Messaging.Abstractions
{
    public class Builder<TP>
        where TP : Packet, new()
    {
        private TP _packet;
        private readonly MethodInfo _withDataTValue;
        private readonly MethodInfo _withDataTValueTag;

        public Builder()
        {
            _packet = new TP();

            var tbType = typeof(Builder<TP>);

            foreach (MethodInfo mi in tbType.GetMethods())
            {
                if (!mi.Name.Equals("WithData") || !mi.IsGenericMethod)
                {
                    continue;
                }

                var mp = mi.GetParameters();

                switch (mp.Length)
                {
                    case 3:
                        _withDataTValueTag = mi;
                        break;
                    case 2:
                        _withDataTValue = mi;
                        break;
                }
            }

            if (_withDataTValue == null || _withDataTValueTag == null)
            {
                throw new Exception(
                    "Unable to construct ConfigBuilder.  Could not find necessary 'WithData' functions");
            }
        }

        public Builder<TP> Setup()
        {
            _packet = new TP();
            return this;
        }

        public TP FromConfig(IConfigReader reader, string filename)
        {
            // TODO Fix this
            if (!BuildFromFile(reader, filename))
            {
                throw new Exception("Unable to build from file");
            }

            return Build();
        }

        public TP Build()
        {
            return _packet;
        }

        public Builder<TP> WithData<T>(T value, ISerializer<T>? serializer = null)
        {
            _packet.AddData(value, serializer);
            return this;
        }

        public Builder<TP> WithData<T>(T value, long tag, ISerializer<T>? serializer = null)
        {
            _packet.AddData(value, tag, serializer);
            return this;
        }

        #region Private Methods

        private bool BuildFromFile(IConfigReader reader, string file)
        {
            var configs = reader.Read(file);
            return BuildData(configs);
        }

        private bool BuildData(IEnumerable<IConfig> configs)
        {
            return configs.All(WithData);
        }

        private bool WithData(IConfig config)
        {
            if (!GetType(out var type, config.TypeName) || type == null)
            {
                return false;
            }

            var tag = config.Tag != null ? ResolveTag(config.Tag) : -1;

            object? value = null;
            ISerializer? serializer= null;

            if (type.IsArray)
            {
                if (config.TypeArgs == null || config.TypeArgs.Length != 1)
                {
                    throw new Exception("TypeArgs for array must be of size 1, indicating the length of the array");
                }
                int size = Convert.ToInt32(config.TypeArgs[0]);
                
                var elementType = type.GetElementType();
                if (elementType == null || elementType.IsGenericType || elementType.IsAbstract)
                {
                    throw new Exception("Invalid element type for array");
                }

                value = Array.CreateInstance(elementType, size);

                var serializerClass = typeof(Serializer.ArrayTypeSerializer<>);
                var typedSerializerClass = serializerClass.MakeGenericType(elementType);
                var oSerializer = Activator.CreateInstance(typedSerializerClass, size);
                if (oSerializer == null)
                {
                    throw new Exception("Unable to create ArrayTypeSerializer");
                }

                serializer = oSerializer as ISerializer;
                if (serializer == null)
                {
                    throw new Exception("Unable to create ISerializer from ArrayTypeSerializer");
                }
            }
            else
            {
                object?[]? parameters = null;
                bool success = false;
                if (config.TypeArgs != null && config.TypeArgs.Length > 0)
                {
                    var constructors = type.GetConstructors();
                    foreach (ConstructorInfo constructor in constructors)
                    {
                        var ps = constructor.GetParameters();
                        if (ps.Length != config.TypeArgs.Length)
                        {
                            continue;
                        }

                        success = true;
                        parameters = new object?[ps.Length];
                        for(int i = 0; i < ps.Length; i++)
                        {
                            try
                            {
                                parameters[i] = Convert.ChangeType(config.TypeArgs[i], ps[i].ParameterType);
                            }
                            catch(InvalidCastException)
                            {
                                success = false;
                                break;
                            }
                        }

                        if (success)
                        {
                            break;
                        }
                    }

                    if (!success)
                    {
                        throw new Exception("Unable to match TypeArgs to a constructor");
                    }
                }

                value = config.Value ?? Activator.CreateInstance(type, parameters);
                value = Convert.ChangeType(value, type);
            }

            if (value == null)
            {
                throw new Exception("Unable to create default value");
            }

            return tag switch
            {
                -1 => WithData(type, value, serializer),
                _ => WithData(type, value, tag, serializer)
            };
        }

        private bool WithData(Type type, object value, ISerializer? serializer = null)
        {
            return WithData(_withDataTValue, type, new object?[] { value, serializer });
        }

        private bool WithData(Type type, object value, long tag, ISerializer? serializer = null)
        {
            return WithData(_withDataTValueTag, type, new object?[] { value, tag, serializer });
        }

        private bool WithData(MethodInfo method, Type type, params object?[]? parameters)
        {
            MethodInfo genericMethod = method.MakeGenericMethod(type);
            genericMethod.Invoke(this, parameters);
            return true;
        }

        private bool GetType(out Type? type, string name)
        {
            type = _packet.TypeFactory.Get(name);
            return type != null;
        }

        private static long ResolveTag(string tag)
        {
            if (string.IsNullOrEmpty(tag))
            {
                return -1;
            }

            return (long)Convert.ChangeType(tag, typeof(long));
        }

        #endregion
    }

    public class DefaultBuilder : Builder<DefaultPacket>
    {
    }

}
